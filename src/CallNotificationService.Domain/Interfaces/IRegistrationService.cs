// Copyright (c) 2022 Jason Shave. All rights reserved.
// Copyright (c) 2022 Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using CallNotificationService.Domain.Models;

namespace CallNotificationService.Domain.Interfaces;

public interface IRegistrationService
{
    Task<Registration?> GetRegistration(string resourceId, string id);

    Task<Registration> SetRegistrationAsync(Registration registration);

    Task<bool> RemoveRegistration(string resourceId, string Id);

    Task<IEnumerable<Registration>> ListRegistrationsByTarget(string resourceId, string target);

    Task<IEnumerable<Registration>> ListRegistrationsByApplicationId(string resourceId, string applicationId);

    Task<IEnumerable<Registration>> ListRegistrations(string resourceId);
}