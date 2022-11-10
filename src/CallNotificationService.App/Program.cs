// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using Azure.Communication.Identity;
using CallNotificationService.App.Profiles;
using CallNotificationService.Domain.Interfaces;
using CallNotificationService.Domain.Models;
using CallNotificationService.Domain.Services;
using CallNotificationService.Infrastructure.AcsIdentity;
using CallNotificationService.Infrastructure.CosmosDb;
using CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;
using CallNotificationService.Infrastructure.TokenService;
using CallNotificationService.Infrastructure.WebhookSender.Services;
using CallNotificationService.Persistence.Profiles;
using CallNotificationService.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using System.Reflection;

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
        services.AddSingleton<ITokenService, TokenService>();
        services.AddSingleton<IApplicationIdentityService, AcsIdentityService>();

        services.AddSingleton<ICrudRepository<CallbackRegistration>, RegistrationRepository>();
        services.AddSingleton<ICrudRepository<Notification>, NotificationRepository>();

        services.AddAutoMapper(typeof(RestProfile), typeof(PersistenceProfile));
        services.AddHttpClient<CallbackClient>()
            .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));

        services.AddCosmosDb(cfg => hostBuilder.Configuration.Bind(nameof(CosmosDbConfiguration), cfg));
        services.AddSingleton(new CallAutomationClient(hostBuilder.Configuration["ACS:ConnectionString"]));
        services.AddSingleton(new CommunicationIdentityClient(hostBuilder.Configuration["ACS:ConnectionString"]));

        services.Configure<TokenConfiguration>(hostBuilder.Configuration.GetSection(nameof(TokenConfiguration)));
        services.Configure<NotificationSettings>(hostBuilder.Configuration.GetSection(nameof(NotificationSettings)));
    })
    .Build();

host.Services.UseCosmosDb();

await host.RunAsync();