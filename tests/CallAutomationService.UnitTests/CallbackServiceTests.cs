// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using AutoFixture;
using CallAutomation.Contracts;
using CallNotificationService.Domain.Abstractions.Interfaces;
using CallNotificationService.Domain.Models;
using FluentAssertions;
using Moq;

namespace CallNotificationService.UnitTests
{
    internal class CallbackServiceTests
    {
        [Fact]
        public async Task Callback_Sends()
        {
            // arrange
            var fixture = new Fixture();
            var incomingCall = fixture.Create<IncomingCall>();

            var callNotification = new CallNotification(
                Guid.NewGuid().ToString(),
                incomingCall.From.RawId,
                incomingCall.To.RawId,
                incomingCall.CallerDisplayName,
                null,
                incomingCall.CorrelationId,
                incomingCall.ServerCallId);

            var mockSender = new Mock<ISender>();
            mockSender.Setup(x => x.SendAsync(It.IsAny<CallNotification>())).Returns(Task.FromResult(callNotification));

            var publisher = new CallbackPublisher(mockSender.Object);

            // act and assert
            await publisher.Invoking(x => x.PublishAsync<CallNotification>(It.IsAny<CallNotification>()))
                .Should().NotThrowAsync();

        }
    }
}
