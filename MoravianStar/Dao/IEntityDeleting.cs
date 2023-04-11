using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoravianStar.Dao
{
    public interface IEntityDeleting<TEntity>
    {
        Task DeletingAsync(TEntity entity, IDictionary<string, object> additionalParameters = null);
    }
}