namespace StreamMaster.Domain.Enums;

public enum SMQueCommand
{
    BuildLogoCaches,
    BuildLogosCacheFromStreams,
    BuildProgLogosCacheFromEPGs,
    //BuildLogosCacheFromVideoStreams,
    ReadDirectoryLogosRequest,

    ProcessEPGFile,
    ProcessM3UFile,
    ProcessM3UFiles,

    ScanDirectoryForEPGFiles,
    ScanDirectoryForLogoFiles,
    ScanDirectoryForM3UFiles,
    ScanForCustomPlayLists,

    EPGSync,
    SetIsSystemReady,
    SetTestTask,

    UpdateEntitiesFromIPTVChannels,
}
