using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoravianStar.Dao
{
    public interface IEntitySaved<TEntity>
    {
        Task SavedAsync(TEntity entity, TEntity originalEntity, bool entityWasNew, IDictionary<string, object> additionalParameters = null);
    }
}