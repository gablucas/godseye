using GodsEye.API.DTO;
using GodsEye.API.Exceptions;
using MySqlConnector;
using System.Net;
using System.Text.Json;

namespace GodsEye.API.Middlewares
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro não tratado: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = exception switch
            {
                GodsEyeServiceException ex => (HttpStatusCode)ex.StatusCode,
                MediaMtxServiceException ex => (HttpStatusCode)ex.StatusCode,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                KeyNotFoundException => HttpStatusCode.NotFound,
                ArgumentException => HttpStatusCode.BadRequest,
                InvalidOperationException => HttpStatusCode.BadRequest,
                MySqlException => HttpStatusCode.InternalServerError,
                _ => HttpStatusCode.InternalServerError
            };

            string message;

            if (exception is MySqlException)
                message = "Erro interno ao acessar o banco de dados. Por favor, tente novamente mais tarde";
            else
                message = exception.Message;

            var response = new
            {
                Status = (int)statusCode,
                Message = message,
                StackTrace = _env.IsDevelopment() ? exception.StackTrace : null
            };
                
            

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            await context.Response.WriteAsync(JsonSerializer.Serialize(
                response,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = _env.IsDevelopment()
                }
            ));
        }
    }
}
