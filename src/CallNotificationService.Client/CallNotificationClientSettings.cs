// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Client;

public sealed class CallNotificationClientSettings
{
    public string SetRegistrationEndpointUri { get; set; }

    public string DeRegisterEndpointUri { get; set; }

    public string GetRegistrationEndpointUri { get; set; }

    public string ListRegistrationsEndpointUri { get; set; }
}