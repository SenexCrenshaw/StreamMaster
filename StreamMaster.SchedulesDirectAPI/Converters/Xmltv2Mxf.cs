﻿using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Data;

using StreamMasterDomain.Common;

using System.Globalization;
using System.Text.RegularExpressions;

namespace StreamMaster.SchedulesDirectAPI.Converters;

public class Xmltv2Mxf(ILogger<Xmltv2Mxf> logger, ISchedulesDirectData schedulesDirectData) : IXmltv2Mxf
{
    private class SeriesEpisodeInfo
    {
        public string TmsId;
        public string Type;
        public string SeriesId;
        public int ProductionNumber;
        public int SeasonNumber;
        public int EpisodeNumber;
        public int PartNumber;
        public int NumberOfParts;
    }

    public XMLTV? ConvertToMxf(string filepath, int EPGId)
    {
        var xmltv = FileUtil.ReadXmlFile(filepath, typeof(XMLTV));

        return ConvertToMxf(xmltv, EPGId);
    }

    public XMLTV? ConvertToMxf(XMLTV xmltv, int EPGId)
    {
        //var lineupName = $"EPG{EPGId}";
        ////foreach (var channel in xmltv.Channels)
        ////{
        ////    channel.Id = $"{lineupName}-{channel.Id}";
        ////}
        if (!BuildLineupAndChannelServices(xmltv, EPGId) || !BuildScheduleEntries(xmltv))
        {
            return null;
        }


        BuildKeywords();
        return xmltv;
    }

    private bool BuildLineupAndChannelServices(XMLTV xmltv, int EPGId, string lineupName = "EPG123+ Default Lineup Name")
    {

        logger.LogInformation("Building lineup and channel services.");
        var mxfLineup = schedulesDirectData.FindOrCreateLineup(lineupName.ToUpper().Replace(" ", "-"), lineupName);

        foreach (var channel in xmltv.Channels)
        {
            var mxfService = schedulesDirectData.FindOrCreateService(channel.Id);
            mxfService.extras.Add("epgid", EPGId);
            if (string.IsNullOrEmpty(mxfService.CallSign))
            {
                // add "callsign" and "station name"
                mxfService.CallSign = channel.DisplayNames.Count > 0 ? (mxfService.Name = channel.DisplayNames[0]?.Text ?? channel.Id) : channel.Id;

                if (channel.DisplayNames.Count > 1)
                {
                    mxfService.Name = channel.DisplayNames[1]?.Text ?? mxfService.Name;
                }

                // add station logo if present
                if (channel.Icons.Count > 0)
                {
                    mxfService.mxfGuideImage = schedulesDirectData.FindOrCreateGuideImage(channel.Icons[0].Src);
                    mxfService.extras.Add("logo", new StationImage
                    {
                        Url = channel.Icons[0].Src,

                    });
                }

                // gather possible channel number(s)
                HashSet<string> lcns = [];
                foreach (var lcn in channel.Lcn)
                {
                    lcns.Add(lcn.Text);
                }

                foreach (var dn in channel.DisplayNames.Where(arg => Regex.Match(arg.Text, "^\\d*\\.?\\d+$").Success))
                {
                    lcns.Add(dn.Text);
                }

                // add service with channel numbers to lineup
                if (lcns.Count > 0)
                {
                    foreach (var lcn in lcns)
                    {
                        string[] numbers = lcn.Split('.');
                        mxfLineup.channels.Add(new MxfChannel(mxfLineup, mxfService, int.Parse(numbers[0]), int.Parse(numbers[1])));
                    }
                }
                else
                {
                    mxfLineup.channels.Add(new MxfChannel(mxfLineup, mxfService));
                }
            }
        }
        return true;
    }

    private bool BuildScheduleEntries(XMLTV xmltv)
    {
        logger.LogInformation("Building schedule entries and programs.");
        foreach (var program in xmltv.Programs)
        {
            var info = GetSeriesEpisodeInfo(program);

            var mxfService = schedulesDirectData.FindOrCreateService(program.Channel);
            var mxfProgram = schedulesDirectData.FindOrCreateProgram(DetermineProgramUid(program));
            if (mxfService.extras.ContainsKey("epgid") && !mxfProgram.extras.ContainsKey("epgid"))
            {
                mxfProgram.extras.Add("epgid", mxfService.extras["epgid"]);
            }

            if (mxfProgram.Title == null)
            {
                mxfProgram.IsAction = program.Categories.Any(arg => arg.Text.ToLower().Contains("action")) ||
                                      program.Categories.Any(arg => arg.Text.ToLower().Contains("adventure"));
                mxfProgram.IsAdultOnly = program.Categories.Any(arg => arg.Text.ToLower().Contains("adults only"));
                mxfProgram.IsComedy = program.Categories.Any(arg => arg.Text.ToLower().Contains("comedy"));
                mxfProgram.IsDocumentary = program.Categories.Any(arg => arg.Text.ToLower().Contains("documentary"));
                mxfProgram.IsDrama = program.Categories.Any(arg => arg.Text.ToLower().Contains("drama"));
                mxfProgram.IsEducational = program.Categories.Any(arg => arg.Text.ToLower().Contains("educational"));
                mxfProgram.IsHorror = program.Categories.Any(arg => arg.Text.ToLower().Contains("horror"));
                mxfProgram.IsIndy = program.Categories.Any(arg => arg.Text.ToLower().Contains("independent")) ||
                                    program.Categories.Any(arg => arg.Text.ToLower().Contains("indy"));
                mxfProgram.IsKids = program.Categories.Any(arg => arg.Text.ToLower().Contains("kids")) ||
                                    program.Categories.Any(arg => arg.Text.ToLower().Contains("children"));
                mxfProgram.IsMusic = program.Categories.Any(arg => arg.Text.ToLower().Contains("music"));
                mxfProgram.IsNews = program.Categories.Any(arg => arg.Text.ToLower().Contains("news"));
                mxfProgram.IsReality = program.Categories.Any(arg => arg.Text.ToLower().Contains("reality"));
                mxfProgram.IsRomance = program.Categories.Any(arg => arg.Text.ToLower().Contains("romance")) ||
                                       program.Categories.Any(arg => arg.Text.ToLower().Contains("romantic"));
                mxfProgram.IsScienceFiction = program.Categories.Any(arg => arg.Text.ToLower().Contains("science fiction"));
                mxfProgram.IsSoap = program.Categories.Any(arg => arg.Text.ToLower().Contains("soap"));
                mxfProgram.IsThriller = program.Categories.Any(arg => arg.Text.ToLower().Contains("suspense")) ||
                                        program.Categories.Any(arg => arg.Text.ToLower().Contains("thriller"));

                //mxfProgram.IsSeasonFinale = NOT PART OF XMLTV
                mxfProgram.IsSeasonPremiere = program.Premiere?.Text?.ToLower().Contains("season") ?? false;
                //mxfProgram.IsSeriesFinale = NOT PART OF XMLTV
                mxfProgram.IsSeriesPremiere = program.Premiere?.Text?.ToLower().Contains("series") ?? false;

                mxfProgram.IsLimitedSeries = program.Categories.Any(arg => arg.Text.ToLower().Contains("limited series"));
                mxfProgram.IsMiniseries = program.Categories.Any(arg => arg.Text.ToLower().Contains("miniseries"));
                mxfProgram.IsMovie = info.Type?.Equals("MV") ?? (program.Categories.Any(arg => arg.Text.ToLower().Contains("movie")) ||
                                                                program.Categories.Any(arg => arg.Text.ToLower().Equals("feature film")));
                mxfProgram.IsPaidProgramming = program.Categories.Any(arg => arg.Text.ToLower().Contains("paid programming"));
                mxfProgram.IsProgramEpisodic = program.Categories.Any(arg => arg.Text.ToLower().Contains("episodic"));
                mxfProgram.IsSerial = program.Categories.Any(arg => arg.Text.ToLower().Contains("serial"));
                mxfProgram.IsSeries = (info.SeasonNumber > 0 && info.EpisodeNumber > 0) || (program.Subtitles != null) || (program.Categories.Any(arg => arg.Text.ToLower().Equals("series")) &&
                                                                                                                           !program.Categories.Any(arg => arg.Text.ToLower().Contains("sports talk")));
                mxfProgram.IsShortFilm = program.Categories.Any(arg => arg.Text.ToLower().Equals("short film"));
                mxfProgram.IsSpecial = program.Categories.Any(arg => arg.Text.ToLower().Contains("special"));
                mxfProgram.IsSports = program.Categories.Any(arg => arg.Text.ToLower().Contains("sports event")) ||
                                      program.Categories.Any(arg => arg.Text.ToLower().Contains("sports non-event")) ||
                                      program.Categories.Any(arg => arg.Text.ToLower().Contains("team event")) ||
                                      program.Categories.Any(arg => arg.Text.ToLower().Contains("sports talk"));

                mxfProgram.Title = program.Titles.FirstOrDefault(arg => arg.Text != null).Text;
                if (info.NumberOfParts > 1)
                {
                    var partOfParts = $"({info.PartNumber}/{info.NumberOfParts})";
                    mxfProgram.Title = $"{mxfProgram.Title.Replace(partOfParts, "")} {partOfParts}";
                }
                mxfProgram.EpisodeTitle = program.SubTitles?.FirstOrDefault(arg => arg.Text != null)?.Text;
                mxfProgram.Description = program.Descriptions?.FirstOrDefault(arg => arg.Text != null)?.Text;
                //mxfProgram.ShortDescription = NOT PART OF XMLTV
                mxfProgram.Language = program.Language?.Text ?? program.Titles?.FirstOrDefault(arg => arg.Text != null)?.Language;
                mxfProgram.Year = mxfProgram.IsMovie ? int.Parse(program.Date?[..4] ?? "0") : 0;
                mxfProgram.SeasonNumber = info.SeasonNumber;
                mxfProgram.EpisodeNumber = info.EpisodeNumber;
                mxfProgram.OriginalAirdate = !mxfProgram.IsMovie && (program.Date?.Length ?? 0) >= 8 ? $"{program.Date[..4]}-{program.Date.Substring(4, 2)}-{program.Date.Substring(6, 2)}" : !mxfProgram.IsMovie ? "1970-01-01" : $"{DateTime.MinValue}";
                mxfProgram.HalfStars = mxfProgram.IsMovie ? DetermineHalfStarRatings(program) : 0;
                mxfProgram.MpaaRating = mxfProgram.IsMovie ? DetermineMpaaRatings(program) : 0;

                //mxfProgram.Keywords =
                DetermineProgramKeywords(mxfProgram, program.Categories.Select(arg => arg.Text).ToArray());

                //mxfProgram.mxfGuideImage =
                DetermineGuideImage(mxfProgram, program);

                // advisories
                //mxfProgram.HasAdult =
                //mxfProgram.HasBriefNudity =
                //mxfProgram.HasGraphicLanguage =
                //mxfProgram.HasGraphicViolence =
                //mxfProgram.HasLanguage =
                //mxfProgram.HasMildViolence =
                //mxfProgram.HasNudity =
                //mxfProgram.HasRape =
                //mxfProgram.HasStrongSexualContent =
                //mxfProgram.HasViolence =
                DetermineRatingAdvisories(mxfProgram, program);

                // credits
                //mxfProgram.ActorRole =
                //mxfProgram.WriteRole =
                //mxfProgram.GuestActorRole =
                //mxfProgram.HostRole =
                //mxfProgram.ProducerRole =
                //mxfProgram.DirectorRole =
                DetermineCastAndCrewCredits(mxfProgram, program);

                //mxfProgram.IsGeneric =
                //mxfProgram.Season =
                //mxfProgram.Series =
                if (!string.IsNullOrEmpty(info.TmsId))
                {
                    if ((info.Type?.Equals("SH") ?? false) && (mxfProgram.IsSeries || mxfProgram.IsSports) && !mxfProgram.IsMiniseries && !mxfProgram.IsPaidProgramming)
                    {
                        mxfProgram.IsGeneric = true;
                    }

                    if (!mxfProgram.IsMovie)
                    {
                        mxfProgram.mxfSeriesInfo = schedulesDirectData.FindOrCreateSeriesInfo(info.SeriesId);
                        if (string.IsNullOrEmpty(mxfProgram.mxfSeriesInfo.Title))
                        {
                            mxfProgram.mxfSeriesInfo.Title = mxfProgram.Title;
                        }

                        if (info.SeasonNumber > 0)
                        {
                            mxfProgram.mxfSeason = schedulesDirectData.FindOrCreateSeason(info.SeriesId, info.SeasonNumber, info.TmsId);
                        }
                    }
                }
                else if (mxfProgram.IsSeries || mxfProgram.IsSports || program.New != null || program.PreviouslyShown != null)
                {
                    mxfProgram.mxfSeriesInfo = schedulesDirectData.FindOrCreateSeriesInfo(mxfProgram.Title);
                    if (string.IsNullOrEmpty(mxfProgram.mxfSeriesInfo.Title))
                    {
                        mxfProgram.mxfSeriesInfo.Title = mxfProgram.Title;
                    }

                    if (info.SeasonNumber == 0 && mxfProgram.EpisodeTitle != null)
                    {
                        mxfProgram.IsGeneric = true;
                    }
                }
                else if (!mxfProgram.IsMovie)
                {
                    mxfProgram.IsGeneric = true;
                }
            }

            var dtStart = DateTime.ParseExact(program.Start, "yyyyMMddHHmmss zzz", CultureInfo.InvariantCulture).ToUniversalTime();
            mxfService.MxfScheduleEntries.ScheduleEntry.Add(new MxfScheduleEntry
            {
                mxfProgram = mxfProgram,

                StartTime = dtStart,
                Duration = (int)(DateTime.ParseExact(program.Stop, "yyyyMMddHHmmss zzz", CultureInfo.InvariantCulture).ToUniversalTime() - dtStart).TotalSeconds,
                IsCc = program.Subtitles.Any(arg => arg.Type.Equals("teletext", StringComparison.OrdinalIgnoreCase)),
                IsSigned = program.Subtitles.Any(arg => arg.Type.Equals("deaf-signed", StringComparison.OrdinalIgnoreCase)),
                AudioFormat = DetermineAudioFormat(program),
                IsLive = program.Live != null,
                IsLiveSports = program.Live != null && mxfProgram.IsSports,
                //IsTape = NOT PART OF XMLTV
                //IsDelay = NOT PART OF XMLTV
                IsSubtitled = program.Subtitles.Any(arg => arg.Type.Equals("onscreen", StringComparison.OrdinalIgnoreCase)),
                IsPremiere = program.Premiere != null,
                //IsFinale = NOT PART OF XMLTV
                //IsInProgress = NOT PART OF XMLTV
                //IsSap = NOT PART OF XMLTV
                //IsBlackout = NOT PART OF XMLTV
                //IsEnhanced = NOT PART OF XMLTV
                //Is3D = NOT PART OF XMLTV
                //IsLetterbox = NOT PART OF XMLTV
                IsHdtv = program.Video?.Quality.ToLower().Contains("hd") ?? false,
                //IsHdtvSimulcast = NOT PART OF XMLTV
                //IsDvs = NOT PART OF XMLTV
                Part = info.NumberOfParts > 1 ? info.PartNumber : 0,
                Parts = info.NumberOfParts > 1 ? info.NumberOfParts : 0,
                TvRating = DetermineTvRatings(program),
                //IsClassroom = NOT PART OF XMLTV
                IsRepeat = !mxfProgram.IsMovie && program.PreviouslyShown != null
            });
        }
        return true;
    }

    private SeriesEpisodeInfo GetSeriesEpisodeInfo(XmltvProgramme xmltvProgramme)
    {
        var ret = new SeriesEpisodeInfo();
        foreach (var epNum in xmltvProgramme.EpisodeNums)
        {
            if (epNum.System == null)
            {
                continue;
            }

            switch (epNum.System.ToLower())
            {
                case "dd_progid":
                    var m = Regex.Match(epNum.Text, @"(MV|SH|EP|SP)[0-9]{8}.[0-9]{4}");
                    if (m.Length > 0)
                    {
                        ret.TmsId = epNum.Text.ToUpper().Replace(".", "_");
                        ret.Type = ret.TmsId[..2];
                        ret.SeriesId = ret.TmsId.Substring(2, 8);
                        ret.ProductionNumber = int.Parse(ret.TmsId.Substring(11, 4));
                    }
                    break;
                case "xmltv_ns":
                    var se1 = epNum.Text.Split('.');
                    _ = int.TryParse(se1[0].Split('/')[0], out ret.SeasonNumber);
                    ++ret.SeasonNumber;
                    _ = int.TryParse(se1[1].Split('/')[0], out ret.EpisodeNumber);
                    ++ret.EpisodeNumber;
                    _ = int.TryParse(se1[2].Split('/')[0], out ret.PartNumber);
                    ++ret.PartNumber;
                    if (!se1[2].Contains("/") || !int.TryParse(se1[2].Split('/')[1], out ret.NumberOfParts))
                    {
                        ret.NumberOfParts = 1;
                    }
                    break;
                case "sxxexx":
                case "onscreen":
                case "common":
                    var se2 = epNum.Text.ToLower()[1..].Split('e');
                    if (se2.Length == 2)
                    {
                        if (ret.SeasonNumber == 0)
                        {
                            _ = int.TryParse(se2[0], out ret.SeasonNumber);
                        }

                        if (ret.EpisodeNumber == 0)
                        {
                            _ = int.TryParse(se2[1], out ret.EpisodeNumber);
                        }
                    }
                    break;
            }
        }
        return ret;
    }

    private static string DetermineProgramUid(XmltvProgramme program)
    {
        var ret = program.EpisodeNums?.FirstOrDefault(arg => arg.System?.Equals("dd_progid", StringComparison.OrdinalIgnoreCase) ?? false)?.Text;
        if (ret != null)
        {
            return ret;
        }

        var hash = program.Titles.FirstOrDefault(arg => arg.Text != null)?.GetHashCode() ?? 0;
        hash = (hash * 397) ^ (program.SubTitles?.FirstOrDefault(arg => arg.Text != null)?.GetHashCode() ?? 0);
        hash = (hash * 397) ^ (program.Descriptions?.FirstOrDefault(arg => arg.Text != null)?.GetHashCode() ?? 0);
        hash = (hash * 397) ^ (program.Date?.GetHashCode() ?? 0);
        return (hash & 0x7fffffff).ToString();
    }

    private static int DetermineAudioFormat(XmltvProgramme program)
    {
        if (program.Audio?.Stereo == null)
        {
            return 0;
        }

        switch (program.Audio.Stereo.ToLower())
        {
            case "mono": return 1;
            case "stereo": return 2;
            case "dolby": return 3;
            case "surround": case "dolby digital": return 4;
        }
        return 0;
    }

    private static void DetermineRatingAdvisories(MxfProgram mxfProgram, XmltvProgramme xmltvProgramme)
    {
        foreach (var advisory in xmltvProgramme.Rating?.Where(arg => arg.System?.Equals("advisory", StringComparison.OrdinalIgnoreCase) ?? false))
        {
            switch (advisory.Value.ToLower())
            {
                case "adult situations":
                    mxfProgram.HasAdult = true;
                    break;
                case "brief nudity":
                    mxfProgram.HasBriefNudity = true;
                    break;
                case "graphic language":
                    mxfProgram.HasGraphicLanguage = true;
                    break;
                case "graphic violence":
                    mxfProgram.HasGraphicViolence = true;
                    break;
                case "adult language":
                    mxfProgram.HasLanguage = true;
                    break;
                case "mild violence":
                    mxfProgram.HasMildViolence = true;
                    break;
                case "nudity":
                    mxfProgram.HasNudity = true;
                    break;
                case "rape":
                    mxfProgram.HasRape = true;
                    break;
                case "strong sexual content":
                    mxfProgram.HasStrongSexualContent = true;
                    break;
                case "violence":
                    mxfProgram.HasViolence = true;
                    break;
            }
        }
    }

    private void DetermineCastAndCrewCredits(MxfProgram mxfProgram, XmltvProgramme xmltvProgramme)
    {
        if (xmltvProgramme.Credits == null)
        {
            return;
        }

        foreach (var person in xmltvProgramme.Credits.Directors)
        {
            mxfProgram.DirectorRole = mxfProgram.DirectorRole ?? [];
            mxfProgram.DirectorRole.Add(new MxfPersonRank(schedulesDirectData.FindOrCreatePerson(person)));
        }
        foreach (var person in xmltvProgramme.Credits.Actors)
        {
            mxfProgram.ActorRole = mxfProgram.ActorRole ?? [];
            mxfProgram.ActorRole.Add(new MxfPersonRank(schedulesDirectData.FindOrCreatePerson(person.Actor))
            {
                Character = person.Role
            });
        }
        foreach (var person in xmltvProgramme.Credits.Writers)
        {
            mxfProgram.WriterRole = mxfProgram.WriterRole ?? [];
            mxfProgram.WriterRole.Add(new MxfPersonRank(schedulesDirectData.FindOrCreatePerson(person)));
        }
        foreach (var person in xmltvProgramme.Credits.Adapters)
        {
            mxfProgram.WriterRole = mxfProgram.WriterRole ?? [];
            mxfProgram.WriterRole.Add(new MxfPersonRank(schedulesDirectData.FindOrCreatePerson(person)));
        }
        foreach (var person in xmltvProgramme.Credits.Producers)
        {
            mxfProgram.ProducerRole = mxfProgram.ProducerRole ?? [];
            mxfProgram.ProducerRole.Add(new MxfPersonRank(schedulesDirectData.FindOrCreatePerson(person)));
        }
        foreach (var person in xmltvProgramme.Credits.Composers)
        {
            mxfProgram.ProducerRole = mxfProgram.ProducerRole ?? [];
            mxfProgram.ProducerRole.Add(new MxfPersonRank(schedulesDirectData.FindOrCreatePerson(person)));
        }
        foreach (var person in xmltvProgramme.Credits.Editors)
        {
            mxfProgram.HostRole = mxfProgram.HostRole ?? [];
            mxfProgram.HostRole.Add(new MxfPersonRank(schedulesDirectData.FindOrCreatePerson(person)));
        }
        foreach (var person in xmltvProgramme.Credits.Presenters)
        {
            mxfProgram.HostRole = mxfProgram.HostRole ?? [];
            mxfProgram.HostRole.Add(new MxfPersonRank(schedulesDirectData.FindOrCreatePerson(person)));
        }
        foreach (var person in xmltvProgramme.Credits.Commentators)
        {
            mxfProgram.HostRole = mxfProgram.HostRole ?? [];
            mxfProgram.HostRole.Add(new MxfPersonRank(schedulesDirectData.FindOrCreatePerson(person)));
        }
        foreach (var person in xmltvProgramme.Credits.Guests)
        {
            mxfProgram.GuestActorRole = mxfProgram.GuestActorRole ?? [];
            mxfProgram.GuestActorRole.Add(new MxfPersonRank(schedulesDirectData.FindOrCreatePerson(person)));
        }
    }

    private static int DetermineMpaaRatings(XmltvProgramme xmltvProgramme)
    {
        var rating = xmltvProgramme.Rating.FirstOrDefault(arg => (arg.System?.Equals("motion picture association of america", StringComparison.OrdinalIgnoreCase) ?? false) ||
                                                                 (arg.System?.Equals("mpaa", StringComparison.OrdinalIgnoreCase) ?? false))?.Value;
        if (rating == null)
        {
            return 0;
        }

        switch (rating.Replace("-", "").ToLower())
        {
            case "g": return 1;
            case "pg": return 2;
            case "pg13": return 3;
            case "r": return 4;
            case "nc17": return 5;
            case "x": return 6;
            case "nr": return 7;
            case "ao": return 8;
        }
        return 0;
    }

    private static int DetermineHalfStarRatings(XmltvProgramme xmltvProgramme)
    {
        if (xmltvProgramme.StarRating == null)
        {
            return 0;
        }

        foreach (var rating in xmltvProgramme.StarRating.Where(arg => arg.Value != null))
        {
            var numbers = rating.Value.Split('/');
            if (numbers.Length != 2)
            {
                continue;
            }

            var numerator = double.Parse(numbers[0]);
            var denominator = double.Parse(numbers[1]);
            if (denominator == 0)
            {
                continue;
            }

            return (int)((numerator / denominator * 8) + 0.125);
        }
        return 0;
    }

    private static int DetermineTvRatings(XmltvProgramme xmltvProgramme)
    {
        if (xmltvProgramme.Rating == null)
        {
            return 0;
        }

        foreach (var rating in xmltvProgramme.Rating)
        {
            switch (rating.Value.ToLower())
            {
                // usa
                case "tv-y": case "tvy": return 1;
                case "tv-y7": case "tvy7": return 2;
                case "tv-g": case "tvg": return 3;
                case "tv-pg": case "tvpg": return 4;
                case "tv-14": case "tv14": return 5;
                case "tv-ma": case "tvma": return 6;

                // germany
                case "0": return 7;
                case "6": return 8;
                case "12": return 9;
                case "16": return 10;
                //case "18": return 11;

                // france
                case "-10": return 13;
                case "-12": return 14;
                case "-16": return 15;
                case "-18": return 16;

                // great britain
                case "uc": return 22;
                case "u": return 23;
                case "pg": return 24;
                case "12a": return 25;
                case "15": return 26;
                case "18": return 27;
                case "r18": return 28;
            }
        }
        return 0;
    }

    private void DetermineGuideImage(MxfProgram mxfProgram, XmltvProgramme xmltvProgramme)
    {
        if ((xmltvProgramme.Icons?.Count ?? 0) == 0)
        {
            return;
        }

        if (xmltvProgramme.Icons.Count == 1 || xmltvProgramme.Icons[0].Width == 0 || xmltvProgramme.Icons[0].Height == 0)
        {
            mxfProgram.mxfGuideImage = schedulesDirectData.FindOrCreateGuideImage(xmltvProgramme.Icons[0].Src);
            return;
        }

        var posters = xmltvProgramme.Icons.FirstOrDefault(arg => arg.Width / (double)arg.Height < 0.7);
        if (posters != null)
        {
            mxfProgram.mxfGuideImage = schedulesDirectData.FindOrCreateGuideImage(posters.Src);
            return;
        }
        mxfProgram.mxfGuideImage = schedulesDirectData.FindOrCreateGuideImage(xmltvProgramme.Icons[0].Src);
    }

    private bool BuildKeywords()
    {
        logger.LogInformation("Building keyword categories.");
        foreach (var group in schedulesDirectData.KeywordGroups.ToList())
        {
            // sort the group keywords
            group.mxfKeywords = group.mxfKeywords.OrderBy(k => k.Word).ToList();

            // add the keywords
            schedulesDirectData.Keywords.AddRange(group.mxfKeywords);

            // create an overflow for this group giving a max 198 keywords for each group
            var overflow = schedulesDirectData.FindOrCreateKeywordGroup((KeywordGroupsEnum)group.Index - 1, true);
            if (group.mxfKeywords.Count <= 99)
            {
                continue;
            }

            overflow.mxfKeywords = group.mxfKeywords.Skip(99).Take(99).ToList();
        }
        return true;
    }

    private void DetermineProgramKeywords(MxfProgram mxfProgram, string[] categories)
    {
        // determine primary group of program
        var group = KeywordGroupsEnum.UNKNOWN;
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
        else if (mxfProgram.IsSpecial)
        {
            group = KeywordGroupsEnum.SPECIAL;
        }
        else if (mxfProgram.IsReality)
        {
            group = KeywordGroupsEnum.REALITY;
        }
        else if (mxfProgram.IsMusic)
        {
            group = KeywordGroupsEnum.MUSIC;
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

        var mxfKeyGroup = schedulesDirectData.FindOrCreateKeywordGroup(group);
        mxfProgram.mxfKeywords.Add(new MxfKeyword((int)group, mxfKeyGroup.Index, SchedulesDirectData.KeywordGroupsText[(int)group]));

        // add premiere categories as necessary
        if (mxfProgram.IsSeasonPremiere || mxfProgram.IsSeriesPremiere)
        {
            var premiere = schedulesDirectData.FindOrCreateKeywordGroup(KeywordGroupsEnum.PREMIERES);
            mxfProgram.mxfKeywords.Add(new MxfKeyword((int)KeywordGroupsEnum.PREMIERES, premiere.Index, SchedulesDirectData.KeywordGroupsText[(int)KeywordGroupsEnum.PREMIERES]));
            if (mxfProgram.IsSeasonPremiere)
            {
                mxfProgram.mxfKeywords.Add(premiere.FindOrCreateKeyword("Season Premiere"));
            }

            if (mxfProgram.IsSeriesPremiere)
            {
                mxfProgram.mxfKeywords.Add(premiere.FindOrCreateKeyword("Series Premiere"));
            }
        }

        // now add the real categories
        if (categories != null)
        {
            foreach (var genre in categories)
            {
                switch (genre.ToLower())
                {
                    case "sport":
                    case "sports event":
                    case "sports non-event":
                    case "series":
                    case "movie":
                    case "feature film":
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
}