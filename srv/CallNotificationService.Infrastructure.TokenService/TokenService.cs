// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using JWT.Algorithms;
using JWT.Builder;
using System.Text;
using CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;
using Microsoft.Extensions.Options;

namespace CallNotificationService.Infrastructure.TokenService
{
    public class TokenService : ITokenService
    {
        private readonly TokenConfiguration _tokenConfiguration;

        public TokenService(IOptions<TokenConfiguration> tokenConfiguration)
        {
            _tokenConfiguration = tokenConfiguration.Value;
        }

        public string GenerateToken(string applicationId)
        {
            return new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(Encoding.ASCII.GetBytes(_tokenConfiguration.Secret))
                .AddClaim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                .AddClaim("nbf", DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                .AddClaim("exp", DateTimeOffset.UtcNow.AddMinutes(_tokenConfiguration.TimeToLiveInMinutes).ToUnixTimeSeconds())
                .AddClaim("aud", applicationId)
                .AddClaim("iss", _tokenConfiguration.Issuer)
                .Encode();
        }
    }
}