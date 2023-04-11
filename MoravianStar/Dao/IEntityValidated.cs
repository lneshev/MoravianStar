using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoravianStar.Dao
{
    public interface IEntityValidated<TEntity>
    {
        Task ValidatedAsync(TEntity entity, TEntity originalEntity, IDictionary<string, object> additionalParameters = null);
    }
}