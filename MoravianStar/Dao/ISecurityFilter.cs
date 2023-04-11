namespace MoravianStar.Dao
{
    /// <summary>
    /// An interface for applying a security on a given <see cref="FilterSorterBase{TEntity}"/>.
    /// </summary>
    public interface ISecurityFilter
    {
        /// <summary>
        /// Specifies if the security should be turned on or off.
        /// </summary>
        bool IsSecurityEnabled { get; set; }
    }
}