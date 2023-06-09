namespace StreamMasterDomain.Enums;

public enum SMQueCommand
{
    AddProgrammesFromSD,

    CacheAllIcons,
    CacheIconsFromEPGs,
    CacheIconsFromVideoStreams,
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
