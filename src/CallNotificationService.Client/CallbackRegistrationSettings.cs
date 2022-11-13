// Copyright (c) 2022 Jason Shave. All rights reserved.
// Copyright (c) 2022 Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Client;

public sealed class CallbackRegistrationSettings
{
    public string? ApplicationId { get; set; }

    public string CallbackHost { get; set; } = string.Empty;

    public double LifetimeInMinutes { get; set; }

    public List<string> Targets { get; set; } = new();

    public string CallNotificationPath { get; set; } = string.Empty;

    public string MidCallEventsPath { get; set; } = string.Empty;
}