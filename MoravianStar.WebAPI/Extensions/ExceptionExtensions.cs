using Microsoft.IdentityModel.Tokens;
using MoravianStar.Exceptions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security;

namespace MoravianStar.WebAPI.Extensions
{
    public static class ExceptionExtensions
    {
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
                default:
                    result = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            return result;
        }
    }
}