namespace MoravianStar.Dao
{
    /// <summary>
    /// The base interface of a projection containing Id.
    /// </summary>
    /// <typeparam name="TId">The type of the Id.</typeparam>
    public interface IProjectionBase<TId> : IProjectionBase
    {
        TId Id { get; set; }
    }

    /// <summary>
    /// The base interface of a projection.
    /// </summary>
    public interface IProjectionBase
    {
    }
}