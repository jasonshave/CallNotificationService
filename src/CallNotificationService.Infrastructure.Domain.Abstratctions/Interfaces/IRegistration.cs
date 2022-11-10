// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;

public interface IRegistration
{
    string ApplicationId { get; }

    string[] Targets { get; }

    string CallNotificationUri { get; }

    string MidCallEventsUri { get; }
}