using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using MoravianStar.Dao;
using System;
using System.Threading.Tasks;

namespace MoravianStar.WebAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ExecuteInTransactionAsyncAttribute : Attribute, IAsyncActionFilter
    {
        public ExecuteInTransactionAsyncAttribute(bool isEnabled = true)
        {
            DbContextType = Persistence.DefaultDbContextType;
            IsEnabled = isEnabled;
        }

        public ExecuteInTransactionAsyncAttribute(Type dbContextType)
        {
            DbContextType = dbContextType;
            IsEnabled = true;
        }

        public Type DbContextType { get; }
        public bool IsEnabled { get; }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (IsEnabled)
            {
                var serviceType = typeof(IDbTransaction<>).MakeGenericType(DbContextType);
                var dbTransaction = (IDbTransaction)context.HttpContext.RequestServices.GetRequiredService(serviceType);

                await dbTransaction.BeginAsync();

                var executedContext = await next();

                if (executedContext.Exception == null)
                {
                    await dbTransaction.CommitAsync();
                }
                else
                {
                    await dbTransaction.RollbackAsync();
                }
            }
            else
            {
                await next();
            }
        }
    }
}