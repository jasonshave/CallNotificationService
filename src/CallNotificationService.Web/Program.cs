// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using AutoMapper;
using Azure.Messaging.EventGrid;
using CallNotificationService.Contracts.Models;
using CallNotificationService.Contracts.Requests;
using CallNotificationService.Domain.Abstractions.Interfaces;
using CallNotificationService.Domain.Interfaces;
using CallNotificationService.Domain.Models;
using CallNotificationService.Domain.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddSingleton<IRegistrationService, RegistrationService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/api/registration", async (CreateRegistrationRequest request, IRegistrationService service, IMapper mapper) =>
{
    var registration = new CallbackRegistration()
    {
        ApplicationId = request.ApplicationId,
        CallbackUri = request.CallbackUri.ToString(),
        UpdatedOn = DateTimeOffset.UtcNow,
        Id = Guid.NewGuid().ToString(),
        Targets = request.Targets,
    };

    var result = await service.SetRegistrationAsync(registration);
    var response = mapper.Map<CallbackRegistrationDto>(result);
    return Results.Created("/api/registration/{id}", response);
});

app.MapGet("/api/registration/{id}", async (string id, IRegistrationService service, IMapper mapper) =>
{
    var result = await service.GetRegistration("ACS", id);
    var response = mapper.Map<CallbackRegistrationDto>(result);
    return Results.Ok(response);
});

app.MapGet("/api/registration", async (string callbackUri, IRegistrationService service, IMapper mapper) =>
{
    List<CallbackRegistrationDto> registrations = new();
    var results = await service.ListRegistrationsByCallbackUri(callbackUri);
    foreach (var result in results)
    {
        var response = mapper.Map<CallbackRegistrationDto>(result);
        registrations.Add(response);
    }

    return Results.Ok(registrations);
});

app.MapGet("/api/registration/target/{target}", async (string target, IRegistrationService service, IMapper mapper) =>
{
    List<CallbackRegistrationDto> registrations = new();
    var results = await service.ListRegistrationsByTarget(target);
    foreach (var result in results)
    {
        var response = mapper.Map<CallbackRegistrationDto>(result);
        registrations.Add(response);
    }

    return Results.Ok(registrations);
});

app.MapGet("/api/registration/application/{applicationId}", async (string applicationId, IRegistrationService service, IMapper mapper) =>
{
    List<CallbackRegistrationDto> registrations = new();
    var results = await service.ListRegistrationsByApplicationId(applicationId);
    foreach (var result in results)
    {
        var response = mapper.Map<CallbackRegistrationDto>(result);
        registrations.Add(response);
    }

    return Results.Ok(registrations);
});

app.MapPost("/api/publish", async (EventGridEvent[] events, IPublisherService<EventGridEvent, CallNotification> service) =>
{
    foreach (var eventGridEvent in events)
    {
        await service.PublishAsync(eventGridEvent);
    }
});

app.Run();
