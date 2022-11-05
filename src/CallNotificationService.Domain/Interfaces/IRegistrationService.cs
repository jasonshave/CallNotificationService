// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallNotificationService.Domain.Models;

namespace CallNotificationService.Domain.Interfaces;

public interface IRegistrationService
{
    Task<CallbackRegistration?> GetRegistration(string resourceId, string id);

    Task<CallbackRegistration> SetRegistrationAsync(CallbackRegistration registration);

    //Task<IEnumerable<CallbackRegistration>> ListRegistrationsByCallbackUri(string callbackUri);

    Task<IEnumerable<CallbackRegistration>> ListRegistrationsByTarget(string resourceId, string target);

    //Task<IEnumerable<CallbackRegistration>> ListRegistrationsByApplicationId(string applicationId);
}