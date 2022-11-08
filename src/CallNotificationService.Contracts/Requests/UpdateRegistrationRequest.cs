// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Contracts.Requests;

public class UpdateRegistrationRequest
{
    public string ApplicationId { get; set; }

    public string[] Targets { get; set; }

    public string CallNotificationUri { get; set; }

    public string MidCallEventsUri { get; set; }
}