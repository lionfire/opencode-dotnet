using System.Net;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using LionFire.OpenCode.Serve.Exceptions;
using LionFire.OpenCode.Serve.Internal;

namespace LionFire.OpenCode.Serve.Extensions;

/// <summary>
/// Extension methods for <see cref="HttpClient"/> to simplify API operations.
/// </summary>
internal static class HttpClientExtensions
{
    /// <summary>
    /// Sends a GET request and deserializes the response.
    /// </summary>
    public static async Task<T> GetJsonAsync<T>(
        this HttpClient client,
        string requestUri,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await client.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
            await EnsureSuccessAsync(response, cancellationToken).ConfigureAwait(false);
            return await DeserializeResponseAsync<T>(response, cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            throw OpenCodeConnectionException.ServerNotReachable(client.BaseAddress?.ToString() ?? "unknown", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw OpenCodeTimeoutException.OperationTimedOut("GET " + requestUri, client.Timeout, ex);
        }
    }

    /// <summary>
    /// Sends a POST request with JSON body and deserializes the response.
    /// </summary>
    public static async Task<T> PostJsonAsync<T>(
        this HttpClient client,
        string requestUri,
        object? content,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await client.PostAsJsonAsync(requestUri, content, JsonOptions.Default, cancellationToken).ConfigureAwait(false);
            await EnsureSuccessAsync(response, cancellationToken).ConfigureAwait(false);
            return await DeserializeResponseAsync<T>(response, cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            throw OpenCodeConnectionException.ServerNotReachable(client.BaseAddress?.ToString() ?? "unknown", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw OpenCodeTimeoutException.OperationTimedOut("POST " + requestUri, client.Timeout, ex);
        }
    }

    /// <summary>
    /// Sends a POST request with JSON body, expecting no response body.
    /// </summary>
    public static async Task PostJsonAsync(
        this HttpClient client,
        string requestUri,
        object? content,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await client.PostAsJsonAsync(requestUri, content, JsonOptions.Default, cancellationToken).ConfigureAwait(false);
            await EnsureSuccessAsync(response, cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            throw OpenCodeConnectionException.ServerNotReachable(client.BaseAddress?.ToString() ?? "unknown", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw OpenCodeTimeoutException.OperationTimedOut("POST " + requestUri, client.Timeout, ex);
        }
    }

    /// <summary>
    /// Sends a PUT request with JSON body and deserializes the response.
    /// </summary>
    public static async Task<T> PutJsonAsync<T>(
        this HttpClient client,
        string requestUri,
        object? content,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await client.PutAsJsonAsync(requestUri, content, JsonOptions.Default, cancellationToken).ConfigureAwait(false);
            await EnsureSuccessAsync(response, cancellationToken).ConfigureAwait(false);
            return await DeserializeResponseAsync<T>(response, cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            throw OpenCodeConnectionException.ServerNotReachable(client.BaseAddress?.ToString() ?? "unknown", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw OpenCodeTimeoutException.OperationTimedOut("PUT " + requestUri, client.Timeout, ex);
        }
    }

    /// <summary>
    /// Sends a PATCH request with JSON body and deserializes the response.
    /// </summary>
    public static async Task<T> PatchJsonAsync<T>(
        this HttpClient client,
        string requestUri,
        object? content,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, requestUri)
            {
                Content = JsonContent.Create(content, options: JsonOptions.Default)
            };
            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            await EnsureSuccessAsync(response, cancellationToken).ConfigureAwait(false);
            return await DeserializeResponseAsync<T>(response, cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            throw OpenCodeConnectionException.ServerNotReachable(client.BaseAddress?.ToString() ?? "unknown", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw OpenCodeTimeoutException.OperationTimedOut("PATCH " + requestUri, client.Timeout, ex);
        }
    }

    /// <summary>
    /// Sends a DELETE request.
    /// </summary>
    public static async Task DeleteAsync(
        this HttpClient client,
        string requestUri,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await client.DeleteAsync(requestUri, cancellationToken).ConfigureAwait(false);
            await EnsureSuccessAsync(response, cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            throw OpenCodeConnectionException.ServerNotReachable(client.BaseAddress?.ToString() ?? "unknown", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw OpenCodeTimeoutException.OperationTimedOut("DELETE " + requestUri, client.Timeout, ex);
        }
    }

    /// <summary>
    /// Sends a GET request for Server-Sent Events and returns an async enumerable of events.
    /// </summary>
    public static async IAsyncEnumerable<T> GetServerSentEventsAsync<T>(
        this HttpClient client,
        string requestUri,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Accept.ParseAdd("text/event-stream");

        HttpResponseMessage response;
        try
        {
            response = await client.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken).ConfigureAwait(false);

            await EnsureSuccessAsync(response, cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            throw OpenCodeConnectionException.ServerNotReachable(client.BaseAddress?.ToString() ?? "unknown", ex);
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false);

            if (string.IsNullOrEmpty(line))
                continue;

            // SSE format: "data: {...json...}"
            if (line.StartsWith("data: ", StringComparison.Ordinal))
            {
                var json = line.Substring(6);

                // End of stream marker
                if (json == "[DONE]")
                    yield break;

                T? data;
                try
                {
                    data = JsonSerializer.Deserialize<T>(json, JsonOptions.Default);
                }
                catch (JsonException)
                {
                    // Skip malformed events
                    continue;
                }

                if (data is not null)
                    yield return data;
            }
        }
    }

    /// <summary>
    /// Sends a POST request and returns SSE stream.
    /// </summary>
    public static async IAsyncEnumerable<T> PostServerSentEventsAsync<T>(
        this HttpClient client,
        string requestUri,
        object? content,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = JsonContent.Create(content, options: JsonOptions.Default)
        };
        request.Headers.Accept.ParseAdd("text/event-stream");

        HttpResponseMessage response;
        try
        {
            response = await client.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken).ConfigureAwait(false);

            await EnsureSuccessAsync(response, cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            throw OpenCodeConnectionException.ServerNotReachable(client.BaseAddress?.ToString() ?? "unknown", ex);
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false);

            if (string.IsNullOrEmpty(line))
                continue;

            // SSE format: "data: {...json...}"
            if (line.StartsWith("data: ", StringComparison.Ordinal))
            {
                var json = line.Substring(6);

                // End of stream marker
                if (json == "[DONE]")
                    yield break;

                T? data;
                try
                {
                    data = JsonSerializer.Deserialize<T>(json, JsonOptions.Default);
                }
                catch (JsonException)
                {
                    // Skip malformed events
                    continue;
                }

                if (data is not null)
                    yield return data;
            }
        }
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
            return;

        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var requestId = response.Headers.TryGetValues("X-Request-Id", out var values)
            ? values.FirstOrDefault()
            : null;

        // Try to parse error details from response body
        string? errorMessage = null;
        string? errorCode = null;

        try
        {
            var errorDoc = JsonDocument.Parse(content);
            if (errorDoc.RootElement.TryGetProperty("error", out var errorProp))
            {
                errorMessage = errorProp.GetString();
            }
            if (errorDoc.RootElement.TryGetProperty("code", out var codeProp))
            {
                errorCode = codeProp.GetString();
            }
            if (errorDoc.RootElement.TryGetProperty("message", out var msgProp))
            {
                errorMessage = msgProp.GetString();
            }
        }
        catch (JsonException)
        {
            // Response body is not JSON
            errorMessage = content;
        }

        errorMessage ??= response.ReasonPhrase ?? "Unknown error";

        throw response.StatusCode switch
        {
            HttpStatusCode.NotFound => new OpenCodeNotFoundException(
                errorMessage,
                resourceType: null,
                resourceId: null,
                requestId: requestId),

            HttpStatusCode.Conflict => new OpenCodeConflictException(
                errorMessage,
                requestId: requestId),

            >= HttpStatusCode.InternalServerError => new OpenCodeServerException(
                errorMessage,
                response.StatusCode,
                requestId: requestId),

            _ => new OpenCodeApiException(
                errorMessage,
                response.StatusCode,
                errorCode,
                requestId)
        };
    }

    private static async Task<T> DeserializeResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions.Default, cancellationToken).ConfigureAwait(false);
        return result ?? throw new OpenCodeException("Response was null");
    }
}
