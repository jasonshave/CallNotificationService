﻿// Copyright (c) 2022 Jason Shave. All rights reserved.
// Copyright (c) 2022 Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Infrastructure.TokenService;

public sealed class TokenConfiguration
{
    public bool Enabled { get; init; }

    public string Secret { get; init; }

    public int TimeToLiveInMinutes { get; init; }

    public string Issuer { get; init; }
}