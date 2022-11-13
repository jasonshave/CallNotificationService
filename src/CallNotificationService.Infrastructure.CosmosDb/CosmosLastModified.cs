// Copyright (c) 2022 Jason Shave. All rights reserved.
// Copyright (c) 2022 Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Infrastructure.CosmosDb;

#pragma warning disable SA1300 // Element should begin with upper-case letter
internal record CosmosLastModified
{
    public long _ts { get; set; }
}
#pragma warning restore SA1300 // Element should begin with upper-case letter