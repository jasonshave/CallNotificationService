// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallAutomation.Contracts;
using CallNotificationService.Domain.Models;

namespace CallNotificationService.Domain.Interfaces;

public interface INotificationService
{
    Task Handle(IncomingCall incomingCall);

    Task<Notification> GetNotification(string resourceId, string id);
}