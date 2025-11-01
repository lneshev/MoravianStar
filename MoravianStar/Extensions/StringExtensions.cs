using System;

namespace MoravianStar.Extensions
{
    /// <summary>
    /// Extension methods that are related to operations with strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Slices a string to a maximum number of characters.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <param name="maxLength">The maximum number of characters that the new string should have.</param>
        /// <returns>A new <see langword="string"/> originating from the source value, but sliced up to the specified maximum length.</returns>
        public static string Truncate(this string value, int maxLength)
        {
            return string.IsNullOrEmpty(value) ? value : value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        /// <summary>
        /// Removes the specified suffix from the end of the string if it exists, using the provided <see cref="StringComparison"/>.
        /// </summary>
        /// <param name="value">The source string to operate on. If <c>null</c>, this method will attempt to evaluate and may throw a <see cref="NullReferenceException"/> if used as an instance call on <c>null</c>.</param>
        /// <param name="suffixToRemove">The suffix to remove from the end of <paramref name="value"/>. If <c>null</c> or empty, the original value is returned.</param>
        /// <param name="comparisonType">Specifies the type of comparison to use when checking for the suffix. Defaults to <see cref="StringComparison.CurrentCulture"/>.</param>
        /// <returns>
        /// The original string with the suffix removed if the string ended with the specified suffix (according to <paramref name="comparisonType"/>);
        /// otherwise, the original string is returned unchanged.
        /// </returns>
        public static string TrimEnd(this string value, string suffixToRemove, StringComparison comparisonType = StringComparison.CurrentCulture)
        {
            if (!string.IsNullOrEmpty(suffixToRemove) && value.EndsWith(suffixToRemove, comparisonType))
            {
                return value.Substring(0, value.Length - suffixToRemove.Length);
            }

            return value;
        }
    }
}