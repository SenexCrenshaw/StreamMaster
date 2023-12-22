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

        public static int GetMatchingScore(string userTvgName, string programmeName, bool debug = false)
        {
            if (debug)
            {
                Debug.WriteLine("---------- Start Matching Output ----------");
            }

            string normalizedUserTvgName = NormalizeString(RemoveCommonSuffixes(userTvgName));
            string normalizedProgrammeName = NormalizeString(RemoveCommonSuffixes(programmeName));

            (string extractedProgrammeCode, string extractedProgrammeName) = ExtractComponentsFromProgrammeName(normalizedProgrammeName);

            if (debug)
            {
                Debug.WriteLine($"Normalized User TVG Name: {normalizedUserTvgName}");
                Debug.WriteLine($"Normalized Programme Name: {normalizedProgrammeName}");
                Debug.WriteLine($"Extracted Programme Code: {extractedProgrammeCode}");
                Debug.WriteLine($"Extracted Programme Name: {extractedProgrammeName}");
            }
            int score = 0;
            score += MatchExactOrBase(normalizedUserTvgName, extractedProgrammeCode, extractedProgrammeName);
            score += MatchCallSign(normalizedUserTvgName, extractedProgrammeCode, extractedProgrammeName);
            score += MatchWordIntersection(normalizedUserTvgName, normalizedProgrammeName);

            if (debug)
            {
                Debug.WriteLine($"Total Score: {score}");
                Debug.WriteLine("----------- End Matching Output -----------");
            }

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
            int score = 0;
            if (userTvgName.Equals(programmeCode) || userTvgName.Equals(programmeName))
            {
                score += 50;
            }
            else if (userTvgName.Length > 2)
            {
                string userTvgNameTrimmed = userTvgName[..^2];
                if (userTvgNameTrimmed.Equals(programmeCode) || userTvgNameTrimmed.Equals(programmeName))
                {
                    score += 25; // Reduced score for trimmed match
                }
            }
            return score;
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
            int score = 0;
            string callSign = ExtractCallSign(userTvgName);

            if (!string.IsNullOrEmpty(callSign))
            {

                if (callSign.Equals(programmeCode) || callSign.Equals(programmeName))
                {
                    score += 40;
                }

                // Normalize call sign for comparison
                callSign = RemoveSuffixesFromCallSign(callSign);

                if (callSign.Equals(programmeCode) || callSign.Equals(programmeName))
                {
                    score += 40;
                }
            }
            return score;
        }

        private static string ExtractCallSign(string userTvgName)
        {
            Match match = Regex.Match(userTvgName, @"\((.*?)\)");
            string extractedCallSign = match.Success ? match.Groups[1].Value : "";

            // If no match in parentheses, attempt to extract call sign directly from the name
            if (string.IsNullOrEmpty(extractedCallSign))
            {
                string[] parts = userTvgName.Split(new[] { ' ', '-', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string part in parts)
                {
                    // Assuming call sign is the part that contains letters and possibly ends with -TV or -DT
                    if (part.Any(char.IsLetter) && (part.EndsWith("-TV") || part.EndsWith("-DT")))
                    {
                        return part;
                    }
                }
            }

            return extractedCallSign;
        }

        //private static string RemoveSuffixesFromCallSign(string callSign)
        //{
        //    // Remove common suffixes like -TV or -DT
        //    string[] suffixesToRemove = { "-TV", "-DT" };
        //    foreach (var suffix in suffixesToRemove)
        //    {
        //        if (callSign.EndsWith(suffix))
        //        {
        //            return callSign.Substring(0, callSign.Length - suffix.Length);
        //        }
        //    }
        //    return callSign;
        //}


        private static string RemoveSuffixesFromCallSign(string callSign)
        {

            // Check if the call sign has enough length before removing the last two characters
            if (callSign.Length > 2)
            {
                // Remove the last two characters
                callSign = callSign[..^2];
            }

            // Remove trailing hyphens if any
            callSign = callSign.TrimEnd('-');

            return callSign;
        }


        /// <summary>
        /// Calculates a score based on the intersection of words in the user TVG name and the program name.
        /// </summary>
        /// <param name="userTvgName">Normalized user TVG name.</param>
        /// <param name="programmeName">Normalized program name.</param>
        /// <returns>Score based on the number of intersecting words.</returns>
        private static int MatchWordIntersection(string userTvgName, string programmeName)
        {
            int score = CalculateIntersectionScore(userTvgName, programmeName);
            if (userTvgName.Length > 2)
            {
                string userTvgNameTrimmed = userTvgName[..^2];
                score += CalculateIntersectionScore(userTvgNameTrimmed, programmeName) / 2; // Half score for trimmed match
            }
            return score;
        }

        private static int CalculateIntersectionScore(string name1, string name2)
        {
            List<string> name1Words = name1.Split(' ').ToList();
            List<string> name2Words = name2.Split(' ').ToList();
            int intersectionCount = name1Words.Intersect(name2Words).Count();
            return intersectionCount * 10;
        }
    }
}
