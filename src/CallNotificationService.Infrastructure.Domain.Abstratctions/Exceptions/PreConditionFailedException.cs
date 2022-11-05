// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Infrastructure.Domain.Abstractions.Exceptions;

public class PreconditionFailedException : Exception
{
    public PreconditionFailedException()
    {
    }

    public PreconditionFailedException(string? message)
        : base(message)
    {
    }

    public PreconditionFailedException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}