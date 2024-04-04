namespace StreamMaster.Domain.Enums;

public enum SMQueCommand
{
    BuildIconCaches,
    BuildProgIconsCacheFromEPGs,
    BuildIconsCacheFromVideoStreams,
    ReadDirectoryLogosRequest,

    ProcessEPGFile,
    ProcessM3UFile,
    ProcessM3UFiles,

    ScanDirectoryForEPGFiles,
    ScanDirectoryForIconFiles,
    ScanDirectoryForM3UFiles,

    EPGSync,
    SetIsSystemReady,

    UpdateEntitiesFromIPTVChannels,
}
