// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Infrastructure.WebhookSender.Services;

public sealed class CallbackClient
{
    public HttpClient Client { get; }

    public CallbackClient(HttpClient client)
    {
        Client = client;
    }
}