using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;

using StreamMaster.Domain.Cache;
using StreamMaster.SchedulesDirect.Domain;

namespace StreamMaster.SchedulesDirect.Services;
public class ProgramService(
    ILogger<ProgramService> logger,
    HybridCacheManager<ProgramService> ProgramCache,
    IOptionsMonitor<SDSettings> sdSettings,
    HybridCacheManager<GenericDescription> descriptionCache,
    ISchedulesDirectAPIService schedulesDirectAPI,
    ISchedulesDirectDataService schedulesDirectDataService) : IProgramService, IDisposable
{

    private readonly ConcurrentDictionary<string, string> programChannelsToProcess = new();
    private readonly SemaphoreSlim semaphore = new(SDAPIConfig.MaxParallelDownloads);

    private int totalPrograms;
    private int processedPrograms;

    public async Task<bool> BuildProgramEntriesAsync(CancellationToken cancellationToken)
    {

        await FillChannelWithProgramsAsync(cancellationToken);

        if (programChannelsToProcess.Count == 0)
        {
            logger.LogWarning("No programs to process. Exiting.");
            return true;
        }

        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData;
        totalPrograms = schedulesDirectData.Programs.Values.Count;
        logger.LogInformation("Starting program processing. Total programs: {TotalPrograms}", totalPrograms);

        // Create parallel consumers that fetch and process data in real-time
        List<Task> consumerTasks = [];
        int threads = Math.Clamp(programChannelsToProcess.Values.Count, 1, SDAPIConfig.MaxParallelDownloads);

        for (int i = 0; i < SDAPIConfig.MaxParallelDownloads; i++)
        {
            consumerTasks.Add(Task.Run(() => FetchAndProcessProgramsAsync(cancellationToken), cancellationToken));
        }

        await Task.WhenAll(consumerTasks).ConfigureAwait(false);

        logger.LogInformation("Finished processing all programs. Total processed: {ProcessedPrograms}", processedPrograms);
        return true;
    }

    private bool HasArtWork(Programme sdProgram)
    {
        return sdProgram.HasEpisodeArtwork || sdProgram.HasSportsArtwork || sdProgram.HasSeasonArtwork || sdProgram.HasMovieArtwork || sdProgram.HasSeriesArtwork;
    }

    private async Task FillChannelWithProgramsAsync(CancellationToken cancellationToken)
    {
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData;

        await Parallel.ForEachAsync(schedulesDirectData.Programs.Values, cancellationToken, async (mxfProgram, ct) =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!string.IsNullOrEmpty(mxfProgram.ProgramId))
            {
                try
                {
                    Programme? sdProgram = await ProgramCache.GetAsync<Programme>(mxfProgram.ProgramId).ConfigureAwait(false); // JsonSerializer.Deserialize<Programme>(cachedJson);
                    if (mxfProgram.ProgramId.Equals("EP019254150003"))
                    {
                        int aa = 1;
                    }
                    if (sdProgram != null && !(mxfProgram.ArtWorks.Count == 0 && HasArtWork(sdProgram)))
                    {
                        if (sdProgram.Md5 == mxfProgram.MD5)
                        {
                            await UpdateProgramAsync(sdProgram);
                        }
                        else
                        {

                            programChannelsToProcess.TryAdd(mxfProgram.ProgramId, mxfProgram.ProgramId);
                        }
                    }
                    else
                    {
                        programChannelsToProcess.TryAdd(mxfProgram.ProgramId, mxfProgram.ProgramId);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning("Cache deserialization failed for Program ID {ProgramId}: {Error}", mxfProgram.ProgramId, ex.Message);
                }
            }
        });

    }

    private async Task FetchAndProcessProgramsAsync(CancellationToken cancellationToken)
    {
        const int batchSize = 100;

        List<string> currentBatch = [];

        foreach (string programId in programChannelsToProcess.Keys)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (programId == null || !programChannelsToProcess.TryRemove(programId, out _))
            {
                continue;
            }
            if (programId == "EP019254150003")
            {
                int aa = 1;
            }

            currentBatch.Add(programId);

            if (currentBatch.Count >= batchSize)
            {
                await ProcessBatchAsync(currentBatch, cancellationToken).ConfigureAwait(false);
                currentBatch.Clear();
            }
        }

        if (currentBatch.Count > 0)
        {
            await ProcessBatchAsync(currentBatch, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task ProcessBatchAsync(List<string> programIds, CancellationToken cancellationToken)
    {
        await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            // Fetch program data for the programIds
            List<Programme>? responses = await schedulesDirectAPI
                .GetProgramsAsync([.. programIds], cancellationToken)
                .ConfigureAwait(false);

            if (responses != null)
            {
                // Process each response in parallel
                await Task.WhenAll(responses.Select(response =>
                    Task.Run(() => ProcessSingleProgramAsync(response), cancellationToken)));
            }

            int processed = Interlocked.Add(ref processedPrograms, programIds.Count);
            if (processed % 100 == 0 || processed == totalPrograms)
            {
                logger.LogInformation("Processed {ProcessedPrograms}/{TotalPrograms} programs.", processed, totalPrograms);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process batch of {BatchCount} program IDs.", programIds.Count);
        }
        finally
        {
            semaphore.Release();
        }
    }

    private async Task ProcessSingleProgramAsync(Programme sdProgram)
    {
        if (sdProgram.ProgramId == "EP019254150003")
        {
            int aaa = 1;
        }
        await UpdateProgramAsync(sdProgram);

        if (!string.IsNullOrEmpty(sdProgram.Md5))
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(sdProgram);
                await ProgramCache.SetAsync(sdProgram.ProgramId, jsonString).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError("Failed to cache program {ProgramId}: {Error}", sdProgram.ProgramId, ex.Message);
            }
        }
        else
        {
            logger.LogInformation("Did not cache program {ProgramId} due to missing MD5 hash.", sdProgram.ProgramId);
        }
    }

    private async Task UpdateProgramAsync(Programme sdProgram)
    {
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData;

        if (sdProgram.ProgramId == "EP019254150003")
        {
            int aa = 2;
        }
        MxfProgram? mxfProgram = schedulesDirectData.FindProgram(sdProgram.ProgramId);
        //MxfProgram mxfProgram = schedulesDirectData.FindOrCreateProgram(sdProgram.ProgramId, sdProgram.Md5);
        if (mxfProgram == null)
        {
            logger.LogWarning("Program {ProgramId} not found in the data store.", sdProgram.ProgramId);
            return;
        }
        await BuildMxfProgramAsync(mxfProgram, sdProgram);
    }

    private async Task BuildMxfProgramAsync(MxfProgram mxfProgram, Programme sdProgram)
    {
        if (sdProgram.ProgramId == "EP019254150003")
        {
            int aa = 2;
        }
        //mxfProgram.MD5 = sdProgram.Md5;
        // Populate program properties from the sdProgram
        SetProgramFlags(mxfProgram, sdProgram);
        DetermineTitlesAndDescriptions(mxfProgram, sdProgram);

        if (mxfProgram.IsMovie)
        {
            DetermineMovieInfo(mxfProgram, sdProgram);
        }
        else
        {
            await DetermineSeriesInfoAsync(mxfProgram);
            DetermineEpisodeInfo(mxfProgram, sdProgram);
            CompleteEpisodeTitle(mxfProgram);
        }

        // Set content advisory and cast/crew
        DetermineContentAdvisory(mxfProgram, sdProgram);
        DetermineCastAndCrew(mxfProgram, sdProgram);

        // Additional program data like genres and teams (for sports)
        if (sdProgram.Genres?.Length > 0)
        {
            //mxfProgram.Extras.AddOrUpdate("genres", new List<string>(sdProgram.Genres));
            mxfProgram.Extras.AddOrUpdate("genres", sdProgram.Genres);
        }

        if (sdProgram.EventDetails?.Teams != null)
        {
            mxfProgram.Extras.AddOrUpdate("teams", sdProgram.EventDetails.Teams.ConvertAll(team => team.Name));
        }
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

    private void DetermineEpisodeInfo(MxfProgram mxfProgram, Programme sdProgram)
    {
        if (sdProgram.ProgramId == "EP019254150003")
        {
            int aaa = 1;
        }

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

        if (sdProgram.HasSeasonArtwork && mxfProgram.SeasonNumber != 0)
        {
            ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData;
            string seriesId = mxfProgram.ProgramId.Substring(2, 8);

            mxfProgram.mxfSeason = schedulesDirectData.FindOrCreateSeason(seriesId, mxfProgram.SeasonNumber, mxfProgram.ProgramId);
        }
    }

    private void CompleteEpisodeTitle(MxfProgram mxfProgram)
    {
        if (string.IsNullOrEmpty(mxfProgram.EpisodeTitle) && mxfProgram.ProgramId.StartsWith("EP"))
        {
            mxfProgram.EpisodeTitle = mxfProgram.Title;
        }

        string se = sdSettings.CurrentValue.AlternateSEFormat ? "S{0}:E{1} " : "s{0:D2}e{1:D2} ";
        se = mxfProgram.SeasonNumber != 0 ? string.Format(se, mxfProgram.SeasonNumber, mxfProgram.EpisodeNumber) : mxfProgram.EpisodeNumber != 0 ? $"#{mxfProgram.EpisodeNumber} " : string.Empty;

        if (sdSettings.CurrentValue.PrefixEpisodeTitle)
        {
            mxfProgram.EpisodeTitle = se + mxfProgram.EpisodeTitle;
        }

        if (sdSettings.CurrentValue.PrefixEpisodeDescription)
        {
            mxfProgram.Description = se + mxfProgram.Description;
            if (!string.IsNullOrEmpty(mxfProgram.ShortDescription))
            {
                mxfProgram.ShortDescription = se + mxfProgram.ShortDescription;
            }
        }

        if (sdSettings.CurrentValue.AppendEpisodeDesc && mxfProgram.SeasonNumber != 0 && mxfProgram.EpisodeNumber != 0)
        {
            mxfProgram.Description += $" \u000D\u000ASeason {mxfProgram.SeasonNumber}, Episode {mxfProgram.EpisodeNumber}";
        }
        else if (sdSettings.CurrentValue.AppendEpisodeDesc && mxfProgram.EpisodeNumber != 0)
        {
            mxfProgram.Description += $" \u000D\u000AProduction #{mxfProgram.EpisodeNumber}";
        }

        if (mxfProgram.Extras.TryGetValue("multipart", out dynamic? value))
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

    private static void SetProgramFlags(MxfProgram prg, Programme sd)
    {
        if (sd.Genres is null)
        {
            return;
        }

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
            //sportsImages.SportEvents.Add(prg);
        }
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
    private async Task DetermineSeriesInfoAsync(MxfProgram mxfProgram)
    {
        if (mxfProgram.ProgramId == "EP019254150003")
        {
            int aaa = 1;
        }

        if (string.IsNullOrEmpty(mxfProgram.mxfSeriesInfo.Title))
        {
            mxfProgram.mxfSeriesInfo.Title = mxfProgram.Title;
        }

        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData;

        // For sports programs (identified by ProgramId starting with "SP"), create a series entry based on the title
        if (mxfProgram.ProgramId.StartsWith("SP"))
        {
            //string name = mxfProgram.Title.Replace(' ', '_');
            //mxfProgram.mxfSeriesInfo = schedulesDirectData.FindOrCreateSeriesInfo(name);
            //seriesImages.SportsSeries.Add(name, mxfProgram.ProgramId); // Track sports series for artwork
        }
        else
        {

            //mxfProgram.mxfSeriesInfo = schedulesDirectData.FindOrCreateSeriesInfo(mxfProgram.ProgramId.Substring(2, 8), mxfProgram.ProgramId);

            if (mxfProgram.ProgramId.StartsWith("EP"))
            {
                if (sdSettings.CurrentValue.EpisodeAppendProgramDescription)
                {
                    string seriesId = $"SH{mxfProgram.mxfSeriesInfo.SeriesId}0000";
                    GenericDescription? cached = await descriptionCache.GetAsync<GenericDescription>(seriesId);

                    if (cached != null)
                    {
                        if (!string.IsNullOrEmpty(cached.Description1000))
                        {
                            mxfProgram.Description += cached.Description1000;
                        }
                        if (!string.IsNullOrEmpty(cached.Description100))
                        {
                            mxfProgram.ShortDescription += cached.Description100;
                        }
                    }
                }
            }
            //else
            //{
            //    if (sdSettings.CurrentValue.EpisodeAppendProgramDescription)
            //    {
            //        if (mxfProgram.ProgramId.StartsWith("SH"))
            //        {
            //            GenericDescription? cached = await descriptionCache.GetAsync<GenericDescription>(mxfProgram.ProgramId);

            //            if (cached != null && cached.StartAirdate == null)
            //            {
            //                cached.StartAirdate = mxfProgram.OriginalAirdate ?? string.Empty;
            //            }
            //        }
            //    }
            //}


            //else if (mxfProgram.ProgramId.StartsWith("EP"))
            //{
            //    string seriesId = $"SH{mxfProgram.ProgramId.Substring(2, 8)}0000";
            //    GenericDescription? cached = await descriptionCache.GetAsync<GenericDescription>(seriesId);

            //    if (cached != null && cached.StartAirdate == null)
            //    {
            //        mxfProgram.Description = cached.Description1000;
            //        mxfProgram.ShortDescription = cached.Description100;
            //    }


            //    else
            //    {
            //        // Add new series entry to the cache if it doesn't already exist
            //        //GenericDescription newEntry = new()
            //        //{
            //        //    Code = 0,
            //        //    Description1000 = mxfProgram.IsGeneric ? mxfProgram.Description : null,
            //        //    Description100 = mxfProgram.IsGeneric ? mxfProgram.ShortDescription : null,
            //        //    StartAirdate = mxfProgram.OriginalAirdate ?? string.Empty
            //        //};
            //        //string newEntryJson = JsonSerializer.Serialize(newEntry);
            //        //await hybridCache.SetAsync(mxfProgram.ProgramId, newEntryJson);
            //    }

            //}

        }

    }

    private void DetermineCastAndCrew(MxfProgram prg, Programme sd)
    {
        if (sdSettings.CurrentValue.ExcludeCastAndCrew)
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

    // Helper method to handle cast and crew population
    private List<MxfPersonRank>? GetPersons(List<ProgramPerson> persons, string[] roles)
    {
        if (persons == null)
        {
            return null;
        }

        List<MxfPersonRank> ret = [];
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData;

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

            if (description.DescriptionLanguage[..2].EqualsIgnoreCase("en") || description.DescriptionLanguage.EqualsIgnoreCase("und"))
            {
                language = description.DescriptionLanguage;
                ret = description.Description;
            }
        }
        return ret;
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
            semaphore.Dispose();
            programChannelsToProcess.Clear();
        }
    }

    public List<string> GetExpiredKeys()
    {
        return ProgramCache.GetExpiredKeysAsync().Result;
    }

    public void RemovedExpiredKeys(List<string>? keysToDelete = null) { }
}
