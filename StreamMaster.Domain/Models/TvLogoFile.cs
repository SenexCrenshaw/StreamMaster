namespace StreamMaster.Domain.Models;

public class TvLogoFile : IconFile
{
    public TvLogoFile()
    {
        //DirectoryLocation = FileDefinitions.TVLogo.DirectoryLocation;
        FileExtension = FileDefinitions.TVLogo.DefaultExtension;
        SMFileType = FileDefinitions.TVLogo.SMFileType;
    }
}
