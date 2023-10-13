namespace StreamMasterDomain.Enums;

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

    SetSDProgramme,
    SetIsSystemReady,

    UpdateChannelGroupCounts,
    UpdateEntitiesFromIPTVChannels,
}
