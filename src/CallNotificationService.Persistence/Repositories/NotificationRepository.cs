// Copyright (c) 2022 Jason Shave. All rights reserved.
// Copyright (c) 2022 Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AutoMapper;
using CallNotificationService.Domain.Models;
using CallNotificationService.Infrastructure.CosmosDb;
using CallNotificationService.Persistence.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace CallNotificationService.Persistence.Repositories;

public class NotificationRepository : CosmosDbCrudRepository<Notification, PersistedNotification>
{
    protected override string ContainerId => "CallNotifications";

    public NotificationRepository(Database db, IMapper mapper, ILoggerFactory loggerFactory)
        : base(db, mapper, loggerFactory)
    {
    }
}