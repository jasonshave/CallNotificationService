// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallNotificationService.Infrastructure.CosmosDb;

namespace CallNotificationService.Persistence.Models;

public class PersistedCallbackRegistration : BaseCosmosEntity
{
    public string ApplicationId { get; set; }

    public List<string> Targets { get; set; } = new();

    public string CallbackUri { get; set; }
}