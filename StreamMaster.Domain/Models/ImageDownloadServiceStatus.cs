namespace StreamMaster.Domain.Models;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class ImageDownloadServiceStatus
{
    public int Id { get; set; } = 0;
    public DownloadStats ProgramLogos { get; set; } = new();
    public DownloadStats Logos { get; set; } = new();

}

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class DownloadStats
{
    public int Queue { get; set; }
    public int Attempts { get; set; }
    public int Successful { get; set; }
    public int AlreadyExists { get; set; }
    public int Errors { get; set; }
    public void Success(bool success)
    {
        if (success)
        {
            Successful++;
        }
        else
        {
            Errors++;
        }
    }
}