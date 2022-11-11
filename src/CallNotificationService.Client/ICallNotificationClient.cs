// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Client;

public interface ICallNotificationClient
{
    Task<CallbackRegistration?> SetRegistration(Action<CreateRegistration> options);

    Task<bool> DeRegister(string applicationId);

    Task<CallbackRegistration?> GetRegistration(string applicationId);

    Task<IEnumerable<CallbackRegistration>> ListRegistrations();
}