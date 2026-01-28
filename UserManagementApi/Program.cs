var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Configure middleware in correct order
// 1. Error-handling middleware first
app.UseErrorHandlingMiddleware();

// 2. Authentication middleware next
app.UseTokenValidationMiddleware();

// 3. Logging middleware last
app.UseLoggingMiddleware();

var blogs = new List<Blog>
{
    new Blog {Title = "First", Body = "Blog 1"},
    new Blog {Title = "Second", Body = "Blog 2"}
};

app.MapGet("/blogs", () =>
{
    return blogs;
});

app.MapGet("/blogs/{id}", (int id) =>
{
    if(id < 0 || id >= blogs.Count)
    {
        return Results.NotFound();
    }
    else{return Results.Ok(blogs[id]);}
});

app.MapPost("/blogs", (Blog blog) =>
{
    blogs.Add(blog);
    return Results.Created($"/blogs/{blogs.Count - 1}", blog);
});

app.MapPut("/blogs/{id}", (int id, Blog blog) =>
{
    if(id < 0 || id >= blogs.Count)
    {
        return Results.NotFound();
    }
    blogs[id] = blog;
    return Results.Ok(blog);
});

app.MapDelete("/blogs/{id}", (int id) =>
{
    if(id <= 0 || id > blogs.Count)
    {
        return Results.NotFound();
    }
    else{blogs.RemoveAt(id);
    return Results.NoContent();}
});

app.Run();


public class Blog
{
    public required string Title { get; set; }
    public required string Body { get; set; }
}

// Logging Middleware
public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var method = context.Request.Method;
        var path = context.Request.Path;

        await _next(context);

        var statusCode = context.Response.StatusCode;
        _logger.LogInformation($"HTTP {method} {path} - Response: {statusCode}");
    }
}

// Error Handling Middleware
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unhandled exception: {ex.Message}");
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var errorResponse = new { error = "Internal server error." };
            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}

// Token Validation Middleware
public class TokenValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TokenValidationMiddleware> _logger;

    public TokenValidationMiddleware(RequestDelegate next, ILogger<TokenValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("Authorization", out var token))
        {
            _logger.LogWarning("Missing authorization token");
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            var errorResponse = new { error = "Unauthorized: Missing token." };
            await context.Response.WriteAsJsonAsync(errorResponse);
            return;
        }

        // Validate token (simple validation - check if token starts with "Bearer ")
        if (!token.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Invalid token format");
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            var errorResponse = new { error = "Unauthorized: Invalid token format." };
            await context.Response.WriteAsJsonAsync(errorResponse);
            return;
        }

        _logger.LogInformation("Valid token provided");
        await _next(context);
    }
}

// Extension methods for middleware registration
public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LoggingMiddleware>();
    }

    public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }

    public static IApplicationBuilder UseTokenValidationMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TokenValidationMiddleware>();
    }
}