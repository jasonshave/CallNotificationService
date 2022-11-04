// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Domain.Abstractions.Interfaces
{
    public interface ISender
    {
        Task SendAsync<T>(T payload, Uri callbackUri);
    }
}
