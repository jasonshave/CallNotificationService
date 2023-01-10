// Copyright (c) 2022 Jason Shave. All rights reserved.
// Copyright (c) 2022 Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace CallNotificationService.Infrastructure.TokenService
{
    public sealed class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> _logger;
        private readonly TokenConfiguration _tokenConfiguration;

        public TokenService(
            IOptions<TokenConfiguration> tokenConfiguration,
            ILogger<TokenService> logger)
        {
            _logger = logger;
            _tokenConfiguration = tokenConfiguration.Value;
        }

        public string? GenerateToken(string applicationId)
        {
            if (!_tokenConfiguration.Enabled) return null;

            _logger.LogInformation("Generating token for {applicationId}", applicationId);
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