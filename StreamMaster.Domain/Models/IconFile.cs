namespace StreamMaster.Domain.Models;

public class IconFile : CacheEntity
{
    public IconFile()
    {
        //DirectoryLocation = FileDefinitions.M3ULogo.DirectoryLocation;
        FileExtension = FileDefinitions.Logo.DefaultExtension;
        SMFileType = FileDefinitions.Logo.SMFileType;
    }
}
