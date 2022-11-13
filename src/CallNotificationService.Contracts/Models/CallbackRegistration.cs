// Copyright (c) 2022 Jason Shave. All rights reserved.
// Copyright (c) 2022 Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Contracts.Models;

public sealed class CallbackRegistration
{
    public string ApplicationId { get; set; }

    public string[] Targets { get; set; }

    public string CallNotificationUri { get; set; }

    public string MidCallEventsUri { get; set; }

    public DateTimeOffset UpdatedOn { get; set; }

    public DateTimeOffset ExpiresOn { get; set; }
}