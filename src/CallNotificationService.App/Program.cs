// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallAutomation.Contracts;
using CallNotificationService.Domain.Interfaces;
using CallNotificationService.Domain.Models;
using CallNotificationService.Domain.Services;
using CallNotificationService.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddSingleton<IRegistrationService, RegistrationService>();
        services.AddSingleton<IPublisherService<IncomingCall, CallNotification>, CallbackPublisher<IncomingCall, CallNotification>>();
        services.AddSingleton<ICallbackLocator<IncomingCall>, IncomingCallCallbackLocator>();
        services.AddSingleton(typeof(IRepository<>), typeof(InMemoryRepository<>));

        services.AddAutoMapper(typeof(Program));
        services.AddHttpClient();
    })
    .Build();

await host.RunAsync();