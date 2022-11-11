// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json;

namespace CallNotificationService.Client;

public class CallNotificationHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _serializerOptions = new();

    public CallNotificationHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Sends data and deserializes stream.
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    /// <param name="input"></param>
    /// <param name="endpointUri"></param>
    /// <returns></returns>
    /// <exception cref="ApplicationException"></exception>
    public async Task<TOutput?> Post<TInput, TOutput>(TInput input, Uri endpointUri)
        where TInput : class
    {
        var httpRequest = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = endpointUri
        };
        httpRequest.Content = new StringContent(JsonSerializer.Serialize(input));
        var response = await _httpClient.SendAsync(httpRequest);
        if (!response.IsSuccessStatusCode) throw new ApplicationException(response.ReasonPhrase);

        var stream = await response.Content.ReadAsStreamAsync();
        var output = await JsonSerializer.DeserializeAsync<TOutput>(stream, _serializerOptions);
        return output;
    }

    /// <summary>
    /// Deletes a resource based on the endpoint URI.
    /// </summary>
    /// <param name="endpointUri"></param>
    /// <returns></returns>
    public async Task<bool> Delete(Uri endpointUri)
    {
        var httpRequest = new HttpRequestMessage()
        {
            Method = HttpMethod.Delete,
            RequestUri = endpointUri
        };
        var response = await _httpClient.SendAsync(httpRequest);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Handles get endpoint with return deserialization.
    /// </summary>
    /// <typeparam name="TOutput"></typeparam>
    /// <param name="endpointUri"></param>
    /// <returns></returns>
    /// <exception cref="ApplicationException"></exception>
    public async Task<TOutput?> Get<TOutput>(Uri endpointUri)
    {
        var httpRequest = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = endpointUri
        };
        var response = await _httpClient.SendAsync(httpRequest);
        if (!response.IsSuccessStatusCode) throw new ApplicationException(response.ReasonPhrase);

        var stream = await response.Content.ReadAsStreamAsync();
        var output = await JsonSerializer.DeserializeAsync<TOutput>(stream, _serializerOptions);
        return output;
    }
}