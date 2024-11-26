using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.Models;

public class BaseArt
{
    [XmlAttribute("artWorks")]
    public List<ProgramArtwork> ArtWorks = [];
    public void AddArtwork(ProgramArtwork artwork)
    {
        if (ArtWorks.Any(existing => existing.Uri == artwork.Uri))
        {
            return;
        }

        ArtWorks.Add(artwork);
    }

    public void AddArtwork(IList<ProgramArtwork> artwork)
    {
        foreach (ProgramArtwork art in artwork)
        {
            AddArtwork(art);
        }
    }
}
