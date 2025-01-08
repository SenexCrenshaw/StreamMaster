
using StreamMaster.Domain.Helpers;
using StreamMaster.Domain.Models;
using StreamMaster.Domain.XmltvXml;
using StreamMaster.Streams.Domain.Interfaces;

namespace StreamMaster.EPG
{
    /// <summary>
    /// A helper class to compute fuzzy matches between strings.
    /// </summary>
    public static class FuzzyMatcher
    {
        /// <summary>
        /// Computes the Levenshtein distance between two strings. Lower is closer.
        /// </summary>
        /// <param name="source1">First string.</param>
        /// <param name="source2">Second string.</param>
        /// <returns>Levenshtein distance (0 = identical).</returns>
        public static int LevenshteinDistance(string source1, string source2)
        {
            source1 ??= string.Empty;
            source2 ??= string.Empty;

            int source1Length = source1.Length;
            int source2Length = source2.Length;

            int[,] matrix = new int[source1Length + 1, source2Length + 1];

            // If one string is empty, the distance is the length of the other
            if (source1Length == 0)
            {
                return source2Length;
            }
            if (source2Length == 0)
            {
                return source1Length;
            }

            // Initialize rows and columns
            for (int i = 0; i <= source1Length; i++)
            {
                matrix[i, 0] = i;
            }
            for (int j = 0; j <= source2Length; j++)
            {
                matrix[0, j] = j;
            }

            // Compute distances
            for (int i = 1; i <= source1Length; i++)
            {
                for (int j = 1; j <= source2Length; j++)
                {
                    int cost = (source2[j - 1] == source1[i - 1]) ? 0 : 1;

                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost
                    );
                }
            }

            return matrix[source1Length, source2Length];
        }

        /// <summary>
        /// Scores how close two strings are. Lower is better (distance).
        /// </summary>
        public static int Score(string s1, string s2)
        {
            return LevenshteinDistance(s1, s2);
        }
    }

    /// <summary>
    /// An IEpgMatcher that:
    /// 1) Tries a group match (by stationId).
    /// 2) Tries a global match.
    /// 3) Picks whichever has the lower distance (if under threshold).
    /// </summary>
    public sealed class EpgMatcher(ICacheManager cacheManager) : IEpgMatcher
    {
        /// <inheritdoc/>
        public async Task<StationChannelName?> MatchAsync(SMChannel channel, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Normalize input
            string inputName = channel.Name?.Trim().ToLowerInvariant() ?? string.Empty;
            Console.WriteLine($"[DEBUG] Input channel name: '{channel.Name}' => normalized to '{inputName}'");
            Console.WriteLine($"[DEBUG] Input EPG ID: {channel.EPGId}");

            if (string.IsNullOrEmpty(inputName))
            {
                Console.WriteLine("[DEBUG] Input channel name is empty, returning null.");
                return null;
            }

            // Gather all channels for potential global fallback
            List<StationChannelName> allChannels = cacheManager.StationChannelNames
                .SelectMany(kvp => kvp.Value)
                .ToList();

            // Attempt to parse out stationId
            if (!EPGHelper.TryExtractEPGNumberAndStationId(channel.EPGId, out int epgNumber, out string? stationId))
            {
                Console.WriteLine("[DEBUG] Could not extract epgNumber/stationId => global only.");
                return await BestGlobalMatchAsync(inputName, allChannels).ConfigureAwait(false);
            }

            // See if there's a channel list for epgNumber
            if (!cacheManager.StationChannelNames.TryGetValue(epgNumber, out List<StationChannelName>? groupList))
            {
                Console.WriteLine($"[DEBUG] No station list for epgNumber {epgNumber}, global only.");
                return await BestGlobalMatchAsync(inputName, allChannels).ConfigureAwait(false);
            }

            // Filter by stationId if present
            List<StationChannelName> groupMatches = Array.Empty<StationChannelName>().ToList();
            if (!string.IsNullOrEmpty(stationId))
            {
                groupMatches = groupList
                    .Where(x => x.Channel == stationId)
                    .ToList();
            }

            // Compare group vs. global side by side
            (StationChannelName? groupCandidate, int groupScore) = BestMatchInList(inputName, groupMatches, "[Group]");
            (StationChannelName? globalCandidate, int globalScore) = BestMatchInList(inputName, allChannels, "[Global]");

            // Decide which to return under threshold
            const int threshold = 10;
            bool groupOk = groupCandidate != null && groupScore < threshold;
            bool globalOk = globalCandidate != null && globalScore < threshold;

            if (groupOk && globalOk)
            {
                // Both are good => pick whichever has the lower score
                if (groupScore <= globalScore)
                {
                    Console.WriteLine($"[DEBUG] Both group & global ok; picking group with score={groupScore}");
                    return await Task.FromResult(groupCandidate).ConfigureAwait(false);
                }
                else
                {
                    Console.WriteLine($"[DEBUG] Both group & global ok; picking global with score={globalScore}");
                    return await Task.FromResult(globalCandidate).ConfigureAwait(false);
                }
            }
            if (groupOk)
            {
                Console.WriteLine($"[DEBUG] Only group is good => returning group with score={groupScore}");
                return await Task.FromResult(groupCandidate).ConfigureAwait(false);
            }
            if (globalOk)
            {
                Console.WriteLine($"[DEBUG] Only global is good => returning global with score={globalScore}");
                return await Task.FromResult(globalCandidate).ConfigureAwait(false);
            }

            // If neither under threshold, optionally pick whichever is less
            if (groupCandidate != null && globalCandidate != null)
            {
                // If you prefer to return null instead, that's possible. For now, we'll pick the "less bad."
                if (groupScore <= globalScore)
                {
                    Console.WriteLine($"[DEBUG] Neither < threshold; group is less => score={groupScore}");
                    return await Task.FromResult(groupCandidate).ConfigureAwait(false);
                }
                else
                {
                    Console.WriteLine($"[DEBUG] Neither < threshold; global is less => score={globalScore}");
                    return await Task.FromResult(globalCandidate).ConfigureAwait(false);
                }
            }

            // No matches at all
            Console.WriteLine("[DEBUG] No matches found => returning null.");
            return null;
        }

        #region Private Helpers

        /// <summary>
        /// Finds the single best match in a candidate list by computing the fuzzy score.
        /// </summary>
        private static (StationChannelName? candidate, int score) BestMatchInList(
            string inputName,
            List<StationChannelName> candidates,
            string tag)
        {
            if (candidates.Count == 0)
            {
                Console.WriteLine($"[DEBUG] {tag}: 0 candidates => returning null.");
                return (null, int.MaxValue);
            }

            List<(StationChannelName Candidate, int Score)> scored = candidates
                .Select(x => (Candidate: x, Score: ScoreCandidate(inputName, x)))
                .OrderBy(x => x.Score)
                .ToList();

            // Debug top 10
            foreach ((StationChannelName Candidate, int Score) in scored.Take(10))
            {
                Console.WriteLine($"[DEBUG] {tag} candidate => ID:{Candidate.Id}, Name:{Candidate.ChannelName}, Score={Score}");
            }

            (StationChannelName Candidate, int Score) best = scored.First();
            Console.WriteLine($"[DEBUG] Best {tag} match => {best.Candidate.ChannelName}, Score={best.Score}");
            return (best.Candidate, best.Score);
        }

        /// <summary>
        /// Performs a "best match" globally, ignoring threshold. 
        /// If you want threshold logic here, add it inside.
        /// </summary>
        private static async Task<StationChannelName?> BestGlobalMatchAsync(
            string inputName,
            IReadOnlyList<StationChannelName> allCandidates)
        {
            if (allCandidates.Count == 0)
            {
                Console.WriteLine("[DEBUG] No global candidates => returning null.");
                return null;
            }

            (StationChannelName candidate, int score) = BestMatchInList(inputName, allCandidates.ToList(), "[Global]");
            return await Task.FromResult(candidate).ConfigureAwait(false);
        }

        /// <summary>
        /// Token-based fuzzy scoring that also strips HD/DT. 
        /// Increases partial-substring bonus to -5, and clamps floor at 0.
        /// </summary>
        private static int ScoreCandidate(string inputName, StationChannelName candidate)
        {
            // 1) Remove HD/DT
            string normInput = RemoveHdDt(inputName);
            string disp = RemoveHdDt(candidate.DisplayName?.ToLowerInvariant().Trim() ?? string.Empty);
            string chan = RemoveHdDt(candidate.ChannelName?.ToLowerInvariant().Trim() ?? string.Empty);

            // 2) Tokenize
            string[] inputTokens = Tokenize(normInput);
            string[] dispTokens = Tokenize(disp);
            string[] chanTokens = Tokenize(chan);

            // 3) For each input token, find the best distance among all display or channel tokens
            //    Then sum or average across the input tokens
            const int partialBonus = 5;  // how much we reduce the distance if there's a partial substring match
            int totalDistance = 0;

            foreach (string tok in inputTokens)
            {
                int bestDistance = int.MaxValue;

                // Compare vs. display tokens
                foreach (string dtok in dispTokens)
                {
                    int d = FuzzyMatcher.Score(tok, dtok);
                    if (dtok.Contains(tok) || tok.Contains(dtok))
                    {
                        d -= partialBonus; // subtract bonus
                    }
                    if (d < bestDistance)
                    {
                        bestDistance = d;
                    }
                }

                // Compare vs. channel tokens
                foreach (string ctok in chanTokens)
                {
                    int d = FuzzyMatcher.Score(tok, ctok);
                    if (ctok.Contains(tok) || tok.Contains(ctok))
                    {
                        d -= partialBonus;
                    }
                    if (d < bestDistance)
                    {
                        bestDistance = d;
                    }
                }

                // clamp at 0 so we never go negative
                if (bestDistance < 0)
                {
                    bestDistance = 0;
                }

                totalDistance += bestDistance;
            }

            // 4) Compute average distance 
            if (inputTokens.Length == 0)
            {
                return 999; // if no tokens in input
            }

            int avgDistance = totalDistance / inputTokens.Length;
            return avgDistance;
        }

        /// <summary>
        /// Splits on spaces and punctuation into tokens.
        /// </summary>
        private static string[] Tokenize(string text)
        {
            return string.IsNullOrWhiteSpace(text)
                ? Array.Empty<string>()
                : text.Split(new char[]
            {
                ' ', '(', ')', '[', ']', '-', '+', ':', ',', '.', '\"', '\'',
            }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Removes "hd" or "dt" from the beginning or end repeatedly.
        /// </summary>
        private static string RemoveHdDt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            string[] suffixes = { "hd", "dt" };
            bool changed;
            do
            {
                changed = false;
                foreach (string suffix in suffixes)
                {
                    // prefix
                    if (value.StartsWith(suffix))
                    {
                        value = value[suffix.Length..].Trim();
                        changed = true;
                    }
                    // suffix
                    if (value.EndsWith(suffix))
                    {
                        value = value[..^suffix.Length].Trim();
                        changed = true;
                    }
                }
            }
            while (changed && value.Length >= 2);

            return value;
        }

        #endregion
    }
}
