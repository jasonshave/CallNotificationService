// Copyright (c) 2022 Jason Shave. All rights reserved.
// Copyright (c) 2022 Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.DependencyInjection;

namespace CallNotificationService.Client;

public class CallNotificationClientBuilder
{
    private readonly IServiceCollection _services;
    private readonly CallNotificationClientSettings _settings;

    public CallNotificationClientBuilder(IServiceCollection services, CallNotificationClientSettings settings)
    {
        _services = services;
        _settings = settings;
    }

    public void Build()
    {
        _services.AddHttpClient<CallNotificationHttpClient>();
        _services.AddSingleton(_settings);
        _services.AddSingleton<ICallNotificationClient, CallNotificationClient>();
    }
}