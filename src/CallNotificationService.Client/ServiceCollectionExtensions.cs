// Copyright (c) 2022 Jason Shave. All rights reserved.
// Copyright (c) 2022 Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CallNotificationService.Client;

public static class ServiceCollectionExtensions
{
    public static void AddCallNotificationServiceClient(this IServiceCollection services,
        Action<CallNotificationClientSettings> options)
    {
        var settings = new CallNotificationClientSettings();
        options(settings);
        var builder = new CallNotificationClientBuilder(services, settings);
        builder.Build();
    }

    public static void AddCallNotificationServiceClient(this IServiceCollection services,
        CallNotificationClientSettings settings)
    {
        var builder = new CallNotificationClientBuilder(services, settings);
        builder.Build();
    }

    public static void AddCallNotificationServiceClient(this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = new CallNotificationClientSettings();
        configuration.Bind(nameof(CallNotificationClientSettings), settings);
        var builder = new CallNotificationClientBuilder(services, settings);
        builder.Build();
    }
}