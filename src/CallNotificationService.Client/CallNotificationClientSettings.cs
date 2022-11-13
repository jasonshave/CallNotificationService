// Copyright (c) 2022 Jason Shave. All rights reserved.
// Copyright (c) 2022 Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Client;

public sealed class CallNotificationClientSettings
{
    public string SetRegistrationEndpointUri { get; set; } = string.Empty;

    public string DeRegisterEndpointUri { get; set; } = string.Empty;

    public string GetRegistrationEndpointUri { get; set; } = string.Empty;

    public string ListRegistrationsEndpointUri { get; set; } = string.Empty;
}