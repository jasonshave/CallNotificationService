// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using System.Net;
using CallAutomation.Contracts;
using CallNotificationService.App.Models;
using CallNotificationService.Domain.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Azure;
using Azure.Communication.CallAutomation;
using Microsoft.Azure.Functions.Worker.Http;

namespace CallNotificationService.App
{
    public class IncomingCallFunction
    {
        private readonly INotificationService _notificationService;
        private readonly CallAutomationClient _client;
        private readonly ILogger _logger;

        public IncomingCallFunction(
            ILoggerFactory loggerFactory,
            INotificationService notificationService,
            CallAutomationClient client)
        {
            _notificationService = notificationService;
            _client = client;
            _logger = loggerFactory.CreateLogger<IncomingCallFunction>();
        }

        [Function("IncomingCallTrigger")]
        public async Task Run([EventGridTrigger] EventGridIncomingCall eventGridIncomingCall)
        {
            var incomingCall = new IncomingCall
            {
                From = JsonSerializer.Deserialize<CommunicationId>((JsonElement)eventGridIncomingCall.Data["from"]),
                To = JsonSerializer.Deserialize<CommunicationId>((JsonElement)eventGridIncomingCall.Data["to"]),
                IncomingCallContext = JsonSerializer.Deserialize<string>((JsonElement)eventGridIncomingCall.Data["incomingCallContext"]),
                CallerDisplayName = JsonSerializer.Deserialize<string>((JsonElement)eventGridIncomingCall.Data["callerDisplayName"]),
                CorrelationId = JsonSerializer.Deserialize<string>((JsonElement)eventGridIncomingCall.Data["correlationId"])
            };

            _logger.LogInformation($"Publishing incoming call from {incomingCall.From?.RawId}, to {incomingCall.To?.RawId}. CorrelationId: {incomingCall.CorrelationId}");
            await _notificationService.Handle(incomingCall);
        }

        [Function("AcceptCall")]
        public async Task<HttpResponseData> AcceptCall([HttpTrigger(AuthorizationLevel.Function, "post", Route = "calls/{id}:accept")] HttpRequestData data, string id)
        {
            var notification = await _notificationService.GetNotification("ACS", id);
            try
            {
                AnswerCallResult result = await _client.AnswerCallAsync(
                    new AnswerCallOptions(notification.IncomingCallContext,
                    new Uri(notification.MidCallEventsUri)));
                var httpResponse = data.CreateResponse(HttpStatusCode.OK);
                await httpResponse.WriteAsJsonAsync(result);

                return httpResponse;
            }
            catch (RequestFailedException e)
            {
                var httpResponse = data.CreateResponse(HttpStatusCode.BadRequest);
                await httpResponse.WriteStringAsync(e.Message);
                return httpResponse;
            }
        }
    }
}
