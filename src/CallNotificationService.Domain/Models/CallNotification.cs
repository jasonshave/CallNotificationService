// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Domain.Models
{
    public record CallNotification(
        string Id,
        string From,
        string To,
        string? CallerDisplayName,
        string? NotificationContext,
        string CorrelationId,
        string? ServerCallId);
}
