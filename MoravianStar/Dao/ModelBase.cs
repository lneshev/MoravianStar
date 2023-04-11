using System.ComponentModel.DataAnnotations;

namespace MoravianStar.Dao
{
    /// <summary>
    /// The base class of a model containing Id.
    /// </summary>
    /// <typeparam name="TId">The type of the Id.</typeparam>
    public class ModelBase<TId> : IModelBase<TId>, IProjectionBase<TId>
    {
        [Key]
        public TId Id { get; set; }

        public virtual bool IsNew()
        {
            return Id == null || Id.Equals(default(TId));
        }
    }
}