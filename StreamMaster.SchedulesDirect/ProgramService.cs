using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;

namespace StreamMaster.SchedulesDirect
{
    public class ProgramService(ILogger<ProgramService> logger, IOptionsMonitor<SDSettings> intSettings, ISeriesImages seriesImages, ISportsImages sportsImages, ISchedulesDirectAPIService schedulesDirectAPI, IEPGCache<ProgramService> epgCache, ISchedulesDirectDataService schedulesDirectDataService)
        : IProgramService
    {
        private readonly SDSettings sdsettings = intSettings.CurrentValue;
        private List<string> programQueue = [];
        private ConcurrentBag<Programme> programResponses = [];
        private int processedObjects = 0;
        private readonly SemaphoreSlim semaphore = new(SchedulesDirect.MaxParallelDownloads);

        public async Task<bool> BuildAllProgramEntries(CancellationToken cancellationToken)
        {
            // Reset state
            programQueue = [];
            programResponses = [];

            ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
            ICollection<MxfProgram> toProcess = schedulesDirectData.Programs.Values;

            logger.LogInformation($"Entering BuildAllProgramEntries() for {toProcess.Count} programs.");

            // Populate queue for programs that need to be downloaded
            foreach (MxfProgram mxfProgram in toProcess)
            {
                processedObjects++;
                cancellationToken.ThrowIfCancellationRequested();

                if (!mxfProgram.extras.ContainsKey("md5"))
                {
                    continue;
                }

                if (epgCache.JsonFiles.ContainsKey(mxfProgram.extras["md5"]))
                {
                    // Try to load cached program
                    try
                    {
                        Programme sdProgram = JsonSerializer.Deserialize<Programme>(epgCache.GetAsset(mxfProgram.extras["md5"])) ?? throw new Exception("Deserialization failed.");
                        BuildMxfProgram(mxfProgram, sdProgram);
                    }
                    catch (Exception)
                    {
                        programQueue.Add(mxfProgram.ProgramId); // Add to queue if cache fails
                    }
                }
                else
                {
                    programQueue.Add(mxfProgram.ProgramId); // Add to queue if cache does not exist
                }
            }

            logger.LogDebug($"Found {processedObjects} cached program descriptions.");

            // Download and process programs concurrently
            List<Task> tasks = [];
            int numberOfBatches = (int)Math.Ceiling((double)programQueue.Count / SchedulesDirect.MaxQueries);

            for (int i = 0; i < numberOfBatches; i++)
            {
                int startIndex = i * SchedulesDirect.MaxQueries;
                tasks.Add(ProcessProgramBatchAsync(startIndex, cancellationToken));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
            ProcessProgramResponses();

            logger.LogInformation("Exiting BuildAllProgramEntries(). SUCCESS.");
            programQueue.Clear();
            programResponses.Clear();
            epgCache.SaveCache();
            return true;
        }

        private async Task ProcessProgramBatchAsync(int startIndex, CancellationToken cancellationToken)
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                logger.LogInformation($"Downloading program information {startIndex} of {programQueue.Count}");
                await DownloadProgramResponsesAsync(startIndex, cancellationToken);
            }
            finally
            {
                semaphore.Release();
            }
        }

        private async Task DownloadProgramResponsesAsync(int start, CancellationToken cancellationToken)
        {
            if (programQueue.Count - start < 1)
            {
                return;
            }

            string[] programsToRequest = programQueue.Skip(start).Take(SchedulesDirect.MaxQueries).ToArray();

            List<Programme>? responses = await GetProgramsAsync(programsToRequest, cancellationToken).ConfigureAwait(false);
            if (responses != null)
            {
                programResponses.AddRange(responses);
            }
        }

        private async Task<List<Programme>?> GetProgramsAsync(string[] programIds, CancellationToken cancellationToken)
        {
            try
            {
                DateTime dtStart = DateTime.Now;
                List<Programme>? ret = await schedulesDirectAPI.GetApiResponse<List<Programme>?>(APIMethod.POST, "programs", programIds, cancellationToken).ConfigureAwait(false);

                logger.LogDebug($"Successfully retrieved {ret?.Count}/{programIds.Length} program descriptions. ({DateTime.Now - dtStart:G})");
                return ret;
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to fetch program data. Error: {ex.Message}");
                return null;
            }
        }

        private void ProcessProgramResponses()
        {
            // Process request response
            foreach (Programme sdProgram in programResponses)
            {
                ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
                MxfProgram mxfProgram = schedulesDirectData.FindOrCreateProgram(sdProgram.ProgramId);

                // Build program from SD data
                BuildMxfProgram(mxfProgram, sdProgram);

                // Add program JSON to cache
                if (sdProgram.Md5 != null)
                {
                    mxfProgram.extras.AddOrUpdate("md5", sdProgram.Md5);
                    try
                    {
                        string jsonString = JsonSerializer.Serialize(sdProgram);
                        epgCache.AddAsset(sdProgram.Md5, jsonString);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Failed to cache program {sdProgram.ProgramId}: {ex.Message}");
                    }
                }
                else
                {
                    logger.LogInformation($"Did not cache program {sdProgram.ProgramId} due to missing Md5 hash.");
                }
            }
        }

        // Helper method: Build the full MxfProgram
        private void BuildMxfProgram(MxfProgram mxfProgram, Programme sdProgram)
        {
            // Set program flags
            SetProgramFlags(mxfProgram, sdProgram);

            // Populate title, description, etc.
            DetermineTitlesAndDescriptions(mxfProgram, sdProgram);

            // Determine additional info based on the program type (movie/series)
            if (mxfProgram.IsMovie)
            {
                DetermineMovieInfo(mxfProgram, sdProgram);
            }
            else
            {
                DetermineSeriesInfo(mxfProgram);
                DetermineEpisodeInfo(mxfProgram, sdProgram);
                CompleteEpisodeTitle(mxfProgram);
            }

            // Set content advisory and cast/crew
            DetermineContentAdvisory(mxfProgram, sdProgram);
            DetermineCastAndCrew(mxfProgram, sdProgram);

            // Additional program data like genres and teams (for sports)
            if (sdProgram.Genres != null && sdProgram.Genres.Length > 0)
            {
                mxfProgram.extras.AddOrUpdate("genres", sdProgram.Genres.Clone());
            }

            if (sdProgram.EventDetails?.Teams != null)
            {
                mxfProgram.extras.AddOrUpdate("teams", sdProgram.EventDetails.Teams.ConvertAll(team => team.Name));
            }
        }

        // Helper method: Determine the titles and descriptions for a program
        private static void DetermineTitlesAndDescriptions(MxfProgram mxfProgram, Programme sdProgram)
        {
            // Populate titles
            mxfProgram.Title = sdProgram.Titles?[0]?.Title120 ?? string.Empty;
            mxfProgram.EpisodeTitle = sdProgram.EpisodeTitle150;

            // Populate descriptions and language
            if (sdProgram.Descriptions != null)
            {
                // Get the language from the ShortDescription
                mxfProgram.ShortDescription = GetDescriptions(sdProgram.Descriptions.Description100, out string? shortDescriptionLang);

                // Get the language from the Description
                mxfProgram.Description = GetDescriptions(sdProgram.Descriptions.Description1000, out string? descriptionLang);

                // Populate the language: use ShortDescription language first, fallback to Description language if necessary
                if (!string.IsNullOrEmpty(shortDescriptionLang))
                {
                    mxfProgram.Language = shortDescriptionLang.ToLower();
                }
                else if (!string.IsNullOrEmpty(descriptionLang))
                {
                    mxfProgram.Language = descriptionLang.ToLower();
                }
            }

            mxfProgram.OriginalAirdate = sdProgram.OriginalAirDate.ToString();
        }

        // Helper method: Get descriptions from a program's data
        private static string GetDescriptions(List<ProgramDescription> descriptions, out string language)
        {
            string ret = string.Empty;
            language = string.Empty;

            if (descriptions == null)
            {
                return ret;
            }

            foreach (ProgramDescription description in descriptions)
            {
                if (description.DescriptionLanguage[..2] == CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)
                {
                    language = description.DescriptionLanguage;
                    ret = description.Description;
                    break;
                }

                if (description.DescriptionLanguage[..2].Equals("en", StringComparison.CurrentCultureIgnoreCase) || description.DescriptionLanguage.Equals("und", StringComparison.CurrentCultureIgnoreCase))
                {
                    language = description.DescriptionLanguage;
                    ret = description.Description;
                }
            }
            return ret;
        }

        // Helper methods for decoding movie information
        private static int DecodeMpaaRating(List<ProgramContentRating> sdProgramContentRatings)
        {
            int maxValue = 0;
            if (sdProgramContentRatings == null)
            {
                return maxValue;
            }

            foreach (ProgramContentRating rating in sdProgramContentRatings.Where(rating => rating.Body.StartsWith("Motion Picture Association")))
            {
                switch (rating.Code.ToLower().Replace("-", ""))
                {
                    case "g":
                        maxValue = Math.Max(maxValue, 1);
                        break;

                    case "pg":
                        maxValue = Math.Max(maxValue, 2);
                        break;

                    case "pg13":
                        maxValue = Math.Max(maxValue, 3);
                        break;

                    case "r":
                        maxValue = Math.Max(maxValue, 4);
                        break;

                    case "nc17":
                        maxValue = Math.Max(maxValue, 5);
                        break;
                }
            }

            return maxValue;
        }

        // Helper method to handle cast and crew population
        private List<MxfPersonRank>? GetPersons(List<ProgramPerson> persons, string[] roles)
        {
            if (persons == null)
            {
                return null;
            }

            List<MxfPersonRank> ret = [];
            ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();

            foreach (ProgramPerson? person in persons.Where(p => roles.Any(role => p.Role.Contains(role))))
            {
                ret.Add(new MxfPersonRank(schedulesDirectData.FindOrCreatePerson(person.Name))
                {
                    Rank = int.Parse(person.BillingOrder),
                    Character = person.CharacterName
                });
            }
            return ret;
        }

        private static void DetermineContentAdvisory(MxfProgram mxfProgram, Programme sdProgram)
        {
            ConcurrentHashSet<string> advisories = [];

            if (sdProgram.ContentAdvisory != null)
            {
                foreach (string reason in sdProgram.ContentAdvisory)
                {
                    advisories.Add(reason);
                }
            }

            if (advisories.Count == 0)
            {
                return;
            }

            string[] advisoryTable = [.. advisories];

            // Set flags for advisories
            mxfProgram.HasAdult = SDHelpers.TableContains(advisoryTable, "Adult Situations") || SDHelpers.TableContains(advisoryTable, "Dialog");
            mxfProgram.HasBriefNudity = SDHelpers.TableContains(advisoryTable, "Brief Nudity");
            mxfProgram.HasGraphicLanguage = SDHelpers.TableContains(advisoryTable, "Graphic Language");
            mxfProgram.HasGraphicViolence = SDHelpers.TableContains(advisoryTable, "Graphic Violence");
            mxfProgram.HasLanguage = SDHelpers.TableContains(advisoryTable, "Adult Language") || SDHelpers.TableContains(advisoryTable, "Language", true);
            mxfProgram.HasMildViolence = SDHelpers.TableContains(advisoryTable, "Mild Violence");
            mxfProgram.HasNudity = SDHelpers.TableContains(advisoryTable, "Nudity", true);
            mxfProgram.HasRape = SDHelpers.TableContains(advisoryTable, "Rape");
            mxfProgram.HasStrongSexualContent = SDHelpers.TableContains(advisoryTable, "Strong Sexual Content");
            mxfProgram.HasViolence = SDHelpers.TableContains(advisoryTable, "Violence", true);
        }

        private void DetermineEpisodeInfo(MxfProgram mxfProgram, Programme sdProgram)
        {
            if (sdProgram.EntityType != "Episode")
            {
                return;
            }

            mxfProgram.EpisodeNumber = int.Parse(mxfProgram.ProgramId[10..]);

            if (sdProgram.Metadata != null)
            {
                foreach (Dictionary<string, ProgramMetadataProvider> providers in sdProgram.Metadata)
                {
                    if (providers.TryGetValue("Gracenote", out ProgramMetadataProvider? provider))
                    {
                        mxfProgram.SeasonNumber = provider.SeasonNumber;
                        mxfProgram.EpisodeNumber = provider.EpisodeNumber;
                    }
                }
            }

            if (mxfProgram.SeasonNumber != 0)
            {
                ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
                mxfProgram.mxfSeason = schedulesDirectData.FindOrCreateSeason(mxfProgram.mxfSeriesInfo.SeriesId, mxfProgram.SeasonNumber, sdProgram.HasSeasonArtwork ? mxfProgram.ProgramId : null);
            }
        }

        private void CompleteEpisodeTitle(MxfProgram mxfProgram)
        {
            if (string.IsNullOrEmpty(mxfProgram.EpisodeTitle) && mxfProgram.ProgramId.StartsWith("EP"))
            {
                mxfProgram.EpisodeTitle = mxfProgram.Title;
            }

            string se = sdsettings.AlternateSEFormat ? "S{0}:E{1} " : "s{0:D2}e{1:D2} ";
            se = mxfProgram.SeasonNumber != 0 ? string.Format(se, mxfProgram.SeasonNumber, mxfProgram.EpisodeNumber) : mxfProgram.EpisodeNumber != 0 ? $"#{mxfProgram.EpisodeNumber} " : string.Empty;

            if (sdsettings.PrefixEpisodeTitle)
            {
                mxfProgram.EpisodeTitle = se + mxfProgram.EpisodeTitle;
            }

            if (sdsettings.PrefixEpisodeDescription)
            {
                mxfProgram.Description = se + mxfProgram.Description;
                if (!string.IsNullOrEmpty(mxfProgram.ShortDescription))
                {
                    mxfProgram.ShortDescription = se + mxfProgram.ShortDescription;
                }
            }

            if (sdsettings.AppendEpisodeDesc && mxfProgram.SeasonNumber != 0 && mxfProgram.EpisodeNumber != 0)
            {
                mxfProgram.Description += $" \u000D\u000ASeason {mxfProgram.SeasonNumber}, Episode {mxfProgram.EpisodeNumber}";
            }
            else if (sdsettings.AppendEpisodeDesc && mxfProgram.EpisodeNumber != 0)
            {
                mxfProgram.Description += $" \u000D\u000AProduction #{mxfProgram.EpisodeNumber}";
            }

            if (mxfProgram.extras.TryGetValue("multipart", out dynamic? value))
            {
                mxfProgram.EpisodeTitle += $" ({value})";
            }
        }

        private static int DecodeStarRating(List<ProgramQualityRating> sdProgramQualityRatings)
        {
            if (sdProgramQualityRatings == null)
            {
                return 0;
            }

            double maxValue = (from rating in sdProgramQualityRatings
                               where !string.IsNullOrEmpty(rating.MaxRating)
                               let numerator = double.Parse(rating.Rating, CultureInfo.InvariantCulture)
                               let denominator = double.Parse(rating.MaxRating, CultureInfo.InvariantCulture)
                               select numerator / denominator).Concat([0.0]).Max();

            return maxValue > 0.0 ? (int)((8.0 * maxValue) + 0.125) : 0;
        }

        private void SetProgramFlags(MxfProgram prg, Programme sd)
        {
            string[] types = [sd.EntityType, sd.ShowType];

            prg.IsAction = SDHelpers.TableContains(sd.Genres, "Action") || SDHelpers.TableContains(sd.Genres, "Adventure");
            prg.IsAdultOnly = SDHelpers.TableContains(sd.Genres, "Adults Only");
            prg.IsComedy = SDHelpers.TableContains(sd.Genres, "Comedy");
            prg.IsDocumentary = SDHelpers.TableContains(sd.Genres, "Documentary");
            prg.IsDrama = SDHelpers.TableContains(sd.Genres, "Drama");
            prg.IsEducational = SDHelpers.TableContains(sd.Genres, "Educational");
            prg.IsHorror = SDHelpers.TableContains(sd.Genres, "Horror");
            prg.IsIndy = SDHelpers.TableContains(sd.Genres, "Independent") || SDHelpers.TableContains(sd.Genres, "Indy");
            prg.IsKids = SDHelpers.TableContains(sd.Genres, "Children") || SDHelpers.TableContains(sd.Genres, "Kids");
            prg.IsMusic = SDHelpers.TableContains(sd.Genres, "Music") || SDHelpers.TableContains(types, "Music");
            prg.IsNews = SDHelpers.TableContains(sd.Genres, "News");
            prg.IsReality = SDHelpers.TableContains(sd.Genres, "Reality");
            prg.IsRomance = SDHelpers.TableContains(sd.Genres, "Romance") || SDHelpers.TableContains(sd.Genres, "Romantic");
            prg.IsScienceFiction = SDHelpers.TableContains(sd.Genres, "Science Fiction");
            prg.IsSoap = SDHelpers.TableContains(sd.Genres, "Soap");
            prg.IsThriller = SDHelpers.TableContains(sd.Genres, "Suspense") || SDHelpers.TableContains(sd.Genres, "Thriller");

            prg.IsMiniseries = SDHelpers.TableContains(types, "Miniseries");
            prg.IsMovie = sd.ProgramId.StartsWith("MV") || SDHelpers.TableContains(types, "Movie");
            prg.IsPaidProgramming = SDHelpers.TableContains(types, "Paid Program");
            prg.IsSeries = SDHelpers.TableContains(types, "Series") && !SDHelpers.TableContains(sd.Genres, "Sports talk");
            prg.IsSpecial = SDHelpers.TableContains(types, "Special");
            prg.IsSports = sd.ProgramId.StartsWith("SP") || SDHelpers.TableContains(types, "Event") || SDHelpers.TableContains(sd.Genres, "Sports talk");

            // Add the sports event to SportEvents for tracking
            if (prg.IsSports)
            {
                sportsImages.SportEvents.Add(prg);
            }
        }

        private static void DetermineMovieInfo(MxfProgram mxfProgram, Programme sdProgram)
        {
            // Fill MPAA rating
            mxfProgram.MpaaRating = DecodeMpaaRating(sdProgram.ContentRating);

            // Populate movie-specific attributes
            if (sdProgram.Movie != null)
            {
                // Set the release year and star rating based on the quality rating
                mxfProgram.Year = sdProgram.Movie.Year;
                mxfProgram.HalfStars = DecodeStarRating(sdProgram.Movie.QualityRating);
            }
            else if (!string.IsNullOrEmpty(mxfProgram.OriginalAirdate))
            {
                // If there's no specific movie info but there's an original airdate, extract the year
                mxfProgram.Year = int.Parse(mxfProgram.OriginalAirdate[..4]);
            }
        }

        private void DetermineSeriesInfo(MxfProgram mxfProgram)
        {
            ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();

            // For sports programs (identified by ProgramId starting with "SP"), create a series entry based on the title
            if (mxfProgram.ProgramId.StartsWith("SP"))
            {
                string name = mxfProgram.Title.Replace(' ', '_');
                mxfProgram.mxfSeriesInfo = schedulesDirectData.FindOrCreateSeriesInfo(name);
                seriesImages.SportsSeries.Add(name, mxfProgram.ProgramId); // Track sports series for artwork
            }
            else
            {
                // Create or retrieve series information for regular programs
                mxfProgram.mxfSeriesInfo = schedulesDirectData.FindOrCreateSeriesInfo(mxfProgram.ProgramId.Substring(2, 8), mxfProgram.ProgramId);

                // If it's a generic series (e.g., starts with "SH"), cache or update series information
                if (mxfProgram.ProgramId.StartsWith("SH"))
                {
                    if (epgCache.JsonFiles.ContainsKey(mxfProgram.ProgramId))
                    {
                        try
                        {
                            string? asset = epgCache.GetAsset(mxfProgram.ProgramId);
                            if (asset != null)
                            {
                                using StringReader reader = new(asset);
                                GenericDescription? cached = JsonSerializer.Deserialize<GenericDescription>(reader.ReadToEnd());

                                if (cached != null && cached.StartAirdate == null)
                                {
                                    cached.StartAirdate = mxfProgram.OriginalAirdate ?? string.Empty;
                                    string jsonString = JsonSerializer.Serialize(cached);
                                    epgCache.UpdateAssetJsonEntry(mxfProgram.ProgramId, jsonString);
                                }
                            }
                        }
                        catch
                        {
                            // Ignored
                        }
                    }
                    else
                    {
                        // Add new series entry to the cache if it doesn't already exist
                        GenericDescription newEntry = new()
                        {
                            Code = 0,
                            Description1000 = mxfProgram.IsGeneric ? mxfProgram.Description : null,
                            Description100 = mxfProgram.IsGeneric ? mxfProgram.ShortDescription : null,
                            StartAirdate = mxfProgram.OriginalAirdate ?? string.Empty
                        };

                        string jsonString = JsonSerializer.Serialize(newEntry);
                        epgCache.AddAsset(mxfProgram.ProgramId, jsonString);
                    }
                }
            }

            // If the series info is not set, use the program title
            mxfProgram.mxfSeriesInfo.Title ??= mxfProgram.Title;
        }

        private void DetermineCastAndCrew(MxfProgram prg, Programme sd)
        {
            if (sdsettings.ExcludeCastAndCrew)
            {
                return;
            }

            prg.ActorRole = GetPersons(sd.Cast, ["Actor", "Voice", "Judge", "Self"]);
            prg.DirectorRole = GetPersons(sd.Crew, ["Director"]);
            prg.GuestActorRole = GetPersons(sd.Cast, ["Guest"]);
            prg.HostRole = GetPersons(sd.Cast, ["Anchor", "Host", "Presenter", "Narrator", "Correspondent"]);
            prg.ProducerRole = GetPersons(sd.Crew, ["Executive Producer"]);
            prg.WriterRole = GetPersons(sd.Crew, ["Writer", "Story"]);
        }

        public void ClearCache()
        {
            epgCache.ResetCache();
        }

        public void ResetCache()
        {
            programQueue = [];
            programResponses = [];
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose of managed resources here
                semaphore?.Dispose(); // Dispose of the SemaphoreSlim if it's still active

                // Clear large collections to free memory
                programQueue?.Clear();
                programResponses = []; // Resetting it to an empty bag
            }
        }

        ~ProgramService()
        {
            Dispose(false);
        }
    }
}