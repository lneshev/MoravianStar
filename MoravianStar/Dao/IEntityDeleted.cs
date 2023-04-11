using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoravianStar.Dao
{
    public interface IEntityDeleted<TEntity>
    {
        Task DeletedAsync(TEntity entity, IDictionary<string, object> additionalParameters = null);
    }
}