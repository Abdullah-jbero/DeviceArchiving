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
            if (IsDbConnectionException(ex))
            {
                _logger.LogError(ex, "Database connection error occurred.");
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    Message = "ÕœÀ  „‘ﬂ·… ›Ì «·« ’«· »ﬁ«⁄œ… «·»Ì«‰« . Ì—ÃÏ «·„Õ«Ê·… ·«Õﬁ«."
                };
                var json = System.Text.Json.JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
            else
            {
             
                throw;
            }
        }
    }

    private bool IsDbConnectionException(Exception ex)
    {
        while (ex != null)
        {
            if (ex is Microsoft.Data.SqlClient.SqlException) // Œÿ√ SQL Server
                return true;

            if (ex is Microsoft.EntityFrameworkCore.DbUpdateException) // Œÿ√  ÕœÌÀ EF Core
                return true;

            if (ex is TimeoutException) // «‰ Â«¡ „Â·… «·« ’«·
                return true;

            if (ex is InvalidOperationException && ex.Message.Contains("connection")) // —”«·… «·Œÿ√  Õ ÊÌ ⁄·Ï ﬂ·„… connection
                return true;

            ex = ex.InnerException ;
        }
        return false;
    }

}
