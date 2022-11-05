// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Infrastructure.CosmosDb;

public class CosmosDbConfiguration
{
    public string ConnectionString { get; init; }

    public string Database { get; init; }

    public List<string> Tables { get; init; } = new();
}