using StreamMaster.Domain.Attributes;

namespace StreamMaster.Domain.Configuration;

[RequireAll]
public class FFMPEGProfile
{
    public string Parameters { get; set; } = "";
    public int Timeout { get; set; } = 20;
    public bool IsM3U8 { get; set; } = true;
}

public class FFMPEGProfileDto : FFMPEGProfile
{
    public string Name { get; set; } = "";
}

public class FFMPEGProfileDtos : List<FFMPEGProfileDto>
{
}


