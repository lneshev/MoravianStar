using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoravianStar.Dao
{
    public interface IEntityValidating<TEntity>
    {
        Task ValidatingAsync(TEntity entity, TEntity originalEntity, IDictionary<string, object> additionalParameters = null);
    }
}