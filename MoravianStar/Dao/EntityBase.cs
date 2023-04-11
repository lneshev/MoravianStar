using System.ComponentModel.DataAnnotations;

namespace MoravianStar.Dao
{
    /// <summary>
    /// The base class of a database entity containing Id.
    /// </summary>
    /// <typeparam name="TId">The type of the Id.</typeparam>
    public abstract class EntityBase<TId> : IEntityBase<TId>, IProjectionBase<TId>
    {
        [Key]
        public virtual TId Id { get; set; }

        public virtual bool IsNew()
        {
            return Id == null || Id.Equals(default(TId));
        }
    }
}