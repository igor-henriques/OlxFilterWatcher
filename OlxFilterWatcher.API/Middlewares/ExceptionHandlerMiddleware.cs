namespace OlxFilterWatcher.API.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException ex)
        {
            await Handle(context, ex);
        }
        catch (ArgumentNullException ex)
        {
            await Handle(context, ex);
        }
        catch (Exception ex)
        {
            await Handle(context, ex);
        }
    }
    private async Task Handle(HttpContext context, Exception ex)
    {
        Log.Error(ex, "Internal Error");

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(
            new
            {
                Messages = ex.Message.Split("\n"),
                StatusCode = context.Response.StatusCode
            }));
    }

    private async Task Handle (HttpContext context, ArgumentNullException ex)
    {
        Log.Error(ex, "Validation Error");

        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(
            new
            {
                Messages = ex.Message.Split("\n"),
                StatusCode = context.Response.StatusCode
            }));
    }

    private async Task Handle(HttpContext context, OperationCanceledException ex)
    {
        Log.Information(ex, "Operation canceled by the user");

        context.Response.StatusCode = 0;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(
            new
            {
                Messages = "Requisição cancelada pelo usuário",
                StatusCode = context.Response.StatusCode
            }));
    }
}