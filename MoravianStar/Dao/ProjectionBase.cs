using MoravianStar.Dao;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.Common.Core.Projections
{
    /// <summary>
    /// The base class of a projection containing Id.
    /// </summary>
    /// <typeparam name="TId">The type of the Id.</typeparam>
    public class ProjectionBase<TId> : IProjectionBase<TId>
    {
        [Key]
        public TId Id { get; set; }

        public virtual bool IsNew()
        {
            return Id == null || Id.Equals(default(TId));
        }
    }
}