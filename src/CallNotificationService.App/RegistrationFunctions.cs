// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using AutoMapper;
using CallNotificationService.Contracts.Models;
using CallNotificationService.Contracts.Requests;
using CallNotificationService.Domain.Interfaces;
using CallNotificationService.Domain.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace CallNotificationService.App;

public class RegistrationFunctions
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly IRegistrationService _service;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public RegistrationFunctions(ILoggerFactory loggerFactory, IRegistrationService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
        _logger = loggerFactory.CreateLogger<RegistrationFunctions>();
    }

    [Function("Register")]
    public async Task<HttpResponseData> Register([HttpTrigger(AuthorizationLevel.Function, "post", Route = "registration")] HttpRequestData data)
    {
        var request = JsonSerializer.Deserialize<CreateRegistrationRequest>(data.Body, _serializerOptions);
        var registration = new CallbackRegistration
        {
            ApplicationId = request.ApplicationId,
            CallbackUri = request.CallbackUri.ToString(),
            UpdatedOn = DateTimeOffset.UtcNow,
            Id = Guid.NewGuid().ToString(),
            Targets = request.Targets,
        };

        var result = await _service.SetRegistrationAsync(registration);
        var response = _mapper.Map<CallbackRegistrationDto>(result);
        var httpResponse = data.CreateResponse(HttpStatusCode.OK);
        await httpResponse.WriteAsJsonAsync(response);
        return httpResponse;
    }

    [Function("GetRegistration")]
    public async Task<HttpResponseData> GetRegistration([HttpTrigger(AuthorizationLevel.Function, "get", Route = "registration/{id}")] HttpRequestData data, string? id)
    {
        var result = await _service.GetRegistration("ACS", id);
        var response = _mapper.Map<CallbackRegistrationDto>(result);
        if (response is null)
        {
            var notFoundResponse = data.CreateResponse(HttpStatusCode.NotFound);
            return notFoundResponse;
        }

        var httpResponse = data.CreateResponse(HttpStatusCode.OK);
        await httpResponse.WriteAsJsonAsync(response);
        return httpResponse;
    }

    //[Function("ListRegistrationsByApplicationId")]
    //public async Task<HttpResponseData> ListRegistrationsByApplicationId([HttpTrigger(AuthorizationLevel.Function, "get", Route = "registration/application/{applicationId}")] HttpRequestData data, string? applicationId)
    //{
    //    List<CallbackRegistrationDto> registrations = new();
    //    var results = await _service.ListRegistrationsByApplicationId(applicationId);
    //    foreach (var result in results)
    //    {
    //        var response = _mapper.Map<CallbackRegistrationDto>(result);
    //        registrations.Add(response);
    //    }

    //    var httpResponse = data.CreateResponse(HttpStatusCode.OK);
    //    await httpResponse.WriteAsJsonAsync(registrations);
    //    return httpResponse;
    //}
}