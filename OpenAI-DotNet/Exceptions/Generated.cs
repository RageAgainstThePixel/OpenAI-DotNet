#nullable enable
using OpenAI;
using System;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;

public class OpenAiException : Exception
{
    public ApiError? Error { get; }

    /// <summary>
    /// The underlying HTTP response's body in string format.
    /// </summary>
    /// <remarks>
    /// <b>null</b> if the request failed before receiving a response, represented by a <see cref="OpenAIConnectionException"/>.
    /// </remarks>
    public string? ResponseBody { get; }

    internal OpenAiException(string? body, string message, ApiError? error = null, Exception? cause = null) : base(message, cause)
    {
        ResponseBody = body;
        Error = error;
    }
}

// The following classes extend APIException and are specialized based on different HTTP status codes or error scenarios
public class OpenAIResponseValidationException : OpenAiException
{
    public HttpStatusCode StatusCode { get; set; }

    public OpenAIResponseValidationException(HttpStatusCode statusCode,
        string body, string message = "Data returned by API invalid for expected schema.", ApiError? error = null) : base(body, message, error)
    {
        StatusCode = statusCode;
    }
}

public class OpenAIStatusException : OpenAiException
{
    public int StatusCode { get; set; }

    public OpenAIStatusException(string message, int statusCode, string body, ApiError? error) : base(body, message, error)
    {
        StatusCode = statusCode;
    }
}

public class OpenAIConnectionException : OpenAiException
{
    public OpenAIConnectionException(Exception cause, string errorMessage = "Connection error.") : base(null, errorMessage, cause: cause) { }
}

public class OpenAITimeoutException : OpenAIConnectionException
{
    public OpenAITimeoutException(Exception cause) : base(cause, "Request timed out.") { }
}

// Specific HTTP status code exceptions
public class OpenAIBadRequestException : OpenAIStatusException
{
    public OpenAIBadRequestException(string body, ApiError? apiError, string errorMessage = "Bad Request") : base(errorMessage, 400, body, apiError) { }
}

public class OpenAIAuthenticationException : OpenAIStatusException
{
    public OpenAIAuthenticationException(string body, ApiError? apiError, string errorMessage = "Authentication Failed") : base(errorMessage, 401, body, apiError) { }
}

public class OpenAIPermissionDeniedException : OpenAIStatusException
{
    public OpenAIPermissionDeniedException(string body, ApiError? apiError, string errorMessage = "Permission Denied") : base(errorMessage, 403, body, apiError) { }
}

public class OpenAINotFoundException : OpenAIStatusException
{
    public OpenAINotFoundException(string body, ApiError? apiError, string errorMessage = "Not Found") : base(errorMessage, 404, body, apiError) { }
}

public class ConflictException : OpenAIStatusException
{
    public ConflictException(string body, ApiError? apiError, string errorMessage = "Conflict") : base(errorMessage, 409, body, apiError) { }
}

public class UnprocessableEntityException : OpenAIStatusException
{
    public UnprocessableEntityException(string body, ApiError? apiError, string errorMessage = "Unprocessable Entity") : base(errorMessage, 422, body, apiError) { }
}

public class RateLimitException : OpenAIStatusException
{
    private const string XRateLimitLimitRequests = "x-ratelimit-limit-requests";
    private const string XRateLimitLimitTokens = "x-ratelimit-limit-tokens";
    private const string XRateLimitRemainingRequests = "x-ratelimit-remaining-requests";
    private const string XRateLimitRemainingTokens = "x-ratelimit-remaining-tokens";
    private const string XRateLimitResetRequests = "x-ratelimit-reset-requests";
    private const string XRateLimitResetTokens = "x-ratelimit-reset-tokens";

    /// <summary>
    /// The maximum number of requests that are permitted before exhausting the rate limit.
    /// </summary>
    public int? LimitRequests { get; }

    /// <summary>
    /// The maximum number of tokens that are permitted before exhausting the rate limit.
    /// </summary>
    public int? LimitTokens { get; }

    /// <summary>
    /// The remaining number of requests that are permitted before exhausting the rate limit.
    /// </summary>
    public int? RemainingRequests { get; }

    /// <summary>
    /// The remaining number of tokens that are permitted before exhausting the rate limit.
    /// </summary>
    public int? RemainingTokens { get; }

    /// <summary>
    /// The time until the rate limit (based on requests) resets to its initial state.
    /// </summary>
    public string? ResetRequests { get; }

    /// <summary>
    /// The time until the rate limit (based on tokens) resets to its initial state.
    /// </summary>
    public string? ResetTokens { get; }

    public RateLimitException(HttpResponseHeaders headers, string body, ApiError? apiError, string errorMessage = "Rate Limit Exceeded") : base(errorMessage, 429, body, apiError)
    {
        if (headers.TryGetValues(XRateLimitLimitRequests, out var limitRequests) &&
                int.TryParse(limitRequests.FirstOrDefault(), out var limitRequestsValue))
        {
            LimitRequests = limitRequestsValue;
        }

        if (headers.TryGetValues(XRateLimitLimitTokens, out var limitTokens) &&
            int.TryParse(limitTokens.FirstOrDefault(), out var limitTokensValue))
        {
            LimitTokens = limitTokensValue;
        }

        if (headers.TryGetValues(XRateLimitRemainingRequests, out var remainingRequests) &&
            int.TryParse(remainingRequests.FirstOrDefault(), out var remainingRequestsValue))
        {
            RemainingRequests = remainingRequestsValue;
        }

        if (headers.TryGetValues(XRateLimitRemainingTokens, out var remainingTokens) &&
            int.TryParse(remainingTokens.FirstOrDefault(), out var remainingTokensValue))
        {
            RemainingTokens = remainingTokensValue;
        }

        if (headers.TryGetValues(XRateLimitResetRequests, out var resetRequests))
        {
            ResetRequests = resetRequests.FirstOrDefault();
        }

        if (headers.TryGetValues(XRateLimitResetTokens, out var resetTokens))
        {
            ResetTokens = resetTokens.FirstOrDefault();
        }
    }
}

public class InternalServerException : OpenAIStatusException
{
    public InternalServerException(int statusCode, string body, ApiError? apiError) : base("Internal Server Error", statusCode, body, apiError) { }
}
