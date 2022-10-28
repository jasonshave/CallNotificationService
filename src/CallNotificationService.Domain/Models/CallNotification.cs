// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallNotificationService.Domain.Models
{
    internal record CallNotification(
        string Id,
        string From,
        string To,
        string? CallerDisplayName,
        string? NotificationContext,
        string CorrelationId,
        string? ServerCallId);
}
