using Microsoft.AspNetCore.Antiforgery;
using Microsoft.IdentityModel.Tokens;
using MoravianStar.Exceptions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security;

namespace MoravianStar.Extensions
{
    /// <summary>
    /// Extension methods that are related to operations with exceptions.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Generates a correct HTTP status code for a given exception.
        /// </summary>
        /// <param name="exception">The source exception.</param>
        /// <returns>An <see cref="int"/> value representing the HTTP status code.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static int GetHttpStatusCode(this Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

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
                case AntiforgeryValidationException _:
                    result = 419;
                    break;
                default:
                    result = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            return result;
        }
    }
}