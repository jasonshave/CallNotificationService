﻿// Copyright (c) 2022 Jason Shave. All rights reserved.
// Copyright (c) 2022 Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Client;

public interface ICallNotificationClient
{
    Task<CallbackRegistration?> SetRegistrationAsync(Action<CallNotificationClientSettings> options);

    Task<CallbackRegistration?> SetRegistrationAsync(CallNotificationClientSettings options);

    Task<bool> DeRegister(string applicationId);

    Task<CallbackRegistration?> GetRegistration(string applicationId);

    Task<IEnumerable<CallbackRegistration>> ListRegistrations();
}