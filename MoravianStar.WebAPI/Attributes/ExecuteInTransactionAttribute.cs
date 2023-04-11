using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using MoravianStar.Dao;
using System;
using System.Threading.Tasks;

namespace MoravianStar.WebAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ExecuteInTransactionAsyncAttribute : TypeFilterAttribute
    {
        public ExecuteInTransactionAsyncAttribute() : base(typeof(ExecuteInTransactionAsyncImplAttribute<>))
        {
            DbContextType = Persistence.DefaultDbContext.GetType();
        }

        public ExecuteInTransactionAsyncAttribute(Type dbContextType) : base(typeof(ExecuteInTransactionAsyncImplAttribute<>))
        {
            DbContextType = dbContextType;
        }

        public Type DbContextType { get; }
    }

    public class ExecuteInTransactionAsyncImplAttribute<TDbContext> : IAsyncActionFilter
        where TDbContext : DbContext
    {
        private readonly IDbTransaction<TDbContext> dbTransaction;

        public ExecuteInTransactionAsyncImplAttribute(IDbTransaction<TDbContext> dbTransaction)
        {
            this.dbTransaction = dbTransaction;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
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
    }
}