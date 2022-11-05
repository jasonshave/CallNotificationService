// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using AutoMapper;
using Azure;
using Azure.Data.Tables;
using CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CallNotificationService.Infrastructure.AzureTableStorage
{
    public abstract class TableStorageRepository<TDomainModel, TPersistedModel> : IRepository<TDomainModel>, IProvisionableRepository
        where TDomainModel : IEntity
        where TPersistedModel : class, ITableEntity, new()
    {
        private readonly IMapper _mapper;
        private readonly TableClient _tableClient;

        public TableStorageRepository(IMapper mapper, IConfiguration configuration, string tableName)
        {
            _mapper = mapper;
            var tableServiceClient = new TableServiceClient(configuration["Storage:ConnectionString"]);
            _tableClient = tableServiceClient.GetTableClient(tableName);
        }

        public async Task<TDomainModel> GetAsync(string resourceId, string id)
        {
            Response<TPersistedModel> result = await _tableClient.GetEntityAsync<TPersistedModel>(resourceId, id);
            var response = _mapper.Map<TDomainModel>(result);
            return response;
        }

        public async Task<IEnumerable<TDomainModel>> ListAsync()
        {
            var results = new List<TDomainModel>();
            var queryResults = _tableClient.QueryAsync<TPersistedModel>();
            await foreach (TPersistedModel persistedData in queryResults)
            {
                var domainData = _mapper.Map<TDomainModel>(persistedData);
                results.Add(domainData);
            }

            return results;
        }

        public TDomainModel Upsert(TDomainModel entity)
        {
            var persistedEntity = _mapper.Map<TPersistedModel>(entity);

            _tableClient.UpsertEntityAsync(persistedEntity);
            return entity;
        }

        public async Task<TDomainModel> UpsertAsync(TDomainModel entity)
        {
            var persistedEntity = _mapper.Map<TPersistedModel>(entity);
            await _tableClient.AddEntityAsync(persistedEntity);
            return entity;
        }

        public virtual async Task ProvisionAsync(CancellationToken cancellationToken)
        {
            await _tableClient.CreateIfNotExistsAsync(cancellationToken);
        }
    }
}