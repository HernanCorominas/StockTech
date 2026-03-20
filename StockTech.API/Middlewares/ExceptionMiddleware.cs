using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace StockTech.API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ExceptionResponse
        {
            Message = "Se produjo un error inesperado. Por favor, intente de nuevo más tarde.",
            StatusCode = (int)HttpStatusCode.InternalServerError
        };

        switch (exception)
        {
            case ValidationException validationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = "Error de validación.";
                response.Errors = validationException.Errors.Select(e => e.ErrorMessage).ToList();
                break;

            case ArgumentException:
            case InvalidOperationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = exception.Message;
                break;

            case KeyNotFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Message = "El recurso solicitado no fue encontrado.";
                break;
                
            case DbUpdateConcurrencyException:
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                response.StatusCode = (int)HttpStatusCode.Conflict;
                response.Message = "El recurso fue modificado por otro usuario. Intente nuevamente.";
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Details = exception.Message; // Should be omitted in production
                break;
        }

        var result = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        return context.Response.WriteAsync(result);
    }
}

public class ExceptionResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public List<string>? Errors { get; set; }
}
