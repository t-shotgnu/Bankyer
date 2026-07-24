using System.Text.Json;

namespace Bankyer.Api.Infrastructure;

public sealed record ApiErrorResponse(string Code, string Message);

public sealed record ApiResponse<T>(bool Success, T? Data, ApiErrorResponse? Error)
{
    public static ApiResponse<T> Ok(T? data) => new(true, data, null);

    public static ApiResponse<T> Fail(string code, string message) => new(false, default, new ApiErrorResponse(code, message));
}

/// <summary>Wraps every API response in the standard Bankyer response envelope.</summary>
public sealed class ApiResponseMiddleware(RequestDelegate next, ILogger<ApiResponseMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            await next(context);
            return;
        }

        var originalBody = context.Response.Body;
        await using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception while processing {Method} {Path}",
                context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = null;
            responseBody.SetLength(0);
        }
        finally
        {
            context.Response.Body = originalBody;
        }

        var responseText = await ReadResponseAsync(responseBody);
        var statusCode = context.Response.StatusCode;

        if (statusCode == StatusCodes.Status204NoContent)
        {
            statusCode = StatusCodes.Status200OK;
            context.Response.StatusCode = statusCode;
        }

        var response = IsSuccessStatusCode(statusCode)
            ? ApiResponse<JsonElement?>.Ok(ReadJson(responseText))
            : ApiResponse<JsonElement?>.Fail(GetErrorCode(statusCode), GetErrorMessage(responseText, statusCode));

        context.Response.ContentType = "application/json; charset=utf-8";
        context.Response.ContentLength = null;
        await JsonSerializer.SerializeAsync(originalBody, response, JsonOptions, context.RequestAborted);
    }

    private static async Task<string> ReadResponseAsync(Stream responseBody)
    {
        responseBody.Position = 0;
        using var reader = new StreamReader(
            responseBody,
            System.Text.Encoding.UTF8,
            detectEncodingFromByteOrderMarks: true,
            bufferSize: 1024,
            leaveOpen: true);
        return await reader.ReadToEndAsync();
    }

    private static JsonElement? ReadJson(string responseText)
    {
        if (string.IsNullOrWhiteSpace(responseText))
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(responseText);
            return document.RootElement.Clone();
        }
        catch (JsonException)
        {
            return JsonSerializer.SerializeToElement(responseText, JsonOptions);
        }
    }

    private static string GetErrorMessage(string responseText, int statusCode)
    {
        if (string.IsNullOrWhiteSpace(responseText))
        {
            return $"Request failed with status {statusCode}.";
        }

        try
        {
            using var document = JsonDocument.Parse(responseText);
            var root = document.RootElement;
            if (root.ValueKind == JsonValueKind.String)
            {
                return root.GetString() ?? $"Request failed with status {statusCode}.";
            }

            if (root.ValueKind == JsonValueKind.Object)
            {
                if (root.TryGetProperty("detail", out var detail) && detail.ValueKind == JsonValueKind.String)
                {
                    return detail.GetString()!;
                }

                if (root.TryGetProperty("title", out var title) && title.ValueKind == JsonValueKind.String)
                {
                    return title.GetString()!;
                }
            }
        }
        catch (JsonException)
        {
            // Plain-text errors are already a useful client message.
        }

        return responseText;
    }

    private static bool IsSuccessStatusCode(int statusCode) => statusCode is >= 200 and < 300;

    private static string GetErrorCode(int statusCode) => statusCode switch
    {
        StatusCodes.Status400BadRequest => "bad_request",
        StatusCodes.Status401Unauthorized => "unauthorized",
        StatusCodes.Status403Forbidden => "forbidden",
        StatusCodes.Status404NotFound => "not_found",
        StatusCodes.Status409Conflict => "conflict",
        _ => "internal_error",
    };
}
