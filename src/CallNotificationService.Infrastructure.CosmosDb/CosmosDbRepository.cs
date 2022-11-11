// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using AutoMapper;
using CallNotificationService.Infrastructure.Domain.Abstractions.Exceptions;
using CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Net;

namespace CallNotificationService.Infrastructure.CosmosDb
{
    public abstract class CosmosDbCrudRepository<TDomainModel, TPersistedModel> : ICrudRepository<TDomainModel>
        where TDomainModel : IEntity
        where TPersistedModel : BaseCosmosEntity
    {
        protected abstract string ContainerId { get; }

        private readonly Lazy<Container> _lazyContainer;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private Database _db;

        private Container Container => _lazyContainer.Value;

        protected CosmosDbCrudRepository(
            Database db,
            IMapper mapper,
            ILoggerFactory loggerFactory)
        {
            _db = db;
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger(GetType());
            _lazyContainer = new Lazy<Container>(() => _db.GetContainer(ContainerId));
        }

        public async Task<TDomainModel> Create(TDomainModel model, double ttlInSeconds = default, CancellationToken cancellationToken = default)
        {
            try
            {
                var persistedModel = _mapper.Map<TPersistedModel>(model);
                if (ttlInSeconds > 0)
                    persistedModel.TimeToLiveInSeconds = ttlInSeconds;

                var result = await Container.CreateItemAsync(persistedModel, new PartitionKey(model.ResourceId), cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                return _mapper.Map<TDomainModel>(result.Resource);
            }
            catch (CosmosException ex)
            {
                throw TranslateCosmosException(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<TDomainModel> Get(string resourceId, string id, CancellationToken cancellationToken = default)
        {
            try
            {
                var model = await Container.ReadItemAsync<TPersistedModel>(id, new PartitionKey(resourceId), cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                return _mapper.Map<TDomainModel>(model.Resource);
            }
            catch (CosmosException ex)
            {
                throw TranslateCosmosException(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<TDomainModel?> Find(string resourceId, string id, CancellationToken cancellationToken = default)
        {
            try
            {
                var model = await Container.ReadItemAsync<TPersistedModel>(id, new PartitionKey(resourceId), cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                return _mapper.Map<TDomainModel>(model.Resource);
            }
            catch (CosmosException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    return default;
                }

                throw TranslateCosmosException(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<TDomainModel> Upsert(TDomainModel model, double ttlInSeconds = default, CancellationToken cancellationToken = default) =>
            await Upsert(model, ttlInSeconds, null, cancellationToken);

        public async Task<bool> CheckIfItemExists(string resourceId, string id, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await Container.ReadItemAsync<TPersistedModel>(id, new PartitionKey(resourceId), cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                return true;
            }
            catch (CosmosException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    return false;
                }

                throw TranslateCosmosException(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<TDomainModel>> List(string resourceId, int pageSize = 5000, CancellationToken cancellationToken = default)
        {
            List<TPersistedModel> results = new();
            var result = Container.GetItemQueryIterator<TPersistedModel>(new QueryDefinition("SELECT * FROM c"), null, new QueryRequestOptions()
            {
                PartitionKey = new PartitionKey(resourceId),
                MaxItemCount = pageSize,
            });

            var page = await result.ReadNextAsync(cancellationToken).ConfigureAwait(false);
            results.AddRange(page);
            return results.Select(r => _mapper.Map<TDomainModel>(r));
        }

        public async Task<bool> Delete(string resourceId, string id, IPreconditions? preconditions = null, CancellationToken cancellationToken = default)
        {
            bool deleted;
            try
            {
                var options = new ItemRequestOptions();
                await ApplyConcurrencyHeaders(resourceId, id, preconditions, options);

                await Container.DeleteItemAsync<TPersistedModel>(id, new PartitionKey(resourceId), options, cancellationToken).ConfigureAwait(false);
                deleted = true;
            }
            catch (CosmosException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    deleted = false;
                    return deleted;
                }

                throw TranslateCosmosException(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }

            return deleted;
        }

        protected virtual Exception TranslateCosmosException(CosmosException exception) => exception.StatusCode switch
        {
            HttpStatusCode.NotFound => new NotFoundException("The item was not found.", exception),
            HttpStatusCode.Conflict => new ConflictException("The current state of the item conflicts with the operation.", exception),
            HttpStatusCode.PreconditionFailed => new PreconditionFailedException("The item was in an unexpected state.", exception),
            _ => exception,
        };

        protected virtual async Task ApplyConcurrencyHeaders(string resourceId, string id, IPreconditions? preconditions, ItemRequestOptions requestOptions)
        {
            if (preconditions == null)
                return;

            if (!string.IsNullOrEmpty(preconditions.IfMatch))
            {
                requestOptions.IfMatchEtag = preconditions.IfMatch;
            }

            if (preconditions.IfUnmodifiedSince.HasValue)
            {
                var precheck = await Container.ReadItemStreamAsync(id, new PartitionKey(resourceId));
                if (precheck.IsSuccessStatusCode)
                {
                    var lastModified = DateTimeOffset.FromUnixTimeSeconds(_db.Client.ClientOptions.Serializer.FromStream<CosmosLastModified>(precheck.Content)._ts);
                    if (lastModified > preconditions.IfUnmodifiedSince)
                        throw new PreconditionFailedException();
                    if (!string.IsNullOrEmpty(preconditions.IfMatch) && precheck.Headers.ETag != preconditions.IfMatch)
                        throw new PreconditionFailedException();
                    requestOptions.IfMatchEtag = precheck.Headers.ETag;
                }
            }
        }

        private async Task<TDomainModel> Upsert(TDomainModel model, double ttlInSeconds = default, IPreconditions? preconditions = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var persistedModel = _mapper.Map<TPersistedModel>(model);
                if (ttlInSeconds > 0)
                    persistedModel.TimeToLiveInSeconds = ttlInSeconds;

                var options = new ItemRequestOptions();
                await ApplyConcurrencyHeaders(model.ResourceId, model.Id, preconditions, options);
                var result = await Container.UpsertItemAsync(persistedModel, new PartitionKey(model.ResourceId), options, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                return _mapper.Map<TDomainModel>(result.Resource);
            }
            catch (CosmosException ex)
            {
                throw TranslateCosmosException(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}