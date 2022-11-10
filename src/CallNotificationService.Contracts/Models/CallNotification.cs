// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Contracts.Models
{
    public class CallNotification
    {
        public string Id { get; init; }

        public string From { get; init; }

        public string To { get; init; }

        public string? CallerDisplayName { get; init; }

        public string ApplicationId { get; init; }

        public string CorrelationId { get; init; }

        public string MidCallEventsUri { get; init; }
    }
}