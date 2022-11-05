// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallNotificationService.Domain.Interfaces;
using CallNotificationService.Domain.Models;
using CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;

namespace CallNotificationService.Domain.Services;

public class RegistrationService : IRegistrationService
{
    private readonly ICrudRepository<CallbackRegistration> _repository;

    public RegistrationService(ICrudRepository<CallbackRegistration> repository)
    {
        _repository = repository;
    }

    public async Task<CallbackRegistration?> GetRegistration(string resourceId, string id)
    {
        try
        {
            var result = await _repository.Get(resourceId, id);
            return result;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public async Task<CallbackRegistration> SetRegistrationAsync(CallbackRegistration registration)
    {
        var result = await _repository.Upsert(registration);
        return result;
    }

    //public async Task<IEnumerable<CallbackRegistration>> ListRegistrationsByCallbackUri(string resourceId, string callbackUri)
    //{
    //    var results = await _repository.List(resourceId, 100);
    //    return results.Where(x => x.CallbackUri == callbackUri);
    //}

    public async Task<IEnumerable<CallbackRegistration>> ListRegistrationsByTarget(string resourceId, string target)
    {
        var results = await _repository.List(resourceId, 100);
        return results.Where(x => x.Targets.Contains(target));
    }

    //public async Task<IEnumerable<CallbackRegistration>> ListRegistrationsByApplicationId(string applicationId)
    //{
    //    var results = await _repository.ListAsync();
    //    return results.Where(x => x.ApplicationId.Contains(applicationId));
    //}
}