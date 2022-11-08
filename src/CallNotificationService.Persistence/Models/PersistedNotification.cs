// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallNotificationService.Infrastructure.CosmosDb;

namespace CallNotificationService.Persistence.Models;

public class PersistedNotification : BaseCosmosEntity
{
    public string CorrelationId { get; init; }

    public string IncomingCallContext { get; init; }

    public string MidCallEventsUri { get; init; }
}