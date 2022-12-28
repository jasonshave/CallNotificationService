// Copyright (c) 2022 Jason Shave. All rights reserved.
// Copyright (c) 2022 Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CallNotificationService.Client;

public class CallNotificationClientBuilder
{
    private readonly string? _visualStudioTunnelUri;

    public IServiceCollection Services { get; }

    private IConfiguration Configuration { get; }

    public CallNotificationClientBuilder(IConfiguration configuration, IServiceCollection services, string? visualStudioTunnelUri = null)
    {
        Configuration = configuration;
        Services = services;
        _visualStudioTunnelUri = visualStudioTunnelUri;
    }

    public void Build()
    {
        var callNotificationClientSettings = new CallNotificationClientSettings();
        Configuration.Bind(nameof(CallNotificationClientSettings), callNotificationClientSettings);
        Services.AddSingleton(callNotificationClientSettings);

        var callbackRegistrationSettings = new CallbackRegistrationSettings();
        if (_visualStudioTunnelUri is not null)
        {
            callbackRegistrationSettings.CallbackHost = _visualStudioTunnelUri;
        }

        Configuration.Bind(nameof(CallbackRegistrationSettings), callbackRegistrationSettings);
        Services.AddSingleton(callbackRegistrationSettings);

        Services.AddHttpClient<CallNotificationHttpClient>();
        Services.AddSingleton<ICallNotificationClient, CallNotificationClient>();
    }
}