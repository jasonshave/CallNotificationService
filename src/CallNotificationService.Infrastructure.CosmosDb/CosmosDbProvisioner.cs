// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace CallNotificationService.Infrastructure.CosmosDb;

public class CosmosDbProvisioner : IStorageProvisioner
{
    private readonly IOptions<CosmosDbConfiguration> _configuration;
    private readonly Database _db;
    private readonly CosmosClient _cosmosClient;

    public CosmosDbProvisioner(IOptions<CosmosDbConfiguration> configuration, Database db)
    {
        _configuration = configuration;
        _db = db;
        _cosmosClient = new CosmosClient(_configuration.Value.ConnectionString);
    }

    public async Task ProvisionAsync()
    {
        // provision database
        await _cosmosClient.CreateDatabaseIfNotExistsAsync(_configuration.Value.Database).ConfigureAwait(false);

        var tasks = new List<Task>();
        foreach (var table in _configuration.Value.Tables)
        {
            ContainerProperties props = new(table.Key, @"/ResourceId")
            {
                DefaultTimeToLive = table.Value
            };
            tasks.Add(_db.CreateContainerIfNotExistsAsync(props));
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }
}