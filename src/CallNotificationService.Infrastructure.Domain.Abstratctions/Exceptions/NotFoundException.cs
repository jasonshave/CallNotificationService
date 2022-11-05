// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Infrastructure.Domain.Abstractions.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException()
    {
    }

    public NotFoundException(string? message)
        : base(message)
    {
    }

    public NotFoundException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}