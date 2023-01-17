// Copyright (c) 2022 Jason Shave. All rights reserved.
// Copyright (c) 2022 Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.ComponentModel;

namespace CallNotificationService.Infrastructure.CosmosDb;

public sealed class CosmosDbProvisioner : IStorageProvisioner
{
    private readonly IOptions<CosmosDbConfiguration> _configuration;
    private readonly Database _db;
    private readonly ILogger<CosmosDbProvisioner> _logger;
    private readonly CosmosClient _cosmosClient;

    public CosmosDbProvisioner(
        IOptions<CosmosDbConfiguration> configuration,
        Database db,
        ILogger<CosmosDbProvisioner> logger)
    {
        _configuration = configuration;
        _db = db;
        _logger = logger;
        _cosmosClient = new CosmosClient(_configuration.Value.ConnectionString);
    }

    public async Task ProvisionAsync()
    {
        // provision database
        DatabaseResponse databaseResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_configuration.Value.Database).ConfigureAwait(false);
        _logger.LogInformation("Create database status code: {statusCode} | ID: {id}", databaseResponse.StatusCode, databaseResponse.Database.Id);

        _logger.LogInformation("Found {numContainers} containers to provision.", _configuration.Value.Tables.Count);
        foreach (var table in _configuration.Value.Tables)
        {
            try
            {
                ContainerProperties props = new(table.Key, @"/ResourceId");
                ContainerResponse containerResponse = await _db.CreateContainerIfNotExistsAsync(props);
                _logger.LogInformation("Create container in {database} status code: {statusCode} | ID: {id}", databaseResponse.Database.Id, containerResponse.StatusCode, containerResponse.Container.Id);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message, e);
                throw;
            }
        }
    }
}