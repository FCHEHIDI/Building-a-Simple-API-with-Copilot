# Middleware Design and Rationale

## Exception Handling Middleware
Copilot implemented a global exception handling middleware to ensure all unhandled exceptions are caught and returned as standardized error responses. This middleware is placed at the start of the pipeline, so it can:
- Log all unhandled exceptions for diagnostics and auditing.
- Prevent leaking stack traces or sensitive information to clients.
- Return a consistent JSON error response structure across all endpoints.

**Why:**
This approach enforces corporate policy for standardized error handling and improves both security and maintainability. It also makes troubleshooting easier for developers and support staff.

## Token Authentication Middleware
Copilot implemented a token-based authentication middleware to secure all API endpoints. This middleware is registered after logging, ensuring:
- Every request (except Swagger and static assets) must include a valid `Authorization: Bearer <token>` header.
- Unauthorized requests are rejected with a 401 response and a clear error message.
- For demo purposes, a hardcoded token is used; in production, this would be replaced with JWT or another secure validation method.

**Why:**
This enforces corporate security policy, prevents unauthorized access, and provides a clear pattern for future enhancements (e.g., role-based access, JWT validation).
