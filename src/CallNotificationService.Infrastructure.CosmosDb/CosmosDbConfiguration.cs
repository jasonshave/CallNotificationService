// Copyright (c) 2022 Jason Shave. All rights reserved.
// Copyright (c) 2022 Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Infrastructure.CosmosDb;

public sealed class CosmosDbConfiguration
{
    public string ConnectionString { get; init; }

    public string Database { get; init; }

    public Dictionary<string, int> Tables { get; init; } = new();
}