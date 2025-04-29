using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MinimalApi
{
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
            catch(Exception ex)
            {
                _logger.LogError(ex, "Exception occurs");
                await handleExceptionAsync(context, ex);
            }
        }
        private Task handleExceptionAsync(HttpContext context,  Exception exception)
        {
            //Maybe the response was sent before the exception being catched in this middleware
            if(context.Response.HasStarted)
            {
                return Task.CompletedTask;
            }
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var problems = new ProblemDetails
            {
                Title = "Exception occured at :" + exception.Source,
                Detail = exception.Message,
            };
            return context.Response.WriteAsJsonAsync(problems);
        }
    }
}
