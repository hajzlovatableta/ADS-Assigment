using WebAPI.Exceptions;

namespace WebAPI.Middleware;

public class GlobalExceptionHandler : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            int statusCode = e switch
            {
                BadRequestException => StatusCodes.Status400BadRequest,
                // The file repositories throw InvalidOperationException when an entity is not found
                InvalidOperationException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(new
            {
                status = statusCode,
                error = e.Message
            });
        }
    }
}