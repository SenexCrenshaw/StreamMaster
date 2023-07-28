namespace StreamMasterDomain.Enums;

public enum SMQueCommand
{
    AddProgrammesFromSD,

    BuildIconCaches,
    BuildProgIconsCacheFromEPGs,
    BuildIconsCacheFromVideoStreams,
    ReadDirectoryLogosRequest,

    ProcessEPGFile,
    ProcessM3UFile,
    ProcessM3UFiles,

    RebuildProgrammeChannelNames,

    ScanDirectoryForEPGFiles,
    ScanDirectoryForIconFiles,
    ScanDirectoryForM3UFiles,

    SetIsSystemReady,
    UpdateEntitiesFromIPTVChannels,
}
