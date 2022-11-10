// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces
{
    public interface IRepository<TDomainModel>
        where TDomainModel : IEntity
    {
        Task<TDomainModel> UpsertAsync(TDomainModel entity);

        TDomainModel Upsert(TDomainModel entity);

        Task<TDomainModel> GetAsync(string resourceId, string id);

        Task<IEnumerable<TDomainModel>> ListAsync();
    }
}
