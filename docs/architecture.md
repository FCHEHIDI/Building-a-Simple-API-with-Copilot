# System Architecture Diagram

```mermaid
graph TD
    subgraph API
        A[UsersController]
        B[UserDbContext]
        C[GlobalExceptionMiddleware]
        D[TokenAuthenticationMiddleware]
        E[RequestResponseLoggingMiddleware]
    end
    subgraph Database
        F[(SQLite DB)]
    end
    subgraph Client
        G[API Client / Swagger / Bulk Upload Script]
    end

    G -->|HTTP Requests| C
    C --> D
    D --> E
    E --> A
    A --> B
    B --> F
    A -->|Responses| G
    C -->|Error Responses| G
```

**Legend:**
- All requests flow through the middleware pipeline before reaching the controller.
- The controller interacts with the database context, which in turn uses SQLite.
- Clients include Swagger UI, curl, and the bulk upload script.
- Error and authentication handling are centralized in middleware.
