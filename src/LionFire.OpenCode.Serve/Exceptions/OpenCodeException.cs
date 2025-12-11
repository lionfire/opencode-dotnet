using System.Net;

namespace LionFire.OpenCode.Serve.Exceptions;

/// <summary>
/// Base exception for all OpenCode SDK errors.
/// </summary>
public class OpenCodeException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenCodeException"/> class.
    /// </summary>
    public OpenCodeException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenCodeException"/> class with a message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public OpenCodeException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenCodeException"/> class with a message and inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public OpenCodeException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when the OpenCode server returns an error response.
/// </summary>
public class OpenCodeApiException : OpenCodeException
{
    /// <summary>
    /// Gets the HTTP status code from the response.
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Gets the error code from the API response, if available.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Gets the request ID for troubleshooting, if available.
    /// </summary>
    public string? RequestId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenCodeApiException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="errorCode">The API error code, if available.</param>
    /// <param name="requestId">The request ID, if available.</param>
    public OpenCodeApiException(
        string message,
        HttpStatusCode statusCode,
        string? errorCode = null,
        string? requestId = null) : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        RequestId = requestId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenCodeApiException"/> class with an inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="innerException">The inner exception.</param>
    /// <param name="errorCode">The API error code, if available.</param>
    /// <param name="requestId">The request ID, if available.</param>
    public OpenCodeApiException(
        string message,
        HttpStatusCode statusCode,
        Exception innerException,
        string? errorCode = null,
        string? requestId = null) : base(message, innerException)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        RequestId = requestId;
    }
}

/// <summary>
/// Exception thrown when a requested resource is not found (HTTP 404).
/// </summary>
public class OpenCodeNotFoundException : OpenCodeApiException
{
    /// <summary>
    /// Gets the type of resource that was not found.
    /// </summary>
    public string? ResourceType { get; }

    /// <summary>
    /// Gets the ID of the resource that was not found.
    /// </summary>
    public string? ResourceId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenCodeNotFoundException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="resourceType">The type of resource not found.</param>
    /// <param name="resourceId">The ID of the resource not found.</param>
    /// <param name="requestId">The request ID, if available.</param>
    public OpenCodeNotFoundException(
        string message,
        string? resourceType = null,
        string? resourceId = null,
        string? requestId = null) : base(message, HttpStatusCode.NotFound, "not_found", requestId)
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }
}

/// <summary>
/// Exception thrown when an operation conflicts with the current state (HTTP 409).
/// </summary>
public class OpenCodeConflictException : OpenCodeApiException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenCodeConflictException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="requestId">The request ID, if available.</param>
    public OpenCodeConflictException(
        string message,
        string? requestId = null) : base(message, HttpStatusCode.Conflict, "conflict", requestId)
    {
    }
}

/// <summary>
/// Exception thrown when the server returns a 5xx error.
/// </summary>
public class OpenCodeServerException : OpenCodeApiException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenCodeServerException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="requestId">The request ID, if available.</param>
    public OpenCodeServerException(
        string message,
        HttpStatusCode statusCode,
        string? requestId = null) : base(message, statusCode, "server_error", requestId)
    {
    }
}

/// <summary>
/// Exception thrown when the OpenCode server cannot be reached.
/// </summary>
public class OpenCodeConnectionException : OpenCodeException
{
    /// <summary>
    /// Gets the base URL that was being connected to.
    /// </summary>
    public string? BaseUrl { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenCodeConnectionException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="baseUrl">The base URL that was being connected to.</param>
    /// <param name="innerException">The inner exception.</param>
    public OpenCodeConnectionException(
        string message,
        string? baseUrl = null,
        Exception? innerException = null) : base(message, innerException!)
    {
        BaseUrl = baseUrl;
    }

    /// <summary>
    /// Creates a connection exception with a helpful message about the server not being reachable.
    /// </summary>
    /// <param name="baseUrl">The base URL that was being connected to.</param>
    /// <param name="innerException">The inner exception.</param>
    /// <returns>A new <see cref="OpenCodeConnectionException"/> with a helpful message.</returns>
    public static OpenCodeConnectionException ServerNotReachable(string baseUrl, Exception? innerException = null)
    {
        return new OpenCodeConnectionException(
            $"OpenCode server not responding at {baseUrl}. Is `opencode serve` running? " +
            $"Start the server with: opencode serve --port {new Uri(baseUrl).Port}",
            baseUrl,
            innerException);
    }
}

/// <summary>
/// Exception thrown when an operation times out.
/// </summary>
public class OpenCodeTimeoutException : OpenCodeException
{
    /// <summary>
    /// Gets the timeout that was exceeded.
    /// </summary>
    public TimeSpan Timeout { get; }

    /// <summary>
    /// Gets the operation that timed out.
    /// </summary>
    public string? Operation { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenCodeTimeoutException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="timeout">The timeout that was exceeded.</param>
    /// <param name="operation">The operation that timed out.</param>
    /// <param name="innerException">The inner exception.</param>
    public OpenCodeTimeoutException(
        string message,
        TimeSpan timeout,
        string? operation = null,
        Exception? innerException = null) : base(message, innerException!)
    {
        Timeout = timeout;
        Operation = operation;
    }

    /// <summary>
    /// Creates a timeout exception with a helpful message.
    /// </summary>
    /// <param name="operation">The operation that timed out.</param>
    /// <param name="timeout">The timeout that was exceeded.</param>
    /// <param name="innerException">The inner exception.</param>
    /// <returns>A new <see cref="OpenCodeTimeoutException"/> with a helpful message.</returns>
    public static OpenCodeTimeoutException OperationTimedOut(
        string operation,
        TimeSpan timeout,
        Exception? innerException = null)
    {
        return new OpenCodeTimeoutException(
            $"The {operation} operation timed out after {timeout.TotalSeconds:F1} seconds. " +
            $"Try increasing the timeout in OpenCodeClientOptions.",
            timeout,
            operation,
            innerException);
    }
}
