// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallNotificationService.Domain.Abstractions.Interfaces;

namespace CallNotificationService.Persistence;

public class InMemoryRepository<T> : IRepository<T>
    where T : IEntity
{
    private readonly Dictionary<string, T> _entities = new();

    public T Save(T entity)
    {
        _entities.TryAdd(entity.Id, entity);
        return entity;
    }

    public Task<T> SaveAsync(T entity)
    {
        _entities.TryAdd(entity.Id, entity);
        return Task.FromResult(entity);
    }

    public Task<T> GetAsync(string id)
    {
        var result = _entities[id];
        return Task.FromResult(result);
    }

    public Task<IEnumerable<T>> ListAsync()
    {
        var entities = _entities.Values.ToList();
        return Task.FromResult(entities.AsEnumerable());
    }
}