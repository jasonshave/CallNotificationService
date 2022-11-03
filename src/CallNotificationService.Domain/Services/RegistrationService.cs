// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallNotificationService.Domain.Interfaces;
using CallNotificationService.Domain.Models;
using CallNotificationService.Persistence;

namespace CallNotificationService.Domain.Services;

public class RegistrationService : IRegistrationService
{
    private readonly IRepository<CallbackRegistration> _repository;

    public RegistrationService(IRepository<CallbackRegistration> repository)
    {
        _repository = repository;
        _repository.Save(new CallbackRegistration()
        {
            ApplicationId = Guid.NewGuid().ToString(),
            CallbackUri = "https://myserver.com/abc",
            Id = Guid.NewGuid().ToString(),
            UpdatedOn = DateTimeOffset.UtcNow,
            Targets = new List<string>() { "abc", "123" }
        });
    }

    public async Task<CallbackRegistration?> GetRegistration(string id)
    {
        try
        {
            var result = await _repository.GetAsync(id);
            return result;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public async Task<CallbackRegistration> SetRegistrationAsync(CallbackRegistration registration)
    {
        var result = await _repository.SaveAsync(registration);
        return result;
    }

    public async Task<IEnumerable<CallbackRegistration>> ListRegistrationsByCallbackUri(string callbackUri)
    {
        var results = await _repository.ListAsync();
        return results.Where(x => x.CallbackUri == callbackUri);
    }

    public async Task<IEnumerable<CallbackRegistration>> ListRegistrationsByTarget(string target)
    {
        var results = await _repository.ListAsync();
        return results.Where(x => x.Targets.Contains(target));
    }

    public async Task<IEnumerable<CallbackRegistration>> ListRegistrationsByApplicationId(string applicationId)
    {
        var results = await _repository.ListAsync();
        return results.Where(x => x.ApplicationId.Contains(applicationId));
    }
}