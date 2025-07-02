using UserManagementAPI.Middleware;
using Microsoft.OpenApi.Models;
using UserManagementAPI.Data;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Configure EF Core with SQLite
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=users.db"));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserManagementAPI", Version = "v1" });
});


var app = builder.Build();

// Add global exception handler middleware (first)
app.UseMiddleware<GlobalExceptionMiddleware>();

// Use routing before custom middleware
app.UseRouting();

// Add token authentication middleware (second)
app.UseMiddleware<TokenAuthenticationMiddleware>();

// Add request/response logging middleware (third)
app.UseMiddleware<RequestResponseLoggingMiddleware>();

// Swagger and dev tools
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserManagementAPI v1");
    });
}

app.UseHttpsRedirection();


// Map controllers after all custom middleware (top-level registration)
app.MapControllers();

app.Run();

