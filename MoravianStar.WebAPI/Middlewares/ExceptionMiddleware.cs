using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MoravianStar.Exceptions;
using MoravianStar.WebAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mime;
using System.Security;
using System.Threading.Tasks;

namespace MoravianStar.WebAPI.Middlewares
{
    /// <summary>
    /// Custom exception middleware that cathes any exception, logs the exception, creates a generic error model,
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

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
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
            context.Response.StatusCode = GetHttpStatusCode(exception);
            context.Response.ContentType = MediaTypeNames.Application.Json;

            return context.Response.WriteAsync(result);
        }

        private void LogCriticalException(Exception ex)
        {
            logger.LogCritical(ex, ex.InnerException != null
                ? ex.InnerException.Message
                : ex.Message);
        }

        private int GetHttpStatusCode(Exception exception)
        {
            int result;

            switch (exception)
            {
                case InvalidModelStateException _:
                case ValidationException _:
                case BusinessException _:
                    result = (int)HttpStatusCode.BadRequest;
                    break;
                case SecurityException _:
                case SecurityTokenExpiredException _:
                    result = (int)HttpStatusCode.Unauthorized;
                    break;
                case EntityNotFoundException _:
                    result = (int)HttpStatusCode.NotFound;
                    break;
                case MethodAccessException _:
                case NotImplementedException _:
                    result = (int)HttpStatusCode.MethodNotAllowed;
                    break;
                case EntityNotUniqueException _:
                    result = (int)HttpStatusCode.Conflict;
                    break;
                default:
                    result = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            return result;
        }
    }
}