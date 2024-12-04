using System.Collections.Concurrent;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData
{
    [XmlArrayItem("Program")]
    public ConcurrentDictionary<string, MxfProgram> Programs { get; set; } = new();

    public async Task<MxfProgram> FindOrCreateProgram(string programId, string md5)
    {
        MxfProgram program = Programs.FindOrCreate(programId, _ => new MxfProgram(Programs.Count + 1, programId));
        program.MD5 = md5;

        if (programId.StartsWith("MV"))
        {
            List<ProgramArtwork>? art = await movieCache.GetAsync<List<ProgramArtwork>>(program.ProgramId);
            if (art is not null)
            {
                program.AddArtworks(art);
            }
        }
        else if (programId.StartsWith("EP"))
        {
            List<ProgramArtwork>? art = await episodeCache.GetAsync<List<ProgramArtwork>>(program.ProgramId);
            if (art is not null)
            {
                program.AddArtworks(art);
            }
        }

        return program;
    }

    public MxfProgram? FindProgram(string programId)
    {
        return Programs.Values.FirstOrDefault(p => p.ProgramId == programId);
    }

    public void RemoveProgram(string programId)
    {
        Programs.TryRemove(programId, out _);
    }
}
