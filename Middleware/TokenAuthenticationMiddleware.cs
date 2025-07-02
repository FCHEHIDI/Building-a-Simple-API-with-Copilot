using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagementAPI.Middleware
{
    public class TokenAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TokenAuthenticationMiddleware> _logger;
        private const string AUTH_HEADER = "Authorization";
        private const string BEARER_PREFIX = "Bearer ";
        // For demo: hardcoded valid token. In production, validate JWT or use a secure store.
        private const string VALID_TOKEN = "demo-token-123";

        public TokenAuthenticationMiddleware(RequestDelegate next, ILogger<TokenAuthenticationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation($"[TokenAuth] Path: {context.Request.Path}");
            if (context.Request.Path.StartsWithSegments("/swagger") ||
                context.Request.Path.StartsWithSegments("/favicon.ico"))
            {
                _logger.LogInformation("[TokenAuth] Skipping auth for Swagger or favicon.");
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue(AUTH_HEADER, out var authHeader) ||
                authHeader.Count == 0 || !authHeader.Any(h => h != null && h.StartsWith(BEARER_PREFIX)))
            {
                _logger.LogWarning("[TokenAuth] Missing or invalid Authorization header.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("{\"message\":\"Unauthorized: Missing or invalid token.\"}");
                return;
            }

            var token = authHeader.FirstOrDefault(h => h != null && h.StartsWith(BEARER_PREFIX));
            if (string.IsNullOrEmpty(token) || token.Length <= BEARER_PREFIX.Length)
            {
                _logger.LogWarning("[TokenAuth] Missing or invalid Authorization header format.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("{\"message\":\"Unauthorized: Missing or invalid token.\"}");
                return;
            }
            token = token.Substring(BEARER_PREFIX.Length);
            if (token != VALID_TOKEN)
            {
                _logger.LogWarning($"[TokenAuth] Unauthorized: Invalid token '{token}'.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("{\"message\":\"Unauthorized: Invalid token.\"}");
                return;
            }

            _logger.LogInformation("[TokenAuth] Token validated, proceeding to next middleware.");
            await _next(context);
        }
    }
}
