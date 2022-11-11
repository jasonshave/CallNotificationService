// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Client;

public sealed class CreateRegistration
{
    public string? ApplicationId { get; set; }

    public List<string> Targets { get; set; } = new();

    public string CallNotificationUri { get; set; }

    public string MidCallEventsUri { get; set; }

    public double LifetimeInMinutes { get; set; }
}