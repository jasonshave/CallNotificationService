// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CallNotificationService.Client;

public sealed class CallbackRegistrationSettings
{
    public string? ApplicationId { get; set; }

    public string CallbackHost { get; set; } = string.Empty;

    public double LifetimeInMinutes { get; set; }

    public List<string> Targets { get; set; } = new();

    public string CallNotificationPath { get; set; } = string.Empty;

    public string MidCallEventsPath { get; set; } = string.Empty;
}