// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using System.Net.Http.Json;
using CallNotificationService.Domain.Abstractions.Interfaces;

namespace CallNotificationService.Domain.Services;

public class WebhookCallbackSender : ISender
{
    private readonly IHttpClientFactory _httpClientFactory;

    public WebhookCallbackSender(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task SendAsync<T>(T payload, Uri callbackUri)
    {
        var httpClient = _httpClientFactory.CreateClient();

        await httpClient.PostAsJsonAsync(callbackUri, payload);
    }
}