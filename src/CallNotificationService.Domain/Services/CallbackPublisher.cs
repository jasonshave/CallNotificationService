// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallNotificationService.Domain.Abstratctions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallNotificationService.Domain.Services
{
    internal class CallbackPublisher
    {
        private readonly ISender _sender;

        public CallbackPublisher(ISender sender)
        {
            _sender = sender;
        }

        public async Task PublishAsync<T>(T payload)
        {

            await _sender.SendAsync(payload);
        }
    }
}
