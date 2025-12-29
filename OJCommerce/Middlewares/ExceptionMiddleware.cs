using Microsoft.EntityFrameworkCore;
using OJCommerce.Exceptions;

namespace OJCommerce.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database Error");

                var errorMessage = GetFriendlyDatabaseMessage(ex);

                await WriteError(context, errorMessage, 400);
            }
            catch (UnauthorizedException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access");
                await WriteError(context, ex.Message, StatusCodes.Status401Unauthorized);
            }

            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Not Found");
                await WriteError(context, ex.Message, StatusCodes.Status404NotFound);
            }

            catch (BusinessRuleViolationException ex)
            {
                _logger.LogWarning(ex, "Business Rule Violation");
                await WriteError(context, ex.Message, StatusCodes.Status400BadRequest);
            }


            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error");
                await WriteError(context, ex.Message, 400);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Server Error");
                await WriteError(context, "Internal server error", 500);
            }
        }


        private static async Task WriteError(HttpContext context, string message, int status)
        {
            context.Response.StatusCode = status;
            context.Response.ContentType = "application/json";

            var response = new { success = false, message };
            await context.Response.WriteAsJsonAsync(response);
        }


        private string GetFriendlyDatabaseMessage(DbUpdateException ex)
        {
            var message = ex.InnerException?.Message ?? "";

            if (message.Contains("Duplicate entry"))
            {
                try
                {
                    // Extract the duplicated VALUE
                    var valueStart = message.IndexOf("Duplicate entry '") + "Duplicate entry '".Length;
                    var valueEnd = message.IndexOf("'", valueStart);
                    var duplicatedValue = message.Substring(valueStart, valueEnd - valueStart);

                    // Extract the INDEX NAME
                    var keyStart = message.IndexOf("for key '") + "for key '".Length;
                    var keyEnd = message.IndexOf("'", keyStart);
                    var indexName = message.Substring(keyStart, keyEnd - keyStart);

                    // Example indexName: "categories.IX_Categories_Name"
                    string fieldName = ExtractColumnNameFromIndex(indexName);

                    return $"{fieldName} '{duplicatedValue}' already exists.";
                }
                catch
                {
                    // Fallback in case parsing fails
                    return "A record with the same value already exists.";
                }
            }

            // Foreign key violations
            if (message.Contains("FOREIGN KEY"))
            {
                return "The provided reference is invalid.";
            }

            return "Unable to process your request at this time.";
        }


        private string ExtractColumnNameFromIndex(string indexName)
        {
            var parts = indexName.Split('_', StringSplitOptions.RemoveEmptyEntries);

            var raw = parts.Last();

            return raw;
        }


    }
}
