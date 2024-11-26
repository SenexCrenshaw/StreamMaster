using System.Collections.Concurrent;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData
{
    [XmlArrayItem("GuideImage")]
    public ConcurrentDictionary<string, ProgramArtwork> GuideImages { get; set; } = [];

    public ProgramArtwork FindOrCreateProgramArtwork(ProgramArtwork artWork)
    {
        if (GuideImages.TryGetValue(artWork.Uri, out ProgramArtwork? existingArtWork))
        {
            return existingArtWork;
        }

        GuideImages.TryAdd(artWork.Uri, artWork);
        return artWork;
    }

    public ProgramArtwork FindOrCreateProgramArtwork(string Uri)
    {
        if (GuideImages.TryGetValue(Uri, out ProgramArtwork? existingArtWork))
        {
            return existingArtWork;
        }
        existingArtWork = new ProgramArtwork { Uri = Uri };
        GuideImages.TryAdd(existingArtWork.Uri, existingArtWork);
        return existingArtWork;
    }
}
