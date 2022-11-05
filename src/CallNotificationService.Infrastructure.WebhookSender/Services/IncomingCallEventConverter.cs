// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallAutomation.Contracts;
using CallNotificationService.Contracts.Models;
using CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;

namespace CallNotificationService.Infrastructure.WebhookSender.Services;

public class IncomingCallEventConverter : IEventConverter<IncomingCall, CallNotification>
{
    public CallNotification Convert(IncomingCall input)
    {
        return new CallNotification(Guid.NewGuid().ToString(), input.From.RawId, input.To.RawId,
            input.CallerDisplayName, input.CorrelationId);
    }
}