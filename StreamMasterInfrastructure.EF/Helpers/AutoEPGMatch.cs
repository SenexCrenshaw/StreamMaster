using System.Diagnostics;
using System.Text.RegularExpressions;

namespace StreamMasterInfrastructureEF.Helpers
{
    internal static class AutoEPGMatch
    {
        private static readonly string[] CommonSuffixes = ["us", "gb", "dt", "hd"];

        private static string RemoveCommonSuffixes(string input)
        {
            foreach (string suffix in CommonSuffixes)
            {
                if (input.EndsWith(suffix))
                {
                    return input[..^suffix.Length];
                }
            }
            return input;
        }

        private static (string code, string name) ExtractComponentsFromProgrammeName(string programmeName)
        {
            Match match = Regex.Match(programmeName, @"\[(.*?)\]\s*([\w-]+)");
            string code = match.Success ? match.Groups[1].Value : "";
            string name = match.Success ? match.Groups[2].Value.Split([' ', '-'])[0] : "";

            return (code, name);
        }

        private static string NormalizeString(string input)
        {
            return input.ToLower();
        }

        public static int GetMatchingScore(string userTvgName, string programmeName)
        {
            Debug.WriteLine("---------- Start Matching Output ----------");

            string normalizedUserTvgName = NormalizeString(RemoveCommonSuffixes(userTvgName));
            string normalizedProgrammeName = NormalizeString(RemoveCommonSuffixes(programmeName));

            (string extractedProgrammeCode, string extractedProgrammeName) = ExtractComponentsFromProgrammeName(normalizedProgrammeName);

            Debug.WriteLine($"Normalized User TVG Name: {normalizedUserTvgName}");
            Debug.WriteLine($"Normalized Programme Name: {normalizedProgrammeName}");
            Debug.WriteLine($"Extracted Programme Code: {extractedProgrammeCode}");
            Debug.WriteLine($"Extracted Programme Name: {extractedProgrammeName}");

            int score = 0;
            score += MatchExactOrBase(normalizedUserTvgName, extractedProgrammeCode, extractedProgrammeName);
            score += MatchCallSign(normalizedUserTvgName, extractedProgrammeCode, extractedProgrammeName);
            score += MatchWordIntersection(normalizedUserTvgName, normalizedProgrammeName);

            Debug.WriteLine($"Total Score: {score}");
            Debug.WriteLine("----------- End Matching Output -----------");

            return score;
        }

        /// <summary>
        /// Matches the exact name or base name of the user TVG name with the program code or name.
        /// </summary>
        /// <param name="userTvgName">Normalized user TVG name.</param>
        /// <param name="programmeCode">Extracted program code from the program name.</param>
        /// <param name="programmeName">Extracted program name from the program name.</param>
        /// <returns>Score based on exact or base name match.</returns>
        private static int MatchExactOrBase(string userTvgName, string programmeCode, string programmeName)
        {
            if (userTvgName.Equals(programmeCode) || userTvgName.Equals(programmeName))
            {
                return 50;
            }
            return 0;
        }

        /// <summary>
        /// Matches the call sign extracted from the user TVG name with the program name or code.
        /// </summary>
        /// <param name="userTvgName">Normalized user TVG name.</param>
        /// <param name="programmeCode">Extracted program code from the program name.</param>
        /// <param name="programmeName">Extracted program name from the program name.</param>
        /// <returns>Score based on call sign match.</returns>
        private static int MatchCallSign(string userTvgName, string programmeCode, string programmeName)
        {
            // Extract call sign from userTvgName (expected to be within parentheses)
            Match match = Regex.Match(userTvgName, @"\((.*?)\)"); // Matches content within parentheses
            string callSign = match.Success ? match.Groups[1].Value.ToLower() : "";

            if (!string.IsNullOrEmpty(callSign) && (callSign.Equals(programmeCode) || callSign.Equals(programmeName)))
            {
                return 40;
            }

            return 0;
        }

        /// <summary>
        /// Calculates a score based on the intersection of words in the user TVG name and the program name.
        /// </summary>
        /// <param name="userTvgName">Normalized user TVG name.</param>
        /// <param name="programmeName">Normalized program name.</param>
        /// <returns>Score based on the number of intersecting words.</returns>
        private static int MatchWordIntersection(string userTvgName, string programmeName)
        {
            List<string> userTvgNameWords = [.. userTvgName.Split(' ')];
            List<string> programmeNameWords = [.. programmeName.Split(' ')];
            int intersectionCount = userTvgNameWords.Intersect(programmeNameWords).Count();

            return intersectionCount * 10;
        }
    }
}
