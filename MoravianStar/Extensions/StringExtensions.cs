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
    }
}