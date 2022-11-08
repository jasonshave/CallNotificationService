// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallNotificationService.App.Profiles;
using CallNotificationService.Domain.Interfaces;
using CallNotificationService.Domain.Models;
using CallNotificationService.Domain.Services;
using CallNotificationService.Infrastructure.CosmosDb;
using CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;
using CallNotificationService.Infrastructure.WebhookSender.Services;
using CallNotificationService.Persistence.Profiles;
using CallNotificationService.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using System.Reflection;
using Azure.Communication.CallAutomation;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(configuration =>
    {
        configuration.AddUserSecrets(Assembly.GetExecutingAssembly());
    })
    .ConfigureServices((hostBuilder, services) =>
    {
        services.AddSingleton<INotificationService, NotificationService>();
        services.AddSingleton<IRegistrationService, RegistrationService>();
        services.AddSingleton<ISender<Notification>, WebhookCallbackSender>();

        services.AddSingleton<ICrudRepository<CallbackRegistration>, RegistrationRepository>();
        services.AddSingleton<ICrudRepository<Notification>, NotificationRepository>();

        services.AddAutoMapper(typeof(RestProfile), typeof(PersistenceProfile));
        services.AddHttpClient<CallbackClient>()
            .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));

        services.AddCosmosDb(cfg => hostBuilder.Configuration.Bind(nameof(CosmosDbConfiguration), cfg));
        services.AddSingleton(new CallAutomationClient(hostBuilder.Configuration["ACS:ConnectionString"]));
    })
    .Build();

host.Services.UseCosmosDb();

await host.RunAsync();