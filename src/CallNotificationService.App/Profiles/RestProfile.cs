// Copyright (c) 2022 Jason Shave. All rights reserved.
// Copyright (c) 2022 Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AutoMapper;
using CallNotificationService.Contracts.Models;
using CallNotificationService.Domain.Models;

namespace CallNotificationService.App.Profiles;

public class RestProfile : Profile
{
    public RestProfile()
    {
        CreateMap<Registration, CallbackRegistration>();
    }
}