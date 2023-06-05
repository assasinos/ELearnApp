namespace ELearnApp.Middleware;

public class StatusCodeMiddleware
{
    private readonly RequestDelegate _next;

    public StatusCodeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        await _next(context);

        if (context.Request.Path.StartsWithSegments("/api")) return;
            switch (context.Response.StatusCode)
        {
            case 400:
                context.Response.ContentType = "text/html";
                context.Response.Redirect("/Error/400");
                break;
            case 401:
                context.Response.ContentType = "text/html";
                context.Response.Redirect("/Error/401");
                break;
            case 403:
                context.Response.ContentType = "text/html";
                context.Response.Redirect("/Error/403");
                break;
            case 404:
                context.Response.ContentType = "text/html";
                context.Response.Redirect("/Error/404");
                break;
            case 500:
                context.Response.ContentType = "text/html";
                context.Response.Redirect("/Error/500");
                break;
        }



    }
}