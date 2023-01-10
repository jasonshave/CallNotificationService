// Copyright (c) 2022 Jason Shave. All rights reserved.
// Copyright (c) 2022 Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;

namespace CallNotificationService.Domain.Models;

public sealed class Registration : IEntity
{
    public string Id { get; set; } = string.Empty;

    public string ResourceId => "ACS";

    public string ApplicationId { get; set; } = string.Empty;

    public string[] Targets { get; set; } = Array.Empty<string>();

    public string CallNotificationUri { get; set; } = string.Empty;

    public string? MidCallEventsUri { get; set; }

    public double LifetimeInMinutes { get; set; } = 60;

    public DateTimeOffset UpdatedOn { get; set; }

    public DateTimeOffset ExpiresOn { get; set; }
}