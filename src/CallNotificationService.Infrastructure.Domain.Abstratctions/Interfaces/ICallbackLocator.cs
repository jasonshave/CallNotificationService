// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;

public interface ICallbackLocator<in TInput, TOutput>
{
    Task<IEnumerable<TOutput>> GetCallbacksByTarget(TInput input);
}