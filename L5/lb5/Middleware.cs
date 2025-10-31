using System.Diagnostics;

namespace Labb5.Middleware;

public class CorrelationMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationHeader = "X-Correlation-ID";

    public CorrelationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Dacă cererea nu are ID, generăm unul
        if (!context.Request.Headers.TryGetValue(CorrelationHeader, out var correlationId))
        {
            correlationId = Guid.NewGuid().ToString("N")[..8];
            context.Request.Headers[CorrelationHeader] = correlationId;
        }

        context.Response.Headers[CorrelationHeader] = correlationId;

        // Adăugăm în contextul global pentru logging (DiagnosticContext)
        using var scope = context.RequestServices.GetRequiredService<ILogger<CorrelationMiddleware>>()
            .BeginScope(new Dictionary<string, object?>
            {
                ["CorrelationId"] = correlationId
            });

        await _next(context);
    }
}