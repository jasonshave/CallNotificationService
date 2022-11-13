// Copyright (c) 2022 Jason Shave. All rights reserved.
// Copyright (c) 2022 Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Domain.Models;

public sealed class NotificationSettings
{
    public bool EnableSendIncomingCallContext { get; set; }

    public double TimeToLiveInSeconds { get; set; }
}