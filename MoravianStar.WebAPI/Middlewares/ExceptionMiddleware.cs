using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MoravianStar.Extensions;
using MoravianStar.WebAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net.Mime;
using System.Threading.Tasks;

namespace MoravianStar.WebAPI.Middlewares
{
    /// <summary>
    /// Custom exception middleware that catches any exception, logs the exception, creates a generic error model,
    /// generates a correct http status code related to the exception, puts it into the error model and writes the error model to the response.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IWebHostEnvironment env;
        private readonly ILogger<ExceptionMiddleware> logger;

        public ExceptionMiddleware(RequestDelegate next, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            this.next = next;
            this.env = env;
            logger = loggerFactory.CreateLogger<ExceptionMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                try
                {
                    LogCriticalException(ex);
                    await HandleExceptionAsync(context, ex);
                }
                catch { }
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var jsonSettings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var error = new ErrorModel()
            {
                Message = exception.Message,
                ExceptionType = env.IsDevelopment() ? exception.GetType().FullName : null,
                StackTrace = env.IsDevelopment() ? exception.StackTrace : null
            };
            var result = JsonConvert.SerializeObject(error, jsonSettings);

            context.Response.Clear();
            context.Response.StatusCode = exception.GetHttpStatusCode();
            context.Response.ContentType = MediaTypeNames.Application.Json;

            await context.Response.WriteAsync(result);
        }

        private void LogCriticalException(Exception ex)
        {
            logger.LogCritical(ex, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
        }
    }
}