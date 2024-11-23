using StreamMaster.Domain.Crypto;

namespace StreamMaster.Domain.Dto;
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class NameLogo
{
    public NameLogo() { }
    //public NameLogo(string Name, string Url, SMFileTypes iconType = SMFileTypes.Logo, bool IsSchedulesDirect = false, string Id = "")
    //{
    //    string _url = string.IsNullOrEmpty(Id) ? Url : Id;
    //    Id = string.IsNullOrEmpty(Id) ? Url.GenerateFNV1a64Hash() : Path.GetFileNameWithoutExtension(Id);
    //    this.Name = Name;
    //    this.Url = _url;
    //    SMFileType = iconType;
    //    Ext = Path.GetExtension(_url) ?? ".png";
    //    FullPath = GetFullPath(FileName, iconType);
    //    this.IsSchedulesDirect = IsSchedulesDirect;
    //}
    public NameLogo(string Url, SMFileTypes iconType = SMFileTypes.Logo)
    {

        Id = Url.GenerateFNV1aHash(withExtension: false);
        this.Url = Url;
        SMFileType = iconType;
        Ext = Path.GetExtension(Url) ?? ".png";
        FullPath = GetFullPath(FileName, iconType);
        IsSchedulesDirect = false;
    }
    public NameLogo(string Name, string Url, SMFileTypes iconType = SMFileTypes.Logo, bool IsSchedulesDirect = false)
    {

        Id = Url.GenerateFNV1aHash(withExtension: false);
        this.Name = Name;
        this.Url = Url;
        SMFileType = iconType;
        Ext = Path.GetExtension(Url) ?? ".png";
        FullPath = GetFullPath(FileName, iconType);
        this.IsSchedulesDirect = IsSchedulesDirect;
    }

    public NameLogo(SMStream smStream)
    {
        SMFileTypes iconType = smStream.SMStreamType switch
        {
            SMStreamTypeEnum.CustomPlayList => SMFileTypes.CustomPlayListLogo,
            _ => SMFileTypes.Logo
        };

        Id = smStream.Logo.GenerateFNV1aHash(withExtension: false);
        Name = smStream.Name;
        Url = smStream.Logo;
        SMFileType = iconType;
        Ext = Path.GetExtension(Url) ?? ".png";
        FullPath = GetFullPath(FileName, iconType);
        IsSchedulesDirect = false;
    }
    public string Ext { get; } = string.Empty;
    public string Id { get; } = string.Empty;
    public string Name { get; } = string.Empty;
    public string FileName => $"{Id}{Ext}";
    public bool IsSchedulesDirect { get; set; }
    public string FullPath { get; } = string.Empty;
    public string Url { get; } = string.Empty;
    //public string SMLogoUrl => $"/api/files/{(int)SMFileType}/{Id.ToBase64String()}{Ext}";
    public SMFileTypes SMFileType { get; }


    public static string GetFullPath(string fileName, SMFileTypes SMFileType)
    {
        FileDefinition? fd = FileDefinitions.GetFileDefinition(SMFileType);
        if (fd is null)
        {
            return string.Empty;
        }

        string subDir = char.ToLower(fileName[0]).ToString();
        string fullPath = Path.Combine(fd.DirectoryLocation, subDir, fileName);
        return fullPath;
    }
}