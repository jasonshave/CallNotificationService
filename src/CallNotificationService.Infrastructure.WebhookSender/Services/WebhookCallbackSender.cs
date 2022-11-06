// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace CallNotificationService.Infrastructure.WebhookSender.Services;

public class WebhookCallbackSender : ISender
{
    private readonly CallbackClient _client;
    private readonly ILogger<WebhookCallbackSender> _logger;

    public WebhookCallbackSender(CallbackClient client, ILogger<WebhookCallbackSender> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task SendAsync<T>(T payload, Uri callbackUri)
    {
        _logger.LogInformation($"Sending payload {typeof(T).Name} to {callbackUri}");
        await _client.Client.PostAsJsonAsync(callbackUri, payload);
    }
}