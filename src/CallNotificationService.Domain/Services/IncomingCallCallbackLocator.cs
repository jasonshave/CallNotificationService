// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallAutomation.Contracts;
using CallNotificationService.Domain.Interfaces;
using CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;

namespace CallNotificationService.Domain.Services;

public class IncomingCallCallbackLocator : ICallbackLocator<IncomingCall>
{
    private readonly IRegistrationService _registrationService;

    public IncomingCallCallbackLocator(IRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    public async Task<IEnumerable<Uri>> LocateCallbacks(IncomingCall input)
    {
        var callbacks = new List<Uri>();
        var registrations = await _registrationService.ListRegistrationsByTarget("ACS", input.To.RawId);
        registrations.ToList().ForEach(x => callbacks.Add(new Uri(x.CallbackUri)));

        return callbacks;
    }
}