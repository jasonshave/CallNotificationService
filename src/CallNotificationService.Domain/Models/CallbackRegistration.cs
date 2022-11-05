// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;

namespace CallNotificationService.Domain.Models;

public class CallbackRegistration : IEntity
{
    public string Id { get; set; }

    public string ResourceId => "ACS";

    public string ApplicationId { get; set; }

    public List<string> Targets { get; set; } = new();

    public string CallbackUri { get; set; }

    public DateTimeOffset UpdatedOn { get; set; }
}