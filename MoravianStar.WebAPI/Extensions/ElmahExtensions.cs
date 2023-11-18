using MoravianStar.Exceptions;
using System.Threading.Tasks;

namespace MoravianStar.WebAPI.Extensions
{
    public class ElmahExtensions
    {
        public static void RaiseLog(string message = null)
        {
            ElmahCore.ElmahExtensions.RaiseError(new ElmahCoreSuccessException(message), async (httpContext, error) =>
            {
                if (error.Exception != null && error.Exception is ElmahCoreSuccessException)
                {
                    error.StatusCode = 200;
                }
                await Task.CompletedTask;
            });
        }
    }
}