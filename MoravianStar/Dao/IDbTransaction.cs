using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace MoravianStar.Dao
{
    public interface IDbTransaction<out TDbContext> : IDbTransaction
        where TDbContext : DbContext
    {
        TDbContext DbContext { get; }
    }

    public interface IDbTransaction : IDisposable, IAsyncDisposable
    {
        Task BeginAsync();
        Task CommitAsync();
        Task RollbackAsync();

        void Begin();
        void Commit();
        void Rollback();

        event EventHandler Committing;
        event EventHandler Committed;
    }
}