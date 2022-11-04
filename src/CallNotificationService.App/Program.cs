// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallAutomation.Contracts;
using CallNotificationService.Contracts.Models;
using CallNotificationService.Dispatcher;
using CallNotificationService.Domain.Abstractions;
using CallNotificationService.Domain.Abstractions.Interfaces;
using CallNotificationService.Domain.Interfaces;
using CallNotificationService.Domain.Services;
using CallNotificationService.Infrastructure.WebhookSender.Services;
using CallNotificationService.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddSingleton<IRegistrationService, RegistrationService>();
        services.AddSingleton<IPublisherService<IncomingCall, CallNotification>, CallbackPublisher<IncomingCall, CallNotification>>();
        services.AddSingleton<IEventConverter<IncomingCall, CallNotification>, IncomingCallEventConverter>();
        services.AddSingleton<ICallbackLocator<IncomingCall>, IncomingCallCallbackLocator>();
        services.AddSingleton<ISender, WebhookCallbackSender>();
        services.AddSingleton(typeof(IRepository<>), typeof(InMemoryRepository<>));

        services.AddHostedService<CallNotificationDispatcher>();

        services.AddAutoMapper(typeof(Program));
        services.AddHttpClient();
    })
    .Build();

await host.RunAsync();