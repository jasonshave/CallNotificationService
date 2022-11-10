// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallAutomation.Contracts;
using CallNotificationService.Domain.Interfaces;
using CallNotificationService.Domain.Models;
using CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;

namespace CallNotificationService.Domain.Services;

internal sealed class NotificationService : INotificationService
{
    private readonly IRegistrationService _registrationService;
    private readonly ICrudRepository<Notification> _repository;
    private readonly ISender<Notification> _sender;

    public NotificationService(
        IRegistrationService registrationService,
        ICrudRepository<Notification> repository,
        ISender<Notification> sender)
    {
        _registrationService = registrationService;
        _repository = repository;
        _sender = sender;
    }

    public async Task Handle(IncomingCall incomingCall)
    {
        // get notification callbacks
        var registrations = await _registrationService.ListRegistrationsByTarget("ACS", incomingCall.To.RawId);

        // send notifications to each registration matching the "to" field
        foreach (var registration in registrations)
        {
            // convert to domain model
            var notification = new Notification
            {
                Id = Guid.NewGuid().ToString(),
                From = incomingCall.From.RawId,
                To = incomingCall.To.RawId,
                CallerDisplayName = incomingCall.CallerDisplayName,
                IncomingCallContext = incomingCall.IncomingCallContext,
                CorrelationId = incomingCall.CorrelationId,
                ApplicationId = registration.ApplicationId,
                MidCallEventsUri = registration.MidCallEventsUri
            };

            // save notification
            await _repository.Create(notification);

            await _sender.SendAsync(notification, new Uri(registration.CallNotificationUri), new Uri(registration.MidCallEventsUri));
        }
    }

    public async Task<Notification> GetNotification(string resourceId, string id) => await _repository.Get(resourceId, id);
}