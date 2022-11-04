// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Contracts.Models;

public class CallbackRegistrationDto
{
    public string Id { get; set; }

    public string ApplicationId { get; set; }

    public List<string> Targets { get; set; } = new();

    public string CallbackUri { get; set; }

    public DateTimeOffset UpdatedOn { get; set; }
}