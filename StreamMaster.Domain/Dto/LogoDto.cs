namespace StreamMaster.Domain.Dto;
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class LogoDto
{
    public LogoDto()
    {
    }
    public LogoDto(string url, string contentType, string fileName, byte[]? image)
    {
        ContentType = contentType;
        FileName = fileName;
        Url = url;
        if (image is not null)
        {
            Image = image;
        }
    }

    public string ContentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public byte[] Image { get; set; } = [];
}
