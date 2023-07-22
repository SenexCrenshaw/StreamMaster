using StreamMasterApplication.ChannelGroups;
using StreamMasterApplication.EPGFiles;
using StreamMasterApplication.Icons;
using StreamMasterApplication.Logs;
using StreamMasterApplication.M3UFiles;
using StreamMasterApplication.Programmes;
using StreamMasterApplication.SchedulesDirectAPI;
using StreamMasterApplication.Settings;
using StreamMasterApplication.StreamGroups;
using StreamMasterApplication.VideoStreams;

namespace StreamMasterApplication.Common.Interfaces;

public interface ISharedHub :
    IChannelGroupHub,
    IEPGFileHub,
    IM3UFileHub,
    ISchedulesDirectHub,
    IVideoStreamHub,
    ILogHub,
    IIconHub,
    IStreamGroupHub,
    IProgrammeChannelHub,
    ISettingHub
{
}

public interface ISharedTasks :
    IChannelGroupTasks,
    IEPGFileTasks,
    IM3UFileTasks,
    ILogTasks,
    IVideoStreamTasks,
    IIconTasks,
    ISchedulesDirectTasks,
    IStreamGroupTasks,
    IProgrammeChannelTasks,
    ISettingTasks
{
}

public interface ISharedDB :
    IChannelGroupDB,
    IEPGFileDB,
    IM3UFileDB,
    IIconDB,
    IVideoStreamDB,
    ISchedulesDirectDB,
    IStreamGroupDB,
    IProgrammeChannelDB,
    ISettingDB
{
}
