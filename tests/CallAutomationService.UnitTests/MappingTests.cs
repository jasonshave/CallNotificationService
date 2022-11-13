// Copyright (c) 2022 Jason Shave. All rights reserved.
// Copyright (c) 2022 Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AutoFixture;
using AutoMapper;
using CallNotificationService.Domain.Models;
using CallNotificationService.Persistence.Models;
using CallNotificationService.Persistence.Profiles;
using FluentAssertions;
using Xunit;

namespace CallNotificationService.UnitTests;

public class MappingTests
{
    [Fact]
    public void CallbackRegistration_ShouldMapTo_PersistedCallbackRegistration()
    {
        // arrange
        var fixture = new Fixture();
        var callbackRegistration = fixture.Create<Registration>();
        var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile(typeof(PersistenceProfile)));
        var mapper = mapperConfiguration.CreateMapper();

        // act
        var persistedRegistration = mapper.Map<PersistedCallbackRegistration>(callbackRegistration);

        // assert
        persistedRegistration.ApplicationId.Should().BeSameAs(callbackRegistration.ApplicationId);
        persistedRegistration.CallNotificationUri.Should().BeSameAs(callbackRegistration.CallNotificationUri);
    }

    [Fact]
    public void PersistedCallbackRegistration_ShouldMapTo_CallbackRegistration()
    {
        // arrange
        var fixture = new Fixture();
        var callbackRegistration = fixture.Create<PersistedCallbackRegistration>();
        var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile(typeof(PersistenceProfile)));
        var mapper = mapperConfiguration.CreateMapper();

        // act
        var persistedRegistration = mapper.Map<Registration>(callbackRegistration);

        // assert
        persistedRegistration.ApplicationId.Should().BeSameAs(callbackRegistration.ApplicationId);
        persistedRegistration.CallNotificationUri.Should().BeSameAs(callbackRegistration.CallNotificationUri);
    }
}