namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class ImageInfo(string programId, string iconUri, string realUrl, string fullName, int width, int height)
{
    public string ProgramId { get; set; } = programId;
    public string IconUri { get; set; } = iconUri;
    public string RealUrl { get; set; } = realUrl;
    public string FullName { get; set; } = fullName;
    public int Width { get; set; } = width;
    public int Height { get; set; } = height;
}
