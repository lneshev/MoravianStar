using HotChocolate;
using MoravianStar.Exceptions;
using MoravianStar.Extensions;
using System.Threading.Tasks;

namespace MoravianStar.GraphQL.Filters
{
    public class ElmahErrorFilter : IErrorFilter
    {
        public IError OnError(IError error)
        {
            ElmahCore.ElmahExtensions.RaiseError(
                error.Exception ?? new InvalidModelStateException(error.Message),
                async (c, e) =>
                {
                    e.StatusCode = e.Exception.GetHttpStatusCode();
                    await Task.CompletedTask;
                });

            return error;
        }
    }
}