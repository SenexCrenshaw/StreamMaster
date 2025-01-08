using System.Collections.Concurrent;

using StreamMaster.Domain.Helpers;
using StreamMaster.Domain.XmltvXml;

namespace AutoMatchEPGTester
{
    /// <summary>
    /// IEpgMatcher that logs everything, compares group vs. global,
    /// uses 2-tier scoring, AND an extra tie-break for exact channelName matches.
    /// </summary>
    public static class EpgMatcher
    {
        /// <inheritdoc/>
        public static async Task<StationChannelName?> MatchAsync(
    string channelName,
    string channelEPGID,
    ConcurrentDictionary<int, List<StationChannelName>> StationChannelNames)
        {
            string inputName = channelName?.Trim().ToLowerInvariant() ?? string.Empty;
            Console.WriteLine($"[DEBUG] Input channel name: '{channelName}' => normalized to '{inputName}'");
            Console.WriteLine($"[DEBUG] Input EPG ID: {channelEPGID}");

            if (string.IsNullOrEmpty(inputName))
            {
                Console.WriteLine("[DEBUG] Input channel name is empty => returning null");
                return null;
            }

            // Parse the EPG ID to get epgNumber and stationId
            if (!EPGHelper.TryExtractEPGNumberAndStationId(channelEPGID, out int epgNumber, out string? stationId))
            {
                Console.WriteLine("[DEBUG] Could not parse stationId => fallback to global only");
                (StationChannelName? candidate, bool IsSuffixMatch) = await BestGlobalMatchAsync(inputName, StationChannelNames).ConfigureAwait(false);
                return candidate;
            }

            // Try to get group-specific station list
            StationChannelName? groupCandidate = null;
            int groupScore = int.MaxValue;
            bool isSuffix;
            if (StationChannelNames.TryGetValue(epgNumber, out List<StationChannelName>? groupList))
            {
                (groupCandidate, isSuffix, groupScore) = BestMatchInList(inputName, groupList, "[Group]");
            }

            // Aggregate all channels for global matching
            List<StationChannelName> allChannels = StationChannelNames.SelectMany(kvp => kvp.Value).ToList();
            (StationChannelName? globalCandidate, isSuffix, int globalScore) = BestMatchInList(inputName, allChannels, "[Global]");

            // Decide between group and global matches
            const int threshold = 10;
            bool groupOk = groupCandidate != null && groupScore < threshold;
            bool globalOk = globalCandidate != null && globalScore < threshold;

            if (groupOk && globalOk)
            {
                if (isSuffix == true)
                {
                    Console.WriteLine($"[DEBUG] Both group & global < threshold => choosing group with suffix match");
                    return groupCandidate;
                }
                if (isSuffix == true)
                {
                    Console.WriteLine($"[DEBUG] Both group & global < threshold => choosing global with suffix match");
                    return globalCandidate;
                }

                if (groupScore <= globalScore)
                {
                    Console.WriteLine($"[DEBUG] Both group & global < threshold => choosing group (score={groupScore})");
                    return groupCandidate;
                }
                else
                {
                    Console.WriteLine($"[DEBUG] Both group & global < threshold => choosing global (score={globalScore})");
                    return globalCandidate;
                }
            }

            if (groupOk)
            {
                Console.WriteLine($"[DEBUG] Only group < threshold => returning group (score={groupScore})");
                return groupCandidate;
            }

            if (globalOk)
            {
                Console.WriteLine($"[DEBUG] Only global < threshold => returning global (score={globalScore})");
                return globalCandidate;
            }

            Console.WriteLine("[DEBUG] No matches found => returning null");
            return null;
        }

        #region Private Helpers

        private static async Task<(StationChannelName? candidate, bool IsSuffixMatch)> BestGlobalMatchAsync(
     string inputName,
     ConcurrentDictionary<int, List<StationChannelName>> StationChannelNames)
        {
            // Flatten all channels
            List<StationChannelName> allChannels = StationChannelNames.SelectMany(kvp => kvp.Value).ToList();
            if (allChannels.Count == 0)
            {
                Console.WriteLine("[DEBUG] No global candidates found => returning null");
                return (null, false);
            }

            (StationChannelName? bestCandidate, bool IsSuffixMatch, int _) = BestMatchInList(inputName, allChannels, "[Global]");
            return await Task.FromResult((bestCandidate, IsSuffixMatch)).ConfigureAwait(false);
        }

        /// <summary>
        /// Scores the entire candidate list, logs each candidate's final & raw distance,
        /// then picks the best after sorting by (finalScore, rawDist).
        /// </summary>
        private static (StationChannelName? candidate, bool IsSuffixMatch, int finalScore) BestMatchInList(
    string inputName,
    List<StationChannelName> candidates,
    string tag)
        {
            if (candidates.Count == 0)
            {
                Console.WriteLine($"[DEBUG] {tag}: 0 candidates => returning null");
                return (null, false, int.MaxValue);
            }

            string[] suffixes = { "hd", "dt" };
            string normalizedInput = inputName.ToLowerInvariant();

            // Compute scores and detect suffix-based matches
            List<(StationChannelName Candidate, int FinalScore, int RawDist, bool IsSuffixMatch)> scored = candidates
                .Select(candidate =>
                {
                    bool isSuffixMatch = suffixes.Any(suffix =>
                        $"{normalizedInput}{suffix}" == candidate.ChannelName?.ToLowerInvariant());

                    if (!isSuffixMatch)
                    {
                        isSuffixMatch = suffixes.Any(suffix =>
                      $"{normalizedInput}{suffix}" == candidate.Channel?.ToLowerInvariant());
                    }

                    (int finalScore, int rawDist) = ComputeScores(normalizedInput, candidate);
                    if (isSuffixMatch)
                    {
                        int aaa = 1;
                    }
                    return (candidate, finalScore, rawDist, isSuffixMatch);
                })
                .ToList();

            // Log debug information
            Console.WriteLine($"[DEBUG] {tag} => Full candidate list scoring:");
            foreach ((StationChannelName Candidate, int FinalScore, int RawDist, bool IsSuffixMatch) in scored)
            {
                Console.WriteLine($"   ID:{Candidate.Id}, ChannelName:{Candidate.ChannelName}, DisplayName:{Candidate.DisplayName}, " +
                                  $"FinalScore={FinalScore}, RawDist={RawDist}, IsSuffixMatch={IsSuffixMatch}");
            }

            // Sort by suffix match, then final score, then raw distance
            List<(StationChannelName Candidate, int FinalScore, int RawDist, bool IsSuffixMatch)> sorted = scored
                .OrderByDescending(x => x.IsSuffixMatch) // Suffix match takes highest priority
                .ThenBy(x => x.FinalScore)             // Then final score
                .ThenBy(x => x.RawDist)                // Then raw distance
                .ToList();

            (StationChannelName Candidate, int FinalScore, int RawDist, bool IsSuffixMatch) best = sorted.First();

            Console.WriteLine($"[DEBUG] Best {tag} => ID:{best.Candidate.Id}, ChannelName:{best.Candidate.ChannelName}, " +
                              $"FinalScore={best.FinalScore}, RawDist={best.RawDist}, IsSuffixMatch={best.IsSuffixMatch}");

            return (best.Candidate, best.IsSuffixMatch, best.FinalScore);
        }

        /// <summary>
        /// Computes (finalScore, rawDistance) with partial-substring bonus and
        /// an extra "tie-break" for exact channel token matches.
        /// </summary>
        private static (int finalScore, int rawDist) ComputeScores(
    string inputName,
    StationChannelName candidate)
        {
            string normalizedInput = inputName.ToLowerInvariant();
            string disp = candidate.DisplayName?.ToLowerInvariant().Trim() ?? "";
            string chan = candidate.ChannelName?.ToLowerInvariant().Trim() ?? "";

            string[] inputTokens = TokenizeWithDebug("Input", normalizedInput);
            string[] dispTokens = TokenizeWithDebug($"Display:{candidate.Id}", disp);
            string[] chanTokens = TokenizeWithDebug($"Channel:{candidate.Id}", chan);

            int sumFinal = 0;
            int sumRaw = 0;
            const int substringBonus = 5;

            foreach (string inTok in inputTokens)
            {
                int bestFinalForTok = int.MaxValue;
                int bestRawForTok = int.MaxValue;

                foreach (string dtok in dispTokens.Concat(chanTokens))
                {
                    int dist = LevenshteinHelper.LevenshteinDistance(inTok, dtok);
                    int distWithBonus = dist;

                    if (dtok.Contains(inTok) || inTok.Contains(dtok))
                    {
                        distWithBonus -= substringBonus;
                    }

                    if (distWithBonus < 0)
                    {
                        distWithBonus = 0;
                    }

                    bestFinalForTok = Math.Min(bestFinalForTok, distWithBonus);
                    bestRawForTok = Math.Min(bestRawForTok, dist);
                }

                sumFinal += bestFinalForTok;
                sumRaw += bestRawForTok;
            }

            if (inputTokens.Length == 0)
            {
                return (999, 999);
            }

            int finalScore = sumFinal / inputTokens.Length;
            int rawDistance = sumRaw / inputTokens.Length;

            Console.WriteLine($"[DEBUG] => Candidate:{candidate.Id}, finalScore={finalScore}, rawDist={rawDistance}");
            return (finalScore, rawDistance);
        }

        /// <summary>
        /// Splits a string by punctuation, logging the tokens.
        /// </summary>
        private static string[] TokenizeWithDebug(string label, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                Console.WriteLine($"[DEBUG] Tokenize({label}): string is empty => no tokens");
                return Array.Empty<string>();
            }

            char[] seps = { ' ', '(', ')', '[', ']', '-', '+', ':', ',', '.', '\"', '\'' };
            string[] tokens = text.Split(seps, StringSplitOptions.RemoveEmptyEntries);

            Console.WriteLine($"[DEBUG] Tokenize({label}): => '{text}' => [{string.Join(", ", tokens)}]");
            return tokens;
        }

        #endregion Private Helpers
    }
}