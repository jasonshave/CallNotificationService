// Copyright (c) 2022 Jason Shave. All rights reserved.
// Copyright (c) 2022 Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CallNotificationService.Client;

public static class ServiceCollectionExtensions
{
    public static CallNotificationClientBuilder AddCallNotificationServiceClient(this IServiceCollection services,
        IConfiguration configuration, string? visualStudioTunnelUri = null)
    {
        var builder = new CallNotificationClientBuilder(configuration, services, visualStudioTunnelUri);

        builder.Build();

        return builder;
    }

    public static void AddRegistrationWorker(this CallNotificationClientBuilder builder)
    {
        builder.Services.AddHostedService<RegistrationWorker>();
    }

    public static void AddRegistrationWorker<TWorker>(this CallNotificationClientBuilder builder)
        where TWorker : BackgroundService
    {
        builder.Services.AddHostedService<TWorker>();
    }
}