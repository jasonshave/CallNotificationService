// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallNotificationService.Domain.Abstractions.Interfaces;

namespace CallNotificationService.Persistence
{
    public interface IRepository<T>
        where T : IEntity
    {
        Task<T> SaveAsync(T entity);

        T Save(T entity);

        Task<T> GetAsync(string id);

        Task<IEnumerable<T>> ListAsync();
    }
}
