// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Infrastructure.Domain.Abstractions.Exceptions;

public class ConflictException : Exception
{
    public const string Code = "Conflict";

    public ConflictException()
    {
    }

    public ConflictException(string? message)
        : base(message)
    {
    }

    public ConflictException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}