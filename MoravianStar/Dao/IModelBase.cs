namespace MoravianStar.Dao
{
    /// <summary>
    /// The base interface of a model containing Id.
    /// </summary>
    /// <typeparam name="TId">The type of the Id.</typeparam>
    public interface IModelBase<TId> : IModelBase
    {
        TId Id { get; set; }
    }

    /// <summary>
    /// The base interface of a model.
    /// </summary>
    public interface IModelBase
    {
        public bool IsNew();
    }
}