using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MoravianStar.Extensions;
using MoravianStar.Resources;
using MoravianStar.WebAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;

namespace MoravianStar.WebAPI.Middlewares
{
    /// <summary>
    /// Custom exception middleware that catches any exception, logs the exception, creates a generic error model,
    /// sets a correct http status code related to the exception and writes the error model to the response.
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

        /// <summary>
        /// Processes an incoming HTTP request and invokes the next middleware in the pipeline.
        /// </summary>
        /// <remarks>This method ensures that any exceptions thrown during the execution of the middleware
        /// pipeline are logged and handled appropriately. If an exception occurs, it is logged as a critical error, and
        /// a custom exception handling mechanism is invoked.</remarks>
        /// <param name="context">The <see cref="HttpContext"/> representing the current HTTP request.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
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
                }
                catch { }

                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Handles exceptions that occur during the processing of an HTTP request and writes an appropriate error
        /// response.
        /// </summary>
        /// <remarks>This method serializes the exception details into a JSON response and sets the
        /// appropriate HTTP status code based on the type of exception. The response body is cleared before writing the
        /// error response, and the content type is set to "application/json".</remarks>
        /// <param name="context">The <see cref="HttpContext"/> representing the current HTTP request and response.</param>
        /// <param name="exception">The <see cref="Exception"/> that was thrown during request processing.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation of handling the exception.</returns>
        protected virtual async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var jsonSettings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var error = SetErrorModel(exception);
            var result = JsonConvert.SerializeObject(error, jsonSettings);

            // Clear only the response body before writing the error response
            ClearResponseBody(context.Response);

            // Set the appropriate status code based on the exception
            context.Response.StatusCode = SetHttpStatusCodeFromException(exception);

            // Set the content type to application/json
            context.Response.ContentType = MediaTypeNames.Application.Json;

            // Write the serialized error model to the response body
            await context.Response.WriteAsync(result);
        }

        /// <summary>
        /// Creates and returns an error model based on the provided exception.
        /// </summary>
        /// <remarks>This method is intended to be overridden in derived classes to customize the error
        /// model generation. The returned error model includes sensitive information, such as the stack trace, only
        /// when the application is running in a development environment.</remarks>
        /// <param name="exception">The exception to be used for generating the error model.</param>
        /// <returns>An object representing the error model, containing details such as the exception message.  Additional
        /// information, such as the exception type and stack trace, is included only in a development environment.</returns>
        protected virtual object SetErrorModel(Exception exception)
        {
            return new ErrorModel()
            {
                Message = exception.Message,
                ExceptionType = env.IsDevelopment() ? exception.GetType().FullName : null,
                StackTrace = env.IsDevelopment() ? exception.StackTrace : null
            };
        }

        /// <summary>
        /// Determines the appropriate HTTP status code based on the provided exception.
        /// </summary>
        /// <param name="exception">The exception from which to derive the HTTP status code.</param>
        /// <returns>An integer representing the HTTP status code that corresponds to the specified exception.</returns>
        protected virtual int SetHttpStatusCodeFromException(Exception exception)
        {
            return exception.GetHttpStatusCode();
        }

        private void LogCriticalException(Exception ex)
        {
            logger.LogCritical(ex, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
        }

        private void ClearResponseBody(HttpResponse response)
        {
            if (response.HasStarted)
            {
                throw new InvalidOperationException(Strings.TheResponseCannotBeClearedItHasAlreadyStartedSending);
            }
            if (response.Body.CanSeek)
            {
                response.Body.SetLength(0);
            }
        }
    }
}