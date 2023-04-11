using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoravianStar.Dao
{
    public interface IGetOriginalEntity<TEntity>
    {
        Task<TEntity> GetAsync(TEntity entity, Func<TEntity, TEntity> defaultRetreiver, IDictionary<string, object> additionalParameters = null);
    }
}