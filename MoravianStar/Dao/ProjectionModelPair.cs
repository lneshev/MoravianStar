namespace MoravianStar.Dao
{
    public class ProjectionModelPair<TProjection, TModel>
        where TProjection : class, IProjectionBase
        where TModel : class, IModelBase
    {
        public TProjection Projection { get; set; }
        public TModel Model { get; set; }
    }
}