namespace StreamMaster.Domain.Models;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class ImageDownloadServiceStatus
{
    public int Id { get; set; } = 0;
    public int TotalProgramMetadata { get; set; }
    public int TotalNameLogo { get; set; }
    public int TotalProgramMetadataDownloadAttempts { get; set; }
    public int TotalNameLogoDownloadAttempts { get; set; }
    public int TotalProgramMetadataSuccessful { get; set; }
    public int TotalNameLogoSuccessful { get; set; }
    public int TotalProgramMetadataAlreadyExists { get; set; }
    public int TotalNameLogoAlreadyExists { get; set; }
    public int TotalProgramMetadataErrors { get; set; }
    public int TotalNameLogoErrors { get; set; }
    public int TotalAlreadyExists { get; set; }
    public int TotalNoArt { get; set; }
}