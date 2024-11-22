namespace StreamMaster.Domain.Dto;
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class NameLogo
{
    public NameLogo() { }
    public NameLogo(SMChannel smChannel, SMFileTypes fileType)
    {
        //string logo = smChannel.Logo.ToBase64String();
        Id = smChannel.Id.ToString();
        Name = smChannel.Name;
        //SMLogoUrl = logo;
        //SMLogoUrl = $"/api/files/{(int)fileType}/{logo}";
        Url = smChannel.Logo;
        SMFileType = fileType;
    }
    public NameLogo(SMStream smStream)
    {
        SMFileTypes iconType = smStream.SMStreamType switch
        {
            //SMStreamTypeEnum.Regular => SMFileTypes.Logo,
            SMStreamTypeEnum.CustomPlayList => SMFileTypes.CustomPlayListLogo,
            // Add more cases here
            _ => SMFileTypes.Logo // Default case
        };

        //string ext = Path.GetExtension(smStream.Logo) ?? ".png";

        //string logo = smStream.Logo.ToBase64String();
        Id = smStream.Id;
        Name = smStream.Name;
        //SMLogoUrl = $"/api/files/{(int)fileType}/{logo}";
        Url = smStream.Logo;
        SMFileType = iconType;
    }
    public string Ext => Path.GetExtension(Url) ?? ".png";
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string FileName => $"{Id}{Ext}";
    public string Url { get; set; } = string.Empty;
    public string SMLogoUrl => $"/api/files/{(int)SMFileType}/{Id.ToBase64String()}{Ext}";
    public SMFileTypes SMFileType { get; set; }
}