// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using AutoMapper;
using CallNotificationService.Domain.Models;
using CallNotificationService.Persistence.Models;

namespace CallNotificationService.Persistence.Profiles;

public class TableStorageProfile : Profile
{
    public TableStorageProfile()
    {
        CreateMap<CallbackRegistration, PersistedCallbackRegistration>().ReverseMap();
    }
}