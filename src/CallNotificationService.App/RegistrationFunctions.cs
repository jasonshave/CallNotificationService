// Copyright (c) 2022 Jason Shave. All rights reserved.
// Copyright (c) 2022 Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AutoMapper;
using CallNotificationService.Contracts.Models;
using CallNotificationService.Contracts.Requests;
using CallNotificationService.Domain.Interfaces;
using CallNotificationService.Domain.Models;
using CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace CallNotificationService.App;

public class RegistrationFunctions
{
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly IApplicationIdentityService _applicationIdentityService;
    private readonly IRegistrationService _registrationService;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public RegistrationFunctions(
        IApplicationIdentityService applicationIdentityService,
        IRegistrationService registrationService,
        IMapper mapper,
        ILogger<RegistrationFunctions> logger)
    {
        _applicationIdentityService = applicationIdentityService;
        _registrationService = registrationService;
        _mapper = mapper;
        _logger = logger;
    }

    [Function("Register")]
    public async Task<HttpResponseData> Register([HttpTrigger(AuthorizationLevel.Function, "post", Route = "registration")] HttpRequestData data)
    {
        var request = JsonSerializer.Deserialize<CreateRegistrationRequest>(data.Body, _serializerOptions);
        var applicationId = request?.ApplicationId ?? await _applicationIdentityService.GenerateIdentityAsync();
        var registration = new Registration
        {
            Id = applicationId,
            Targets = request.Targets.ToArray(),
            ApplicationId = applicationId,
            CallNotificationUri = request.CallNotificationUri,
            MidCallEventsUri = request.MidCallEventsUri,
            UpdatedOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(request.LifetimeInMinutes),
            LifetimeInMinutes = request.LifetimeInMinutes
        };

        var result = await _registrationService.SetRegistrationAsync(registration);
        var response = _mapper.Map<CallbackRegistration>(result);
        var httpResponse = data.CreateResponse(HttpStatusCode.OK);
        await httpResponse.WriteAsJsonAsync(response);
        return httpResponse;
    }

    [Function("DeRegister")]
    public async Task<HttpResponseData> DeRegister([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "registration/{id}")] HttpRequestData data, string id)
    {
        try
        {
            var result = await _registrationService.RemoveRegistration("ACS", id);
            if (result)
                return data.CreateResponse(HttpStatusCode.OK);

            var response = data.CreateResponse(HttpStatusCode.BadRequest);
            await response.WriteStringAsync($"Unable to de-register {id}");
            return response;
        }
        catch (Exception e)
        {
            var httpResponse = data.CreateResponse(HttpStatusCode.BadRequest);
            return httpResponse;
        }
    }

    [Function("GetRegistration")]
    public async Task<HttpResponseData> GetRegistration([HttpTrigger(AuthorizationLevel.Function, "get", Route = "registration/{id}")] HttpRequestData data, string? id)
    {
        var result = await _registrationService.GetRegistration("ACS", id);
        var response = _mapper.Map<CallbackRegistration>(result);
        if (response is null)
        {
            var notFoundResponse = data.CreateResponse(HttpStatusCode.NotFound);
            return notFoundResponse;
        }

        var httpResponse = data.CreateResponse(HttpStatusCode.OK);
        await httpResponse.WriteAsJsonAsync(response);
        return httpResponse;
    }

    [Function("ListRegistrations")]
    public async Task<HttpResponseData> ListRegistrations([HttpTrigger(AuthorizationLevel.Function, "get", Route = "registration")] HttpRequestData data)
    {
        _logger.LogInformation("Received request to list registrations.");
        var results = await _registrationService.ListRegistrations("ACS");
        List<CallbackRegistration> registrations = results.Select(result => _mapper.Map<CallbackRegistration>(result)).ToList();

        var httpResponse = data.CreateResponse(HttpStatusCode.OK);
        await httpResponse.WriteAsJsonAsync(registrations);
        return httpResponse;
    }
}