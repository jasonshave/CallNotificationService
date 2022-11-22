// Copyright (c) 2022 Jason Shave. All rights reserved.
// Copyright (c) 2022 Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using CallNotificationService.Domain.Interfaces;
using CallNotificationService.Domain.Models;
using CallNotificationService.Domain.Services;
using CallNotificationService.Infrastructure.Domain.Abstractions.Interfaces;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace CallNotificationService.UnitTests;

public class DomainTests
{
    [Fact]
    public void NotificationService_Can_Handle_IncomingCall()
    {
        // arrange
        var mockOptionsMonitor = new Mock<IOptionsMonitor<NotificationSettings>>();
        var mockRegistrationService = new Mock<IRegistrationService>();
        var mockCrudRepository = new Mock<ICrudRepository<Notification>>();
        var mockSender = new Mock<ISender<Notification>>();
        var subject = new NotificationService(
            mockOptionsMonitor.Object,
            mockRegistrationService.Object,
            mockCrudRepository.Object,
            mockSender.Object);

        // act


        // assert
    }
}