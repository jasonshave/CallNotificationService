// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Infrastructure.TokenService;

public class TokenConfiguration
{
    public string Secret { get; init; }

    public int TimeToLiveInMinutes { get; init; }

    public string Issuer { get; init; }
}