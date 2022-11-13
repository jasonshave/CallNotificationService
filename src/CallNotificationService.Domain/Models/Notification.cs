// Copyright (c) 2022 Jason Shave. All rights reserved.
// Copyright (c) 2022 Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;

namespace CallNotificationService.Domain.Models;

public sealed class Notification : IEntity
{
    public string Id { get; init; }

    public string ResourceId => "ACS";

    public string From { get; init; }

    public string To { get; init; }

    public string CallerDisplayName { get; init; }

    public string ApplicationId { get; init; }

    public string CorrelationId { get; init; }

    public string IncomingCallContext { get; init; }

    // This is important to have as it ensures customers don't need to save their registration (state).
    // The mid-call events callback URI is configured by the customer so when an incoming call arrives,
    // we look up the registration to find the notification callback URI and save the mid-call events
    // callback URI so the customer can simply answer the call without having to remember which system
    // should process those types of events.
    public string MidCallEventsUri { get; init; }
}