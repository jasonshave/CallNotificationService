// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallNotificationService.Domain.Abstractions.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace CallNotificationService.Infrastructure.WebhookSender.Services;

public class WebhookCallbackSender : ISender
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<WebhookCallbackSender> _logger;

    public WebhookCallbackSender(IHttpClientFactory httpClientFactory, ILogger<WebhookCallbackSender> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task SendAsync<T>(T payload, Uri callbackUri)
    {
        var httpClient = _httpClientFactory.CreateClient();

        _logger.LogInformation($"Sending payload {typeof(T).Name} to {callbackUri}");
        await httpClient.PostAsJsonAsync(callbackUri, payload);
    }
}