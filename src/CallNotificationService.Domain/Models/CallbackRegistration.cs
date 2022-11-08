// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;

namespace CallNotificationService.Domain.Models;

public class CallbackRegistration : IRegistration, IEntity
{
    public string Id { get; set; }

    public string ResourceId => "ACS";

    public string ApplicationId { get; set; }

    public string[] Targets { get; set; }

    public string CallNotificationUri { get; set; }

    public string MidCallEventsUri { get; set; }

    public double LifetimeInMinutes { get; set; }

    public DateTimeOffset UpdatedOn { get; set; }

    public DateTimeOffset ExpiresOn { get; set; }
}