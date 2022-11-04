// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Contracts.Requests;

public class CreateRegistrationRequest
{
    public string ApplicationId { get; init; }

    public List<string> Targets { get; set; }

    public Uri CallbackUri { get; init; }
}