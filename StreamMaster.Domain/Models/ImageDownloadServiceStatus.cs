namespace StreamMaster.Domain.Models;

public class ImageDownloadServiceStatus
{
    public int Id { get; set; } = 0;
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