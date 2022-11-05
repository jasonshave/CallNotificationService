// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallAutomation.Contracts;
using CallNotificationService.App.Profiles;
using CallNotificationService.Contracts.Models;
using CallNotificationService.Domain.Interfaces;
using CallNotificationService.Domain.Models;
using CallNotificationService.Domain.Services;
using CallNotificationService.Infrastructure.CosmosDb;
using CallNotificationService.Infrastructure.Domain.Abstractions;
using CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;
using CallNotificationService.Infrastructure.WebhookSender.Services;
using CallNotificationService.Persistence;
using CallNotificationService.Persistence.Profiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(configuration =>
    {
        configuration.AddUserSecrets(Assembly.GetExecutingAssembly());
    })
    .ConfigureServices((hostBuilder, services) =>
    {
        services.AddSingleton<IRegistrationService, RegistrationService>();
        services.AddSingleton<IPublisherService<IncomingCall, CallNotification>, CallbackPublisher<IncomingCall, CallNotification>>();
        services.AddSingleton<IEventConverter<IncomingCall, CallNotification>, IncomingCallEventConverter>();
        services.AddSingleton<ICallbackLocator<IncomingCall>, IncomingCallCallbackLocator>();
        services.AddSingleton<ISender, WebhookCallbackSender>();
        services.AddSingleton<ICrudRepository<CallbackRegistration>, RegistrationRepository>();

        services.AddAutoMapper(typeof(RestProfile), typeof(TableStorageProfile));
        services.AddHttpClient();

        services.AddCosmosDb(cfg => hostBuilder.Configuration.Bind(nameof(CosmosDbConfiguration), cfg));
    })
    .Build();

host.Services.UseCosmosDb();

await host.RunAsync();