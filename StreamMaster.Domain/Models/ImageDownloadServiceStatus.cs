namespace StreamMaster.Domain.Models;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class ImageDownloadServiceStatus
{
    public int Id { get; set; } = 0;
    public int TotalProgramMetadata { get; set; }
    public int TotallogoInfo { get; set; }
    public int TotalProgramMetadataDownloadAttempts { get; set; }
    public int TotallogoInfoDownloadAttempts { get; set; }
    public int TotalProgramMetadataDownloaded { get; set; }
    public int TotallogoInfoSuccessful { get; set; }
    public int TotalProgramMetadataAlreadyExists { get; set; }
    public int TotallogoInfoAlreadyExists { get; set; }
    public int TotalProgramMetadataErrors { get; set; }
    public int TotallogoInfoErrors { get; set; }
    public int TotalProgramMetadataNoArt { get; set; }
}