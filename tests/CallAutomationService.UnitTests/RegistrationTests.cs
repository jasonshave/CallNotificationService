// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using AutoFixture;
using CallNotificationService.Domain.Models;
using CallNotificationService.Domain.Services;
using FluentAssertions;

namespace CallNotificationService.UnitTests
{
    public class RegistrationTests
    {
        [Fact]
        public void Registration_Should_Create()
        {
            // arrange
            var fixture = new Fixture();
            var registration = fixture.Create<CallbackRegistration>();
            var registrationService = new RegistrationService();

            // act
            var result = registrationService.Register(registration);

            // assert
            result.Should().NotBeNull();
        }
    }
}