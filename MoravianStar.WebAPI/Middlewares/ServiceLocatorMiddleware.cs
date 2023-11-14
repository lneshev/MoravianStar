using Microsoft.AspNetCore.Http;
using MoravianStar.DependencyInjection;
using System.Threading.Tasks;

namespace MoravianStar.WebAPI.Middlewares
{
    /// <summary>
    /// A middleware that initializes the ServiceLocator with the scoped service provider.
    /// </summary>
    public class ServiceLocatorMiddleware
    {
        private readonly RequestDelegate next;

        public ServiceLocatorMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            new ServiceLocator(context.RequestServices);
            await next(context);
        }
    }
}