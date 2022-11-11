// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallNotificationService.Domain.Interfaces;
using CallNotificationService.Domain.Models;
using CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;

namespace CallNotificationService.Domain.Services;

internal sealed class RegistrationService : IRegistrationService
{
    private readonly ICrudRepository<Registration> _repository;

    public RegistrationService(ICrudRepository<Registration> repository)
    {
        _repository = repository;
    }

    public async Task<Registration?> GetRegistration(string resourceId, string id)
    {
        try
        {
            var result = await _repository.Get(resourceId, id);
            return result;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<Registration> SetRegistrationAsync(Registration registration)
    {
        var result = await _repository.Upsert(registration, registration.LifetimeInMinutes * 60);
        return result;
    }

    public async Task RemoveRegistration(string resourceId, string id)
    {
        await _repository.Delete(resourceId, id);
    }

    public async Task<IEnumerable<Registration>> ListRegistrationsByTarget(string resourceId, string target)
    {
        var results = await _repository.List(resourceId, 100);
        return results.Where(x => x.Targets.Contains(target));
    }

    public async Task<IEnumerable<Registration>> ListRegistrationsByApplicationId(string resourceId, string applicationId)
    {
        var results = await _repository.List(resourceId, 100);
        return results.Where(x => x.ApplicationId == applicationId);
    }

    public async Task<IEnumerable<Registration>> ListRegistrations(string resourceId)
    {
        return await _repository.List(resourceId, 100);
    }
}