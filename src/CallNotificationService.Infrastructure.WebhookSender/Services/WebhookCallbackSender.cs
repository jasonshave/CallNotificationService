// Copyright (c) 2022 Jason Shave. All rights reserved.
// Copyright (c) 2022 Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using CallAutomation.Contracts;
using CallNotificationService.Domain.Models;
using CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CallNotificationService.Infrastructure.WebhookSender.Services;

internal sealed class WebhookCallbackSender : ISender<Notification>
{
    private readonly IOptionsMonitor<NotificationSettings> _notificationSettings;
    private readonly ITokenService _tokenService;
    private readonly CallbackClient _callbackClient;
    private readonly ILogger<WebhookCallbackSender> _logger;

    public WebhookCallbackSender(
        IOptionsMonitor<NotificationSettings> notificationSettings,
        ITokenService tokenService,
        CallbackClient callbackClient,
        ILogger<WebhookCallbackSender> logger)
    {
        _notificationSettings = notificationSettings;
        _tokenService = tokenService;
        _callbackClient = callbackClient;
        _logger = logger;
    }

    public async Task SendAsync(Notification notification, Uri callbackUri, Uri midCallEventsUri)
    {
        var callNotification = new CallNotification
        {
            Id = notification.Id,
            To = notification.To,
            From = notification.From,
            CallerDisplayName = notification.CallerDisplayName,
            ApplicationId = notification.ApplicationId,
            CorrelationId = notification.CorrelationId,
            MidCallEventsUri = midCallEventsUri.ToString()
        };

        if (_notificationSettings.CurrentValue.EnableSendIncomingCallContext)
            callNotification.IncomingCallContext = notification.IncomingCallContext;

        var token = _tokenService.GenerateToken(notification.ApplicationId);
        if (token is not null)
            _callbackClient.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        _logger.LogInformation("Sending notification payload to {callbackUri}", callbackUri);
        await _callbackClient.HttpClient.PostAsJsonAsync(callbackUri, callNotification);
    }
}