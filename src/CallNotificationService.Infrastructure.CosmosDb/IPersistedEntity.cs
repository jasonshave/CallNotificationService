// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Infrastructure.CosmosDb;

public interface IPersistedEntity
{
    /// <summary>
    /// Concurrency token of persisted entity.
    /// </summary>
    public string? ETag { get; set; }

    /// <summary>
    /// Last modified timestamp of persisted entity as UnixTimeSeconds.
    /// </summary>
    public long? LastModified { get; set; }
}