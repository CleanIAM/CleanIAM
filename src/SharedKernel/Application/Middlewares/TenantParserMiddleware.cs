using Wolverine;

namespace SharedKernel.Application.Middlewares;

public class TenantParserMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IMessageBus bus)
    {
        if (context.User.Identity is { IsAuthenticated: true })
        {
            var tenantId = context.User.Claims
                .FirstOrDefault(c => c.Type == SharedKernelConstants.TenantClaimName)?.Value;
            if (tenantId != null)
                bus.TenantId = tenantId;
        }

        // Call the next delegate/middleware in the pipeline.
        await next(context);
    }
}