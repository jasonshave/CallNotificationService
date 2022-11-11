// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallNotificationService.Contracts.Models;
using CallNotificationService.Contracts.Requests;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace CallNotificationService.Client
{
    public sealed class CallNotificationClient
    {
        private readonly CallNotificationClientSettings _callNotificationClientSettings;

        public CallNotificationClient(Action<CallNotificationClientSettings> options)
        {
            var settings = new CallNotificationClientSettings();
            options(settings);
            _callNotificationClientSettings = settings;
        }

        public CallNotificationClient(CallNotificationClientSettings settings)
        {
            _callNotificationClientSettings = settings;
        }

        public CallNotificationClient(IConfiguration configuration)
        {
            var settings = new CallNotificationClientSettings();
            configuration.Bind(nameof(CallNotificationClientSettings), settings);
            _callNotificationClientSettings = settings;
        }

        /// <summary>
        /// Creates or updates/refreshes an existing registration using the parameters specified in <see cref="CreateRegistrationRequest"/>
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<CallbackRegistration?> SetRegistration(Action<CreateRegistrationRequest> options)
        {
            var request = new CreateRegistrationRequest();
            options(request);

            using var httpClient = new HttpClient();
            var httpRequest = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_callNotificationClientSettings.SetRegistrationEndpointUri)
            };
            httpRequest.Content = new StringContent(JsonSerializer.Serialize(request));
            var response = await httpClient.SendAsync(httpRequest);
            if (!response.IsSuccessStatusCode) throw new ApplicationException(response.ReasonPhrase);

            var stream = await response.Content.ReadAsStreamAsync();
            var registration = await JsonSerializer.DeserializeAsync<CallbackRegistration>(stream);
            return registration;
        }

        /// <summary>
        /// Removes the registration for the application ID specified.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public async Task<bool> DeRegister(string applicationId)
        {
            using var httpClient = new HttpClient();
            var httpRequest = new HttpRequestMessage()
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(_callNotificationClientSettings.DeRegisterEndpointUri)
            };
            var response = await httpClient.SendAsync(httpRequest);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Gets a specific registration information.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<CallbackRegistration?> GetRegistration(string applicationId)
        {
            using var httpClient = new HttpClient();
            var httpRequest = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_callNotificationClientSettings.GetRegistrationEndpointUri)
            };
            var response = await httpClient.SendAsync(httpRequest);
            if (!response.IsSuccessStatusCode) throw new ApplicationException(response.ReasonPhrase);

            var stream = await response.Content.ReadAsStreamAsync();
            var registration = await JsonSerializer.DeserializeAsync<CallbackRegistration>(stream);
            return registration;
        }

        /// <summary>
        /// Lists all registrations.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<IEnumerable<CallbackRegistration>> ListRegistrations()
        {
            using var httpClient = new HttpClient();
            var httpRequest = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_callNotificationClientSettings.ListRegistrationsEndpointUri)
            };
            var response = await httpClient.SendAsync(httpRequest);
            if (!response.IsSuccessStatusCode) throw new ApplicationException(response.ReasonPhrase);

            var stream = await response.Content.ReadAsStreamAsync();
            var registrations = await JsonSerializer.DeserializeAsync<IEnumerable<CallbackRegistration>>(stream);
            return registrations ?? Enumerable.Empty<CallbackRegistration>();
        }
    }
}