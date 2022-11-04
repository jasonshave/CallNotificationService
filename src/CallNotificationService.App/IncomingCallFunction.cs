// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using CallAutomation.Contracts;
using CallNotificationService.Contracts.Models;
using CallNotificationService.Domain.Abstractions.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CallNotificationService.App
{
    public class IncomingCallFunction
    {
        private readonly IPublisherService<IncomingCall, CallNotification> _publisher;
        private readonly ILogger _logger;

        public IncomingCallFunction(
            ILoggerFactory loggerFactory,
            IPublisherService<IncomingCall, CallNotification> publisher)
        {
            _publisher = publisher;
            _logger = loggerFactory.CreateLogger<IncomingCallFunction>();
        }

        [Function("IncomingCallTrigger")]
        public async Task Run([EventGridTrigger] EventGridIncomingCall eventGridIncomingCall)
        {
            var incomingCall = new IncomingCall();

            incomingCall.From = JsonSerializer.Deserialize<CommunicationId>((JsonElement)eventGridIncomingCall.Data["from"]);
            incomingCall.To = JsonSerializer.Deserialize<CommunicationId>((JsonElement)eventGridIncomingCall.Data["to"]);
            incomingCall.IncomingCallContext = JsonSerializer.Deserialize<string>((JsonElement)eventGridIncomingCall.Data["incomingCallContext"]);
            incomingCall.CallerDisplayName = JsonSerializer.Deserialize<string>((JsonElement)eventGridIncomingCall.Data["callerDisplayName"]);
            incomingCall.CorrelationId = JsonSerializer.Deserialize<string>((JsonElement)eventGridIncomingCall.Data["correlationId"]);

            _logger.LogInformation($"Publishing incoming call from {incomingCall.From.RawId}, to {incomingCall.To.RawId}. CorrelationId: {incomingCall.CorrelationId}");
            await _publisher.PublishAsync(incomingCall);
        }
    }
}
