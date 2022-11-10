// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using System.Net.Http.Headers;
using CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using CallNotificationService.Contracts.Models;
using CallNotificationService.Domain.Models;
using CallNotificationService.Infrastructure.TokenService;

namespace CallNotificationService.Infrastructure.WebhookSender.Services;

public class WebhookCallbackSender : ISender<Notification>
{
    private readonly ITokenService _tokenService;
    private readonly CallbackClient _callbackClient;
    private readonly ILogger<WebhookCallbackSender> _logger;

    public WebhookCallbackSender(
        ITokenService tokenService,
        CallbackClient callbackClient,
        ILogger<WebhookCallbackSender> logger)
    {
        _tokenService = tokenService;
        _callbackClient = callbackClient;
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
            ApplicationId = notification.ApplicationId,
            CorrelationId = notification.CorrelationId,
            MidCallEventsUri = midCallEventsUri.ToString()
        };

        var token = _tokenService.GenerateToken(notification.ApplicationId);
        _callbackClient.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        await _callbackClient.HttpClient.PostAsJsonAsync(callbackUri, payload);
    }
}