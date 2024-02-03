using System;

namespace MoravianStar.Extensions
{
    /// <summary>
    /// Extension methods that are related to operations with Guids.
    /// </summary>
    public static class GuidExtensions
    {
        /// <summary>
        /// Indicates whether the specified guid is <see langword="null"/> or empty guid.
        /// </summary>
        /// <param name="guid">The source guid.</param>
        /// <returns><see langword="true"/> if the value parameter is <see langword="null"/> or an empty guid; otherwise, <see langword="false"/>.</returns>
        public static bool IsNullOrEmpty(this Guid guid)
        {
            return guid == Guid.Empty;
        }

        /// <summary>
        /// Indicates whether the specified guid is <see langword="null"/> or empty guid.
        /// </summary>
        /// <param name="guid">The source guid.</param>
        /// <returns><see langword="true"/> if the value parameter is <see langword="null"/> or an empty guid; otherwise, <see langword="false"/>.</returns>
        public static bool IsNullOrEmpty(this Guid? guid)
        {
            return guid == null || !guid.HasValue || guid == Guid.Empty;
        }
    }
}