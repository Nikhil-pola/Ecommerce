using System;

namespace API.Middleware;

public class ExceptionMiddleware(IHostEnvironment env, RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = env.IsDevelopment()
                ? new Errors.ApiErrorResponse(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                : new Errors.ApiErrorResponse(context.Response.StatusCode, "Internal Server Error", null);

            var options = new System.Text.Json.JsonSerializerOptions { PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase };

            var json = System.Text.Json.JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }
    }
}
