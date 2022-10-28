// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallNotificationService.Domain.Models;
using CallNotificationService.Persistence;

namespace CallNotificationService.Domain.Services;

internal class RegistrationService
{
    private readonly IRepository<CallbackRegistration> _repository;

    public RegistrationService(IRepository<CallbackRegistration> repository)
    {
        _repository = repository;
    }

    public async Task<CallbackRegistration> Register(CallbackRegistration registration)
    {
        await _repository.Save(registration);
        return registration;
    }
}