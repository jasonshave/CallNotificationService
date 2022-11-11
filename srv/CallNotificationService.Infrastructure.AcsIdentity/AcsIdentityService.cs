// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;
using Azure.Communication.Identity;
using CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;

namespace CallNotificationService.Infrastructure.AcsIdentity
{
    public sealed class AcsIdentityService : IApplicationIdentityService
    {
        private readonly CommunicationIdentityClient _identityClient;

        public AcsIdentityService(CommunicationIdentityClient identityClient)
        {
            _identityClient = identityClient;
        }

        public async Task<string> GenerateIdentityAsync()
        {
            CommunicationUserIdentifier identity = await _identityClient.CreateUserAsync();
            return identity.RawId;
        }
    }
}