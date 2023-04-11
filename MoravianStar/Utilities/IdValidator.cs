using MoravianStar.Extensions;
using System;

namespace MoravianStar.Utilities
{
    /// <summary>
    /// Utility for validation of Id
    /// </summary>
    public class IdValidator
    {
        /// <summary>
        /// Indicates whether the specified <paramref name="id"/> is <see langword="null"/> or empty.
        /// </summary>
        /// <typeparam name="TId">The type of the Id.</typeparam>
        /// <param name="id">The id to test.</param>
        /// <returns>Boolean result or throws <see cref="NotSupportedException"/> when the Id's type is not supported.</returns>
        /// <exception cref="NotSupportedException"></exception>
        public static bool IsNullOrEmpty<TId>(TId id)
        {
            bool result = false;

            switch (id)
            {
                case int i:
                    if (i == default)
                    {
                        result = true;
                    }
                    break;
                case Guid guid:
                    if (guid.IsNullOrEmpty())
                    {
                        result = true;
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }

            return result;
        }
    }
}