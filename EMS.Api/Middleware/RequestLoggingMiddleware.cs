namespace EMS.Api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.EnableBuffering();
        var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Position = 0;

        _logger.LogInformation(
            "Request: {Method} {Path} | Body: {Body}",
            context.Request.Method,
            context.Request.Path,
            string.IsNullOrWhiteSpace(requestBody) ? "(empty)" : requestBody);

        var originalBody = context.Response.Body;
        using var memStream = new MemoryStream();
        context.Response.Body = memStream;

        await _next(context);

        var endpoint = context.GetEndpoint();
        var action = endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>();
        var controller = action?.ControllerName ?? "Unknown";
        var actionName = action?.ActionName ?? "Unknown";

        memStream.Position = 0;
        var responseBody = await new StreamReader(memStream).ReadToEndAsync();
        memStream.Position = 0;

        _logger.LogInformation(
            "[{Controller}/{Action}] Response: {StatusCode} | Body: {Body}{NewLine}",
            controller,
            actionName,
            context.Response.StatusCode,
            string.IsNullOrWhiteSpace(responseBody) ? "(empty)" : responseBody,
            Environment.NewLine);

        await memStream.CopyToAsync(originalBody);
        context.Response.Body = originalBody;
    }
}
