// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CallNotificationService.Infrastructure.AzureTableStorage;

public static class ServiceCollectionExtensions
{
    public static IServiceProvider UseTableStorage(this IServiceProvider services)
    {
        var provisionableRepository = services.GetRequiredService<IProvisionableRepository>();
        provisionableRepository.ProvisionAsync().Wait();

        return services;
    }
}