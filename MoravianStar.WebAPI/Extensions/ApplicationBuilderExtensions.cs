using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using MoravianStar.DependencyInjection;
using MoravianStar.WebAPI.Middlewares;
using System;

namespace MoravianStar.WebAPI.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseMoravianStar(this IApplicationBuilder app, IWebHostEnvironment env, Action additionalSettings)
        {
            app.Use(async (httpContext, next) =>
            {
                using (new ServiceLocator(httpContext.RequestServices))
                {
                    await next();
                }
            });

            app.UseMiddleware<ExceptionMiddleware>(env);

            additionalSettings();
        }
    }
}