namespace MoravianStar.Dao
{
    /// <summary>
    /// The base interface of a database entity containing Id.
    /// </summary>
    /// <typeparam name="TId">The type of the Id.</typeparam>
    public interface IEntityBase<TId> : IEntityBase
    {
        TId Id { get; set; }
    }

    /// <summary>
    /// The base interface of a database entity.
    /// </summary>
    public interface IEntityBase
    {
        public bool IsNew();
    }
}