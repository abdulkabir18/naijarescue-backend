using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Application.Interfaces.CurrentUser;
using Domain.Entities;
using Domain.Enums;
using System.Text.Json;

namespace Infrastructure.Middlewares
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditMiddleware> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using var scope = _scopeFactory.CreateScope();
            var currentUserService = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();

            try
            {
                await _next(context);

                var auditInfo = new AuditLog
                (
                    AuditActionType.Accessed,
                    "HttpRequest",
                    null,
                    $"{context.Request.Method} {context.Request.Path}",
                    currentUserService.UserId == Guid.Empty ? null : currentUserService.UserId,
                    currentUserService.IpAddress,
                    currentUserService.UserAgent
                );

                _logger.LogInformation("Audit: {AuditInfo}", JsonSerializer.Serialize(auditInfo));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AuditMiddleware for path {Path}", context.Request.Path);
                throw;
            }
        }
    }
}
