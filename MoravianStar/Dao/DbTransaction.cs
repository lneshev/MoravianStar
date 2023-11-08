using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MoravianStar.Resources;
using System;
using System.Threading.Tasks;

namespace MoravianStar.Dao
{
    public class DbTransaction<TDbContext> : IDbTransaction<TDbContext>
        where TDbContext : DbContext
    {
        private IDbContextTransaction dbContextTransaction;

        public DbTransaction(TDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public TDbContext DbContext { get; }

        public async Task BeginAsync()
        {
            if (dbContextTransaction != null)
            {
                throw new InvalidOperationException(Strings.AnotherTransactionHasAlreadyStarted);
            }
            dbContextTransaction = await DbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (dbContextTransaction != null)
            {
                DbContext.SaveChanges();
                OnCommiting(EventArgs.Empty);
                await dbContextTransaction.CommitAsync();
                OnCommitted(EventArgs.Empty);
                await DisposeAsync();
            }
        }

        public async Task RollbackAsync()
        {
            if (dbContextTransaction != null)
            {
                await dbContextTransaction.RollbackAsync();
                await DisposeAsync();
            }
        }

        public void Dispose()
        {
            if (dbContextTransaction != null)
            {
                dbContextTransaction.Dispose();
                dbContextTransaction = null;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (dbContextTransaction != null)
            {
                await dbContextTransaction.DisposeAsync();
                dbContextTransaction = null;
            }
        }

        private void OnCommiting(EventArgs e)
        {
            Committing?.Invoke(this, e);
            Committing = null;
        }

        private void OnCommitted(EventArgs e)
        {
            Committed?.Invoke(this, e);
            Committed = null;
        }

        public event EventHandler Committing;
        public event EventHandler Committed;
    }
}