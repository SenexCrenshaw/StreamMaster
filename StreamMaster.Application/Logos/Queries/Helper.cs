namespace StreamMaster.Application.Logos.Queries
{
    internal static class Helper
    {
        /// <summary>
        /// Extracts the ID and filename from a Url with a constant prefix "/api/files/".
        /// </summary>
        /// <param name="url">The input Url.</param>
        /// <param name="id">The extracted ID as an integer.</param>
        /// <param name="filename">The extracted filename.</param>
        /// <returns>True if parsing was successful; otherwise, false.</returns>
        public static bool TryParseUrl(this string url, out int id, out string? filename)
        {
            const string prefix = "/api/files/";
            id = 0;
            filename = null;

            if (string.IsNullOrWhiteSpace(url) || !url.StartsWith(prefix))
            {
                return false;
            }

            string remaining = url[prefix.Length..];
            string[] parts = remaining.Split('/', 2); // Split into at most 2 parts

            // Ensure we have both ID and filename
            if (parts.Length == 2 && int.TryParse(parts[0], out id))
            {
                filename = parts[1];
                return true;
            }

            return false;
        }
    }
}
