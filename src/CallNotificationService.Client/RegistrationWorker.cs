// Copyright (c) 2022 Jason Shave. All rights reserved.
// Copyright (c) 2022 Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace CallNotificationService.Client;

public class RegistrationWorker : BackgroundService
{
    private readonly ICallNotificationClient _client;
    private readonly CallNotificationClientSettings _settings;

    private CallbackRegistration? _registration;

    public RegistrationWorker(
        ICallNotificationClient client,
        CallNotificationClientSettings settings)
    {
        _client = client;
        _settings = settings;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await DeRegister();
        await base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _registration = await _client.SetRegistrationAsync(_settings);
            _settings.ApplicationId ??= _registration.ApplicationId;

            await Task.Delay(TimeSpan.FromMinutes(_settings.RegistrationLifetimeInMinutes / 1.2), stoppingToken);
            await DeRegister();
        }
    }

    private async Task DeRegister()
    {
        await _client.DeRegister(_registration.ApplicationId);
    }
}