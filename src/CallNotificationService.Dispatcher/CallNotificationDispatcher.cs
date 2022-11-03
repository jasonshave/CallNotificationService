// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Hosting;

namespace CallNotificationService.Dispatcher
{
    public class CallNotificationDispatcher : BackgroundService
    {
        public CallNotificationDispatcher()
        {
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}