// Copyright (c) 2022 Jason Shave. All rights reserved.
// Copyright (c) 2022 Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces
{
    public interface ISender<in TInput>
    {
        Task SendAsync(TInput payload, Uri callbackUri, Uri midCallEventsUri);
    }
}
