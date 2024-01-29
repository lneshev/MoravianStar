using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using MoravianStar.WebAPI.Middlewares;
using System;

namespace MoravianStar.WebAPI.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseMoravianStar(this IApplicationBuilder app, IWebHostEnvironment env, Action additionalSettings = null)
        {
            app.UseMiddleware<ServiceLocatorMiddleware>();
            app.UseMiddleware<ExceptionMiddleware>(env);
            if (additionalSettings != null)
            {
                additionalSettings();
            }
        }
    }
}