namespace StreamMaster.EPG
{
    /// <summary>
    /// A minimal Levenshtein helper in the same file, with debugging.
    /// </summary>
    internal static class LevenshteinHelper
    {
        public static int LevenshteinDistance(string s1, string s2)
        {
            s1 ??= string.Empty;
            s2 ??= string.Empty;

            int len1 = s1.Length, len2 = s2.Length;
            if (len1 == 0)
            {
                return len2;
            }

            if (len2 == 0)
            {
                return len1;
            }

            int[,] matrix = new int[len1 + 1, len2 + 1];

            for (int i = 0; i <= len1; i++)
            {
                matrix[i, 0] = i;
            }

            for (int j = 0; j <= len2; j++)
            {
                matrix[0, j] = j;
            }

            for (int i = 1; i <= len1; i++)
            {
                for (int j = 1; j <= len2; j++)
                {
                    int cost = (s1[i - 1] == s2[j - 1]) ? 0 : 1;
                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost);
                }
            }
            return matrix[len1, len2];
        }
    }
}
