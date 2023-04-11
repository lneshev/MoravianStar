using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoravianStar.Dao
{
    public interface IEntitySaving<TEntity>
    {
        Task SavingAsync(TEntity entity, TEntity originalEntity, IDictionary<string, object> additionalParameters = null);
    }
}