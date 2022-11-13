// Copyright (c) 2022 Jason Shave. All rights reserved.
// Copyright (c) 2022 Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Logging;

namespace CallNotificationService.Client
{
    internal sealed class CallNotificationClient : ICallNotificationClient
    {
        private readonly CallNotificationHttpClient _client;
        private readonly CallNotificationClientSettings _callNotificationClientSettings;
        private readonly ILogger<CallNotificationClient> _logger;

        public CallNotificationClient(
            CallNotificationHttpClient client,
            CallNotificationClientSettings callNotificationClientSettings,
            ILogger<CallNotificationClient> logger)
        {
            _client = client;
            _callNotificationClientSettings = callNotificationClientSettings;
            _logger = logger;
        }

        /// <summary>
        /// Creates or updates/refreshes an existing registration using the parameters specified in <see cref="CallbackRegistrationSettings"/>
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<CallbackRegistration?> SetRegistrationAsync(Action<CallbackRegistrationSettings> options)
        {
            var request = new CallbackRegistrationSettings();
            options(request);

            return await SetRegistrationAsync(request);
        }

        /// <summary>
        /// Creates or updates/refreshes an existing registration using the parameters specified in <see cref="CallbackRegistrationSettings"/>
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<CallbackRegistration?> SetRegistrationAsync(CallbackRegistrationSettings request)
        {
            var createRegistrationRequest = new CreateRegistrationRequest()
            {
                ApplicationId = request.ApplicationId,
                CallNotificationUri = request.CallbackHost + request.CallNotificationPath,
                MidCallEventsUri = request.CallbackHost + request.MidCallEventsPath,
                LifetimeInMinutes = request.LifetimeInMinutes,
                Targets = request.Targets
            };

            var registration = await _client.Post<CreateRegistrationRequest, CallbackRegistration>(createRegistrationRequest, new Uri(_callNotificationClientSettings.SetRegistrationEndpointUri));

            _logger.LogInformation($"Registered WebHook callback {registration.CallNotificationUri} using application {registration.ApplicationId} which expires on {registration.ExpiresOn.ToLocalTime()}.");

            return registration;
        }

        /// <summary>
        /// Removes the registration for the application ID specified.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public async Task<bool> DeRegister(string applicationId)
        {
            var uri = string.Format(_callNotificationClientSettings.DeRegisterEndpointUri, applicationId);
            var isSuccess = await _client.Delete(new Uri(uri));
            if (isSuccess)
            {
                _logger.LogInformation($"Removed WebHook callback registration for application {applicationId}.");
            }
            else
            {
                _logger.LogError($"There was a problem removing the WebHook callback registration for application {applicationId}.");
            }

            return isSuccess;
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