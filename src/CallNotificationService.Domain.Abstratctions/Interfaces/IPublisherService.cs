// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Domain.Abstractions.Interfaces;

public interface IPublisherService<in TInput, out TOutput>
{
    Task PublishAsync(TInput data);
}