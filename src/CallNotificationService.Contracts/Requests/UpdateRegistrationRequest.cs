// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Contracts.Requests;

public class UpdateRegistrationRequest
{
    public List<string> Targets { get; set; }

    public string CallbackUri { get; set; }
}