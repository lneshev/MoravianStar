using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoravianStar.Dao
{
    public interface IEntityFilling<TEntity>
    {
        Task FillingAsync(TEntity entity, TEntity originalEntity, IDictionary<string, object> additionalParameters = null);
    }
}