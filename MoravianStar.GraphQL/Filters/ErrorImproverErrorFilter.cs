using HotChocolate;
using MoravianStar.Extensions;

namespace MoravianStar.GraphQL.Filters
{
    public class ErrorImproverErrorFilter : IErrorFilter
    {
        public IError OnError(IError error)
        {
            var ex = error.Exception;
            if (ex != null)
            {
                return ErrorBuilder.FromError(error)
                                   .SetMessage(ex.Message)
                                   .SetCode(ex.GetHttpStatusCode().ToString())
                                   .Build();
            }
            return error;
        }
    }
}