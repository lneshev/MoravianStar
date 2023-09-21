using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace MoravianStar.Dao
{
    public interface IDbTransaction<out TDbContext> : IAsyncDisposable
        where TDbContext : DbContext
    {
        TDbContext DbContext { get; }

        Task BeginAsync();
        Task CommitAsync();
        Task RollbackAsync();

        event EventHandler Committing;
        event EventHandler Committed;
    }
}