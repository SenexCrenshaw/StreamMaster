using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Common;
using StreamMaster.Domain.Extensions;
using StreamMaster.SchedulesDirect.Data;
using StreamMaster.SchedulesDirect.Helpers;

using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;

namespace StreamMaster.SchedulesDirect;

public partial class SchedulesDirect
{
    private List<string> programQueue = [];
    private ConcurrentBag<Programme> programResponses = [];

    private bool BuildAllProgramEntries()
    {
        // reset counters
        programQueue = [];
        programResponses = [];
        sportsSeries = [];
        //sportsEvents = new List<MxfProgram>();
        //IncrementNextStage(mxf.ProgramsToProcess.Count);
        logger.LogInformation($"Entering BuildAllProgramEntries() for {totalObjects} programs.");

        // fill mxf programs with cached values and queue the rest
        programQueue = [];
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        ICollection<MxfProgram> toProcess = schedulesDirectData.Programs.Values;
        foreach (MxfProgram mxfProgram in toProcess)
        {
            if (!mxfProgram.extras.ContainsKey("md5"))
            {
                continue;
            }
            if (epgCache.JsonFiles.ContainsKey(mxfProgram.extras["md5"]))
            {
                try
                {
                    dynamic cache = epgCache.GetAsset(mxfProgram.extras["md5"]);
                    using StringReader reader = new(epgCache.GetAsset(mxfProgram.extras["md5"]));
                    Programme sdProgram = JsonSerializer.Deserialize<Programme>(reader.ReadToEnd()) ?? throw new Exception("Deserialization failed.");
                    BuildMxfProgram(mxfProgram, sdProgram);
                    //IncrementProgress();
                }
                catch (Exception ex)
                {
                    programQueue.Add(mxfProgram.ProgramId);
                }
            }
            else
            {
                programQueue.Add(mxfProgram.ProgramId);
            }
        }
        logger.LogDebug($"Found {processedObjects} cached program descriptions.");

        // maximum 5000 queries at a time
        if (programQueue.Count > 0)
        {
            Parallel.For(0, (programQueue.Count / MaxQueries) + 1, new ParallelOptions { MaxDegreeOfParallelism = MaxParallelDownloads }, i =>
            {
                DownloadProgramResponses(i * MaxQueries);
            });

            ProcessProgramResponses();
            if (processedObjects != totalObjects)
            {
                logger.LogWarning($"Failed to download and process {schedulesDirectData.ProgramsToProcess.Count - processedObjects} program descriptions.");
            }
        }
        logger.LogInformation("Exiting BuildAllProgramEntries(). SUCCESS.");
        programQueue = []; programResponses = [];
        return true;
    }

    private void DownloadProgramResponses(int start)
    {
        // reject 0 requests
        if (programQueue.Count - start < 1)
        {
            return;
        }

        // build the array of programs to request for
        string[] programs = new string[Math.Min(programQueue.Count - start, MaxQueries)];
        for (int i = 0; i < programs.Length; ++i)
        {
            programs[i] = programQueue[start + i];
        }

        // request programs from Schedules Direct
        List<Programme>? responses = GetProgramsAsync(programs, CancellationToken.None).Result;
        if (responses != null)
        {
            Parallel.ForEach(responses, (response) =>
            {
                programResponses.Add(response);
            });
        }
    }

    private void ProcessProgramResponses()
    {
        // process request response
        foreach (Programme sdProgram in programResponses)
        {
            //IncrementProgress();

            ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
            MxfProgram mxfProgram = schedulesDirectData.FindOrCreateProgram(sdProgram.ProgramId);

            // build a standalone program
            BuildMxfProgram(mxfProgram, sdProgram);

            // add JSON to cache
            if (sdProgram.Md5 != null)
            {
                mxfProgram.extras.AddOrUpdate("md5", sdProgram.Md5);

                try
                {
                    string jsonString = JsonSerializer.Serialize(sdProgram);
                    epgCache.AddAsset(sdProgram.Md5, jsonString);
                }
                catch
                {
                    // Ignored
                }
            }
            else
            {
                logger.LogInformation($"Did not cache program {mxfProgram.ProgramId} due to missing Md5 hash.");
            }
        }
    }


    private void BuildMxfProgram(MxfProgram mxfProgram, Programme sdProgram)
    {
        // set program flags
        SetProgramFlags(mxfProgram, sdProgram);

        // populate title, short title, description, and short description
        DetermineTitlesAndDescriptions(mxfProgram, sdProgram);

        // populate program keywords
        DetermineProgramKeywords(mxfProgram, sdProgram);

        // determine movie or series information
        if (mxfProgram.IsMovie)
        {
            DetermineMovieInfo(mxfProgram, sdProgram);
        }
        else
        {
            DetermineSeriesInfo(mxfProgram, sdProgram);
            DetermineEpisodeInfo(mxfProgram, sdProgram);
            CompleteEpisodeTitle(mxfProgram);
        }

        // set content reason flags
        DetermineContentAdvisory(mxfProgram, sdProgram);

        // populate the cast and crew
        DetermineCastAndCrew(mxfProgram, sdProgram);

        // populate stuff for xmltv

        if (sdProgram.Genres != null && sdProgram.Genres.Length > 0)
        {
            mxfProgram.extras.AddOrUpdate("genres", sdProgram.Genres.Clone());
        }

        if (sdProgram.EventDetails?.Teams != null)
        {
            mxfProgram.extras.AddOrUpdate("teams", sdProgram.EventDetails.Teams.Select(team => team.Name).ToList());
        }
    }

    private void DetermineTitlesAndDescriptions(MxfProgram mxfProgram, Programme sdProgram)
    {
        Setting setting = memoryCache.GetSetting();
        // populate titles
        if (sdProgram.Titles != null)
        {
            mxfProgram.Title = sdProgram.Titles[0].Title120;
        }
        else
        {
            logger.LogWarning($"Program {sdProgram.ProgramId} is missing required content.");
        }


        mxfProgram.EpisodeTitle = sdProgram.EpisodeTitle150;

        // populate descriptions and language
        if (sdProgram.Descriptions != null)
        {
            mxfProgram.ShortDescription = GetDescriptions(sdProgram.Descriptions.Description100, out string? lang);
            mxfProgram.Description = GetDescriptions(sdProgram.Descriptions.Description1000, out lang);

            // if short description is empty, not a movie, and append episode option is enabled
            // copy long description into short description
            if (setting.SDSettings.AppendEpisodeDesc && !mxfProgram.IsMovie && string.IsNullOrEmpty(mxfProgram.ShortDescription))
            {
                mxfProgram.ShortDescription = mxfProgram.Description;
            }

            // populate language
            if (!string.IsNullOrEmpty(lang))
            {
                mxfProgram.Language = lang.ToLower();
            }
        }

        mxfProgram.OriginalAirdate = sdProgram.OriginalAirDate.ToString();
    }

    private string GetDescriptions(List<ProgramDescription> descriptions, out string language)
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
                // optimal selection ... description language matches computer culture settings
                language = description.DescriptionLanguage;
                ret = description.Description;
                break;
            }

            if (description.DescriptionLanguage[..2].ToLower() == "en" || description.DescriptionLanguage.ToLower() == "und")
            {
                // without culture match above, english is acceptable alternate
                language = description.DescriptionLanguage;
                ret = description.Description;
            }
            else if (string.IsNullOrEmpty(ret))
            {
                // first language not of the same culture or english
                language = description.DescriptionLanguage;
                ret = description.Description;
            }
        }
        return ret;
    }

    private void SetProgramFlags(MxfProgram prg, Programme sd)
    {
        string[] types = [sd.EntityType, sd.ShowType];

        // transfer genres to mxf program
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

        // below flags are populated when creating the program in ProcessMd5ScheduleEntry(string md5)
        // prg.IsSeasonFinale
        // prg.IsSeasonPremiere
        // prg.IsSeriesFinale
        // prg.IsSeriesPremiere

        // transfer show types to mxf program
        //prg.IsLimitedSeries = null;
        prg.IsMiniseries = SDHelpers.TableContains(types, "Miniseries");
        prg.IsMovie = sd.ProgramId.StartsWith("MV") || SDHelpers.TableContains(types, "Movie");
        prg.IsPaidProgramming = SDHelpers.TableContains(types, "Paid Program");
        //prg.IsProgramEpisodic = null;
        //prg.IsSerial = null;
        prg.IsSeries = SDHelpers.TableContains(types, "Series") && !SDHelpers.TableContains(sd.Genres, "Sports talk");
        prg.IsShortFilm = SDHelpers.TableContains(types, "Short Film");
        prg.IsSpecial = SDHelpers.TableContains(types, "Special");
        prg.IsSports = sd.ProgramId.StartsWith("SP") ||
                       SDHelpers.TableContains(types, "Event") ||
                       SDHelpers.TableContains(sd.Genres, "Sports talk");

        // set isGeneric flag if programID starts with "SH", is a series, is not a miniseries, and is not paid programming
        if (prg.ProgramId.StartsWith("SH") && ((prg.IsSports && !SDHelpers.TableContains(types, "Sports")) ||
                                               (prg.IsSeries && !prg.IsMiniseries && !prg.IsPaidProgramming)))
        {
            prg.IsGeneric = true;
        }

        // queue up the sport event to get the event image
        if (SDHelpers.TableContains(types, "Event"))// && (sd.HasSportsArtwork | sd.HasEpisodeArtwork | sd.HasSeriesArtwork | sd.HasImageArtwork))
        {
            sportEvents.Add(prg);
        }
    }

    private void DetermineProgramKeywords(MxfProgram mxfProgram, Programme sdProgram)
    {
        // determine primary group of program
        KeywordGroupsEnum group = KeywordGroupsEnum.UNKNOWN;
        if (mxfProgram.IsMovie)
        {
            group = KeywordGroupsEnum.MOVIES;
        }
        else if (mxfProgram.IsPaidProgramming)
        {
            group = KeywordGroupsEnum.PAIDPROGRAMMING;
        }
        else if (mxfProgram.IsSports)
        {
            group = KeywordGroupsEnum.SPORTS;
        }
        else if (mxfProgram.IsKids)
        {
            group = KeywordGroupsEnum.KIDS;
        }
        else if (mxfProgram.IsEducational)
        {
            group = KeywordGroupsEnum.EDUCATIONAL;
        }
        else if (mxfProgram.IsNews)
        {
            group = KeywordGroupsEnum.NEWS;
        }
        else if (mxfProgram.IsMusic)
        {
            group = KeywordGroupsEnum.MUSIC;
        }
        else if (mxfProgram.IsSpecial)
        {
            group = KeywordGroupsEnum.SPECIAL;
        }
        else if (mxfProgram.IsReality)
        {
            group = KeywordGroupsEnum.REALITY;
        }
        else if (mxfProgram.IsSeries)
        {
            group = KeywordGroupsEnum.SERIES;
        }

        // build the keywords/categories
        if (group == KeywordGroupsEnum.UNKNOWN)
        {
            return;
        }
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        MxfKeywordGroup mxfKeyGroup = schedulesDirectData.FindOrCreateKeywordGroup(group);
        mxfProgram.mxfKeywords.Add(new MxfKeyword((int)group, mxfKeyGroup.Index, SchedulesDirectData.KeywordGroupsText[(int)group]));

        // add premiere categories as necessary
        if (mxfProgram.IsSeasonPremiere || mxfProgram.IsSeriesPremiere)
        {
            MxfKeywordGroup premiere = schedulesDirectData.FindOrCreateKeywordGroup(KeywordGroupsEnum.PREMIERES);
            mxfProgram.mxfKeywords.Add(new MxfKeyword(premiere.Index, premiere.Index, SchedulesDirectData.KeywordGroupsText[(int)KeywordGroupsEnum.PREMIERES]));
            if (mxfProgram.IsSeriesPremiere)
            {
                mxfProgram.mxfKeywords.Add(premiere.FindOrCreateKeyword("Series Premiere"));
            }
            else if (mxfProgram.IsSeasonPremiere)
            {
                mxfProgram.mxfKeywords.Add(premiere.FindOrCreateKeyword("Season Premiere"));
            }
        }
        else if (mxfProgram.extras.ContainsKey("premiere"))
        {
            if (group == KeywordGroupsEnum.MOVIES)
            {
                mxfProgram.mxfKeywords.Add(mxfKeyGroup.FindOrCreateKeyword("Premiere"));
            }
            else if (SDHelpers.TableContains(sdProgram.Genres, "miniseries"))
            {
                MxfKeywordGroup premiere = schedulesDirectData.FindOrCreateKeywordGroup(KeywordGroupsEnum.PREMIERES);
                mxfProgram.mxfKeywords.Add(new MxfKeyword(premiere.Index, premiere.Index, SchedulesDirectData.KeywordGroupsText[(int)KeywordGroupsEnum.PREMIERES]));
                mxfProgram.mxfKeywords.Add(premiere.FindOrCreateKeyword("Miniseries Premiere"));
            }
        }

        // now add the real categories
        if (sdProgram.Genres != null)
        {
            foreach (string genre in sdProgram.Genres)
            {
                if (genre == SchedulesDirectData.KeywordGroupsText[(int)group])
                {
                    continue;
                }

                mxfProgram.mxfKeywords.Add(mxfKeyGroup.FindOrCreateKeyword(genre));
            }
        }

        // ensure there is at least 1 category to present in category search
        if (mxfProgram.mxfKeywords.Count > 1)
        {
            return;
        }

        mxfProgram.mxfKeywords.Add(mxfKeyGroup.FindOrCreateKeyword("Uncategorized"));
    }

    private void DetermineMovieInfo(MxfProgram mxfProgram, Programme sdProgram)
    {
        // fill MPAA rating
        mxfProgram.MpaaRating = DecodeMpaaRating(sdProgram.ContentRating);

        // populate movie specific attributes
        if (sdProgram.Movie != null)
        {
            mxfProgram.Year = sdProgram.Movie.Year;
            mxfProgram.HalfStars = DecodeStarRating(sdProgram.Movie.QualityRating);
        }
        else if (!string.IsNullOrEmpty(mxfProgram.OriginalAirdate))
        {
            mxfProgram.Year = int.Parse(mxfProgram.OriginalAirdate[..4]);
        }
    }

    private void DetermineSeriesInfo(MxfProgram mxfProgram, Programme sdProgram)
    {
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        // for sports programs that start with "SP", create a series entry based on program title
        // this groups them all together as a series for recordings
        MxfSeriesInfo mxfSeriesInfo;
        if (mxfProgram.ProgramId.StartsWith("SP"))
        {
            string name = mxfProgram.Title.Replace(' ', '_');
            mxfSeriesInfo = schedulesDirectData.FindOrCreateSeriesInfo(name);
            sportsSeries.Add(name, mxfProgram.ProgramId);
        }
        else
        {
            // create a seriesInfo entry if needed
            mxfSeriesInfo = schedulesDirectData.FindOrCreateSeriesInfo(mxfProgram.ProgramId.Substring(2, 8), mxfProgram.ProgramId);
            if (mxfProgram.ProgramId.StartsWith("SH"))
            {
                // go ahead and create/update the cache entry as needed
                if (epgCache.JsonFiles.ContainsKey(mxfProgram.ProgramId))
                {
                    try
                    {
                        string? asset = epgCache.GetAsset(mxfProgram.ProgramId);
                        if (asset == null)
                        {
                            return;
                        }
                        using StringReader reader = new(asset);

                        GenericDescription? cached = JsonSerializer.Deserialize<GenericDescription>(reader.ReadToEnd());
                        if (cached == null)
                        {
                            return;
                        }
                        if (cached.StartAirdate == null)
                        {
                            cached.StartAirdate = mxfProgram.OriginalAirdate ?? string.Empty;
                            using StringWriter writer = new();
                            string jsonString = JsonSerializer.Serialize(cached);
                            epgCache.UpdateAssetJsonEntry(mxfProgram.ProgramId, jsonString);
                        }

                    }
                    catch
                    {
                        // ignored
                    }
                }
                else
                {
                    GenericDescription newEntry = new()
                    {
                        Code = 0,
                        Description1000 = mxfProgram.IsGeneric ? mxfProgram.Description : null,
                        Description100 = mxfProgram.IsGeneric ? mxfProgram.ShortDescription : null,
                        StartAirdate = mxfProgram.OriginalAirdate ?? string.Empty
                    };

                    using StringWriter writer = new();
                    string jsonString = JsonSerializer.Serialize(newEntry);
                    epgCache.AddAsset(mxfProgram.ProgramId, jsonString);

                }
            }
        }

        mxfSeriesInfo.Title ??= mxfProgram.Title;
        mxfProgram.mxfSeriesInfo = mxfSeriesInfo;
    }

    private void DetermineEpisodeInfo(MxfProgram mxfProgram, Programme sdProgram)
    {
        if (sdProgram.EntityType != "Episode")
        {
            return;
        }

        // use the last 4 numbers as a production number
        mxfProgram.EpisodeNumber = int.Parse(mxfProgram.ProgramId[10..]);

        if (sdProgram.Metadata != null)
        {
            // grab season and episode numbers if available
            foreach (Dictionary<string, ProgramMetadataProvider> providers in sdProgram.Metadata)
            {
                ProgramMetadataProvider provider = null;
                //if (config.TheTvdbNumbers)
                //{
                //    if (providers.TryGetValue("TheTVDB", out provider) || providers.TryGetValue("TVmaze", out provider))
                //    {
                //        if (provider.SeasonNumber == 0 && provider.EpisodeNumber == 0) continue;
                //    }
                //}
                if (provider == null && !providers.TryGetValue("Gracenote", out provider))
                {
                    continue;
                }

                mxfProgram.SeasonNumber = provider.SeasonNumber;
                mxfProgram.EpisodeNumber = provider.EpisodeNumber;
            }
        }

        // if there is a season number, create a season entry
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        if (mxfProgram.SeasonNumber != 0)
        {
            mxfProgram.mxfSeason = schedulesDirectData.FindOrCreateSeason(mxfProgram.mxfSeriesInfo.SeriesId, mxfProgram.SeasonNumber,
                sdProgram.HasSeasonArtwork ? mxfProgram.ProgramId : null);

            Setting setting = memoryCache.GetSetting();
            if (setting.SDSettings.AppendEpisodeDesc || setting.SDSettings.PrefixEpisodeDescription || setting.SDSettings.PrefixEpisodeTitle)
            {
                mxfProgram.mxfSeason.HideSeasonTitle = true;
            }
        }
    }

    private void CompleteEpisodeTitle(MxfProgram mxfProgram)
    {
        // by request, if there is no episode title, and the program is not generic, duplicate the program title in the episode title
        if (mxfProgram.ProgramId.StartsWith("EP") && string.IsNullOrEmpty(mxfProgram.EpisodeTitle))
        {
            mxfProgram.EpisodeTitle = mxfProgram.Title;
        }
        else if (string.IsNullOrEmpty(mxfProgram.EpisodeTitle))
        {
            return;
        }

        Setting setting = memoryCache.GetSetting();
        string se = setting.SDSettings.AlternateSEFormat ? "S{0}:E{1} " : "s{0:D2}e{1:D2} ";
        se = mxfProgram.SeasonNumber != 0
            ? string.Format(se, mxfProgram.SeasonNumber, mxfProgram.EpisodeNumber)
            : mxfProgram.EpisodeNumber != 0 ? $"#{mxfProgram.EpisodeNumber} " : string.Empty;

        // prefix episode title with season and episode numbers as configured
        if (setting.SDSettings.PrefixEpisodeTitle)
        {
            mxfProgram.EpisodeTitle = se + mxfProgram.EpisodeTitle;
        }

        // prefix episode description with season and episode numbers as configured
        if (setting.SDSettings.PrefixEpisodeDescription)
        {
            mxfProgram.Description = se + mxfProgram.Description;
            if (!string.IsNullOrEmpty(mxfProgram.ShortDescription))
            {
                mxfProgram.ShortDescription = se + mxfProgram.ShortDescription;
            }
        }

        // append season and episode numbers to the program description as configured
        if (setting.SDSettings.AppendEpisodeDesc)
        {
            // add space before appending season and episode numbers in case there is no short description
            if (mxfProgram.SeasonNumber != 0 && mxfProgram.EpisodeNumber != 0)
            {
                mxfProgram.Description += $" \u000D\u000ASeason {mxfProgram.SeasonNumber}, Episode {mxfProgram.EpisodeNumber}";
            }
            else if (mxfProgram.EpisodeNumber != 0)
            {
                mxfProgram.Description += $" \u000D\u000AProduction #{mxfProgram.EpisodeNumber}";
            }
        }

        // append part/parts to episode title as needed
        if (mxfProgram.extras.ContainsKey("multipart"))
        {
            mxfProgram.EpisodeTitle += $" ({mxfProgram.extras["multipart"]})";
        }
    }

    private void DetermineContentAdvisory(MxfProgram mxfProgram, Programme sdProgram)
    {
        // fill content ratings and advisories; set flags
        HashSet<string> advisories = [];
        if (sdProgram.ContentRating != null)
        {
            //var origins = !string.IsNullOrEmpty(config.RatingsOrigin) ? config.RatingsOrigin.Split(',') : new[] { RegionInfo.CurrentRegion.ThreeLetterISORegionName };
            string[] origins = new[] { RegionInfo.CurrentRegion.ThreeLetterISORegionName };
            Dictionary<string, string> contentRatings = [];
            if (SDHelpers.TableContains(origins, "ALL"))
            {
                foreach (ProgramContentRating rating in sdProgram.ContentRating)
                {
                    contentRatings.Add(rating.Body, rating.Code);
                }
            }
            else
            {
                foreach (string? origin in origins)
                {
                    foreach (ProgramContentRating? rating in sdProgram.ContentRating.Where(arg => arg.Country?.Equals(origin) ?? false))
                    {
                        contentRatings.Add(rating.Body, rating.Code);
                    }
                    if (contentRatings.Count > 0)
                    {
                        break;
                    }
                }
            }

            mxfProgram.extras.AddOrUpdate("ratings", contentRatings);

        }

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

        string[] advisoryTable = advisories.ToArray();

        // set flags
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

    private void DetermineCastAndCrew(MxfProgram prg, Programme sd)
    {
        Setting setting = memoryCache.GetSetting();
        if (setting.SDSettings.ExcludeCastAndCrew)
        {
            return;
        }

        prg.ActorRole = GetPersons(sd.Cast, new[] { "Actor", "Voice", "Judge", "Self" });
        prg.DirectorRole = GetPersons(sd.Crew, new[] { "Director" });
        prg.GuestActorRole = GetPersons(sd.Cast, new[] { "Guest" }); // "Guest Star", "Guest"
        prg.HostRole = GetPersons(sd.Cast, new[] { "Anchor", "Host", "Presenter", "Narrator", "Correspondent" });
        prg.ProducerRole = GetPersons(sd.Crew, new[] { "Executive Producer" }); // "Producer", "Executive Producer", "Co-Executive Producer"
        prg.WriterRole = GetPersons(sd.Crew, new[] { "Writer", "Story" }); // "Screenwriter", "Writer", "Co-Writer"
    }

    private List<MxfPersonRank> GetPersons(List<ProgramPerson> persons, string[] roles)
    {
        if (persons == null)
        {
            return null;
        }

        List<string> personName = [];
        List<MxfPersonRank> ret = [];
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        foreach (ProgramPerson? person in persons.Where(person => roles.Any(role => person.Role.ToLower().Contains(role.ToLower()) && !personName.Contains(person.Name))))
        {
            ret.Add(new MxfPersonRank(schedulesDirectData.FindOrCreatePerson(person.Name))
            {
                Rank = int.Parse(person.BillingOrder),
                Character = person.CharacterName
            });
            personName.Add(person.Name);
        }
        return ret;
    }

    private int DecodeMpaaRating(List<ProgramContentRating> sdProgramContentRatings)
    {
        if (sdProgramContentRatings == null)
        {
            return 0;
        }

        int maxValue = 0;
        foreach (ProgramContentRating? rating in sdProgramContentRatings.Where(rating => rating.Body.ToLower().StartsWith("motion picture association")))
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
                case "x":
                    maxValue = Math.Max(maxValue, 6);
                    break;
                case "nr":
                    maxValue = Math.Max(maxValue, 7);
                    break;
                case "ao":
                    maxValue = Math.Max(maxValue, 8);
                    break;
            }
        }
        return maxValue;
    }

    private int DecodeStarRating(List<ProgramQualityRating> sdProgramQualityRatings)
    {
        if (sdProgramQualityRatings == null)
        {
            return 0;
        }

        double maxValue = (from rating in sdProgramQualityRatings where !string.IsNullOrEmpty(rating.MaxRating) let numerator = double.Parse(rating.Rating, CultureInfo.InvariantCulture) let denominator = double.Parse(rating.MaxRating, CultureInfo.InvariantCulture) select numerator / denominator).Concat(new[] { 0.0 }).Max();

        // return rounded number of half stars in a 4 star scale
        return maxValue > 0.0 ? (int)((8.0 * maxValue) + 0.125) : 0;
    }
}
