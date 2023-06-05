namespace StreamMasterDomain.Enums;

//public enum SMQueCommand2
//{
//    AddChannelCategory,
//    AddChannelsFromEPGs,
//    AddIPTVChannelFromEPG,
//    AddIPTVChannelsFromEPGs,
//    AddM3UStreamsFromM3UFile,
//    AddProgrammesFromEPG,
//    AddProgrammesFromEPGs,
//    CacheAllIcons,
//    CacheIconsFromIPChannels,
//    CacheIconsFromStreams,
//    DeleteChannelCategory,
//    MatchM3UStreamsToChannels,
//    MatchProgrammeToChannels,
//    ProcessEPGFile,
//    ProcessM3UFile,
//    ReadTVLogos,
//    RefreshEPGFile,
//    RefreshM3UFile,
//    ScanDirectoryForEPGFiles,
//    ScanDirectoryForIconFiles,
//    ScanDirectoryForM3UFiles,
//    SetIsSystemReady,
//    UpdateCategory,
//    UpdateEntitiesFromChannels,
//    UpdateEntitiesFromIPTVChannels,
//    UpdateIconsSourcesFromChannels,
//    RebuildProgrammeChannelNames,
//    UpdateSetting,
//}

public enum SMQueCommand
{
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
