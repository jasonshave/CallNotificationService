// Copyright (c) 2022 Jason Shave. All rights reserved.
// Copyright (c) 2022 Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CallNotificationService.Infrastructure.CosmosDb;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCosmosDb(this IServiceCollection services, Action<CosmosDbConfiguration> configAction)
    {
        services.Configure(configAction);
        services.AddSingleton<IStorageProvisioner, CosmosDbProvisioner>();

        var config = new CosmosDbConfiguration();
        configAction(config);

        var client = new CosmosClient(config.ConnectionString);
        Database db = client.GetDatabase(config.Database);

        services.AddSingleton(db);

        return services;
    }

    public static IServiceProvider UseCosmosDb(this IServiceProvider services)
    {
        var provisioner = services.GetService<IStorageProvisioner>();

        if (provisioner is null) throw new ArgumentNullException($"Unable to locate {nameof(IStorageProvisioner)}");
        provisioner.ProvisionAsync().Wait();

        return services;
    }
}