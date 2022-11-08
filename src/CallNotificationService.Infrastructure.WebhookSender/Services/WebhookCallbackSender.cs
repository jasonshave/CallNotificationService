// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using CallNotificationService.Contracts.Models;
using CallNotificationService.Domain.Models;

namespace CallNotificationService.Infrastructure.WebhookSender.Services;

public class WebhookCallbackSender : ISender<Notification>
{
    private readonly CallbackClient _client;
    private readonly ILogger<WebhookCallbackSender> _logger;

    public WebhookCallbackSender(CallbackClient client, ILogger<WebhookCallbackSender> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task SendAsync(Notification notification, Uri callbackUri, Uri midCallEventsUri)
    {
        var payload = new CallNotification()
        {
            Id = notification.Id,
            To = notification.To,
            From = notification.From,
            CallerDisplayName = notification.CallerDisplayName,
            CorrelationId = notification.CorrelationId,
            MidCallEventsUri = midCallEventsUri.ToString()
        };
        await _client.Client.PostAsJsonAsync(callbackUri, payload);
    }
}