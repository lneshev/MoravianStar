using HotChocolate.Types;
using HotChocolate.Types.Descriptors;
using Microsoft.Extensions.DependencyInjection;
using MoravianStar.Dao;
using System;
using System.Reflection;

namespace MoravianStar.GraphQL.Attributes
{
    public class UseTransactionAttribute : ObjectFieldDescriptorAttribute
    {
        public UseTransactionAttribute()
        {
            DbContextType = Persistence.DefaultDbContextType;
        }

        public UseTransactionAttribute(Type dbContextType)
        {
            DbContextType = dbContextType;
        }

        public Type DbContextType { get; }

        public override void OnConfigure(IDescriptorContext context, IObjectFieldDescriptor descriptor, MemberInfo member)
        {
            descriptor.Extend().Definition.MiddlewareDefinitions.Add(new(next => async context =>
            {
                var serviceType = typeof(IDbTransaction<>).MakeGenericType(DbContextType);
                var dbTransaction = (IDbTransaction)context.Services.GetRequiredService(serviceType);

                await dbTransaction.BeginAsync();

                try
                {
                    await next(context);
                }
                catch (Exception e)
                {
                    await dbTransaction.RollbackAsync();
                    context.ReportError(e);
                    return;
                }

                await dbTransaction.CommitAsync();
            }));
        }
    }
}
