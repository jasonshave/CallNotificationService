// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace CallNotificationService.Infrastructure.CosmosDb;

public class BaseCosmosEntity
{
    [JsonProperty("id")]
    public string Id { get; init; }

    public string ResourceId { get; init; }

    [JsonProperty("ttl")]
    public double TimeToLiveInSeconds { get; set; }

    [JsonProperty("_etag")]
    public string? ETag { get; init; }
}