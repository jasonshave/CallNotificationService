// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallNotificationService.Client
{
    internal sealed class CallNotificationClient : ICallNotificationClient
    {
        private readonly CallNotificationHttpClient _client;
        private readonly CallNotificationClientSettings _callNotificationClientSettings;

        public CallNotificationClient(
            CallNotificationHttpClient client,
            CallNotificationClientSettings callNotificationClientSettings)
        {
            _client = client;
            _callNotificationClientSettings = callNotificationClientSettings;
        }

        /// <summary>
        /// Creates or updates/refreshes an existing registration using the parameters specified in <see cref="CreateRegistration"/>
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<CallbackRegistration?> SetRegistration(Action<CreateRegistration> options)
        {
            var request = new CreateRegistration();
            options(request);

            return await _client.Post<CreateRegistration, CallbackRegistration>(request, new Uri(_callNotificationClientSettings.SetRegistrationEndpointUri));
        }

        /// <summary>
        /// Removes the registration for the application ID specified.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public async Task<bool> DeRegister(string applicationId)
        {
            var uri = string.Format(_callNotificationClientSettings.DeRegisterEndpointUri, applicationId);
            return await _client.Delete(new Uri(uri));
        }

        /// <summary>
        /// Gets a specific registration information.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<CallbackRegistration?> GetRegistration(string applicationId)
        {
            var uri = string.Format(_callNotificationClientSettings.GetRegistrationEndpointUri, applicationId);
            return await _client.Get<CallbackRegistration>(
                new Uri(uri));
        }

        /// <summary>
        /// Lists all registrations.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<IEnumerable<CallbackRegistration>> ListRegistrations()
        {
            var result = await _client.Get<IEnumerable<CallbackRegistration>>(new Uri(_callNotificationClientSettings
                .ListRegistrationsEndpointUri));

            return result ?? Enumerable.Empty<CallbackRegistration>();
        }
    }
}