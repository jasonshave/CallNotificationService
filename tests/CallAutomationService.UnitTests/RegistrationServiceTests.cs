// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using AutoFixture;
using CallNotificationService.Domain.Models;
using CallNotificationService.Domain.Services;
using CallNotificationService.Persistence;
using FluentAssertions;
using Moq;

namespace CallNotificationService.UnitTests
{
    public class RegistrationServiceTests
    {
        [Fact]
        public void Registration_Should_Create()
        {
            // arrange
            var fixture = new Fixture();
            var registration = fixture.Create<CallbackRegistration>();
            var mockRepository = new Mock<IRepository<CallbackRegistration>>();
            mockRepository.Setup(x => x.Save(It.IsAny<CallbackRegistration>())

            var registrationService = new RegistrationService(mockRepository.Object);

            // act
            var result = registrationService.Register(registration);

            // assert
            result.Should().NotBeNull();
        }
    }
}