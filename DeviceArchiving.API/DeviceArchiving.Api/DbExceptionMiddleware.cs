using DeviceArchiving.Data.Dto;
using System.Text.Json;

public class DbExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DbExceptionMiddleware> _logger;

    public DbExceptionMiddleware(RequestDelegate next, ILogger<DbExceptionMiddleware> logger)
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
            _logger.LogError(ex, "Exception occurred during request execution.");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            string message = IsDbConnectionException(ex)
                ? "ÕœÀ  „‘ﬂ·… ›Ì «·« ’«· »ﬁ«⁄œ… «·»Ì«‰« .  «ﬂœ „‰ ›ﬂ «· ‘›Ì—."
                : ex.Message;

            var response = BaseResponse<object>.Failure(message);
            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
    }

    private bool IsDbConnectionException(Exception ex)
    {
        while (ex != null)
        {
            if (ex is Microsoft.Data.SqlClient.SqlException ||
                ex is Microsoft.EntityFrameworkCore.DbUpdateException ||
                ex is TimeoutException ||
                (ex is InvalidOperationException && ex.Message.Contains("connection", StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            ex = ex.InnerException;
        }

        return false;
    }
}
