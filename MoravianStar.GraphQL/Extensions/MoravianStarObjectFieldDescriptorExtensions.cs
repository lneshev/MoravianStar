using HotChocolate.Types;
using MoravianStar.DependencyInjection;

namespace MoravianStar.GraphQL.Extensions
{
    public static class MoravianStarObjectFieldDescriptorExtensions
    {
        public static IObjectFieldDescriptor UseMoravianStar(this IObjectFieldDescriptor descriptor)
        {
            descriptor.Extend().Definition.MiddlewareDefinitions.Add(new(next => async context =>
            {
                using (new ServiceLocator(context.Services))
                {
                    await next(context);
                }
            }));

            return descriptor;
        }
    }
}