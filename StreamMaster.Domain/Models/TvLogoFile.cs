using StreamMaster.Domain.Enums;

namespace StreamMaster.Domain.Models;

public class TvLogoFile : IconFile
{
    public TvLogoFile()
    {
        //DirectoryLocation = FileDefinitions.TVLogo.DirectoryLocation;
        FileExtension = FileDefinitions.TVLogo.FileExtension;
        SMFileType = FileDefinitions.TVLogo.SMFileType;
    }
}
