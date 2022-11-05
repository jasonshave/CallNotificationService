// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using AutoMapper;
using CallNotificationService.Domain.Models;
using CallNotificationService.Infrastructure.CosmosDb;
using CallNotificationService.Persistence.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace CallNotificationService.Persistence;

public class RegistrationRepository : CosmosDbCrudRepository<CallbackRegistration, PersistedCallbackRegistration>
{
    protected override string ContainerId => "CallbackRegistrations";

    public RegistrationRepository(Database db, IMapper mapper, ILoggerFactory loggerFactory)
        : base(db, mapper, loggerFactory)
    {
    }
}