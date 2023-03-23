using System.Collections.Generic;

namespace MoravianStar.Dao
{
    public interface IModelListLoaded<TEntity, TModel>
    {
        void ListLoaded(List<DataPair<TEntity, TModel>> data);
    }
}