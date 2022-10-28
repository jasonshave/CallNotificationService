// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Domain.Models;

public class CallbackRegistration
{
    public string Id { get; set; }

    public string ApplicationId { get; set; }

    public List<string> Targets { get; set; } = new();

    public Uri CallbackUri { get; set; }

    public DateTimeOffset CreatedOn { get; set; }
}