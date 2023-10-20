using HotChocolate.Types;
using HotChocolate.Types.Descriptors;
using MoravianStar.DependencyInjection;
using System.Reflection;

namespace MoravianStar.GraphQL.Attributes
{
    public class UseMoravianStarAttribute : ObjectFieldDescriptorAttribute
    {
        public override void OnConfigure(IDescriptorContext context, IObjectFieldDescriptor descriptor, MemberInfo member)
        {
            descriptor.Extend().Definition.MiddlewareDefinitions.Add(new(next => async context =>
            {
                using (new ServiceLocator(context.Services))
                {
                    await next(context);
                }
            }));
        }
    }
}