// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;

public interface ICrudRepository<TDomainModel>
    where TDomainModel : IEntity
{
    Task<TDomainModel> Create(TDomainModel model, double ttlInSeconds = default, CancellationToken cancellationToken = default);

    Task<TDomainModel> Get(string resourceId, string id, CancellationToken cancellationToken = default);

    Task<TDomainModel?> Find(string resourceId, string id, CancellationToken cancellationToken = default);

    Task<TDomainModel> Upsert(TDomainModel model, double ttlInSeconds = default, CancellationToken cancellationToken = default);

    Task<bool> CheckIfItemExists(string resourceId, string id, CancellationToken cancellationToken = default);

    Task<IEnumerable<TDomainModel>> List(string resourceId, int pageSize, CancellationToken cancellationToken = default);

    Task<bool> Delete(string resourceId, string id, IPreconditions? preconditions = null, CancellationToken cancellationToken = default);
}