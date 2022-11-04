// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallNotificationService.Domain.Abstractions.Interfaces;

namespace CallNotificationService.Domain.Abstractions
{
    public class CallbackPublisher<TInput, TOutput> : IPublisherService<TInput, TOutput>
    {
        private readonly ISender _sender;
        private readonly IEventConverter<TInput, TOutput> _converter;
        private readonly ICallbackLocator<TInput> _callbackLocator;

        public CallbackPublisher(
            ISender sender,
            IEventConverter<TInput, TOutput> converter,
            ICallbackLocator<TInput> callbackLocator)
        {
            _sender = sender;
            _converter = converter;
            _callbackLocator = callbackLocator;
        }

        public async Task PublishAsync(TInput data)
        {
            var uris = await _callbackLocator.LocateCallbacks(data);
            var result = _converter.Convert(data);

            foreach (var uri in uris)
            {
                await _sender.SendAsync(result, uri);
            }
        }
    }
}
