using System.Collections.Concurrent;

using StreamMaster.Domain.Cache;
using StreamMaster.SchedulesDirect.Images;

namespace StreamMaster.SchedulesDirect.Services;

public class ProgramRepository(SMCacheManager<MovieImages> movieCache, SMCacheManager<EpisodeImages> episodeCache)
    : IProgramRepository
{
    public ConcurrentDictionary<string, MxfPerson> People { get; } = [];
    public ConcurrentDictionary<string, MxfProgram> Programs { get; } = new();
    public ConcurrentDictionary<string, Season> Seasons { get; } = new();
    public ConcurrentDictionary<string, SeriesInfo> SeriesInfos { get; } = [];


    public MxfPerson FindOrCreatePerson(string name)
    {
        return People.FindOrCreate(name, key => new MxfPerson(People.Count + 1, key));
    }

    public MxfProgram? FindProgram(string programId)
    {
        return Programs.Values.FirstOrDefault(p => p.ProgramId == programId);
    }

    public bool SetProgramLogos(MxfProgram Program, List<ProgramArtwork> artworks)
    {
        return SetProgramLogos(Program.ProgramId, artworks);
    }

    public bool SetProgramLogos(string ProgramId, List<ProgramArtwork> artworks)
    {
        MxfProgram? mfxProgram = FindProgram(ProgramId);
        if (mfxProgram != null)
        {
            mfxProgram.AddArtworks(artworks);
            return true;
        }

        List<MxfProgram> toUpdate = Programs.Values.Where(a => a.ProgramId.StartsWith(ProgramId)).ToList();
        if (toUpdate.Count != 0)
        {
            toUpdate.ForEach(a => a.AddArtworks(artworks));
            return true;
        }
        return false;
    }


    public Season FindOrCreateSeason(string seriesId, int seasonNumber, string ProgramId)
    {
        SeriesInfo seasonInfo = FindOrCreateSeriesInfo(seriesId, ProgramId);
        Season season = Seasons.FindOrCreate($"{seriesId}_{seasonNumber}", _ => new Season(Seasons.Count + 1, seasonInfo, seasonNumber, ProgramId));
        return season;
    }

    public SeriesInfo FindOrCreateSeriesInfo(string seriesId, string ProgramId)
    {
        SeriesInfo seriesInfo = new(seriesId, ProgramId);
        SeriesInfos.TryAdd(ProgramId, seriesInfo);
        return seriesInfo;
    }

    public SeriesInfo? FindSeriesInfo(string seriesId)
    {
        return SeriesInfos.TryGetValue(seriesId, out SeriesInfo? seriesInfo) ? seriesInfo : null;
    }

    public async Task<MxfProgram> FindOrCreateProgram(string programId, string md5)
    {
        MxfProgram program = Programs.FindOrCreate(programId, _ => new MxfProgram(Programs.Count + 1, programId));
        program.MD5 = md5;

        if (programId.StartsWith("MV"))
        {
            List<ProgramArtwork>? art = await movieCache.GetAsync<List<ProgramArtwork>>(program.ProgramId);
            if (art is not null)
            {
                SetProgramLogos(program, art);
                //program.AddArtworks(art);
            }
        }
        else if (programId.StartsWith("EP"))
        {
            List<ProgramArtwork>? art = await episodeCache.GetAsync<List<ProgramArtwork>>(program.ProgramId);
            if (art is not null)
            {
                SetProgramLogos(program, art);
                //program.AddArtworks(art);
            }
        }

        return program;
    }
    public void RemoveProgram(string programId)
    {
        Programs.TryRemove(programId, out _);
    }
}
