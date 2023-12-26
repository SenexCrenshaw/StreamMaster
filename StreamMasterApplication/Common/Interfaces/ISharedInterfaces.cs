using StreamMasterApplication.ChannelGroups;
using StreamMasterApplication.EPGFiles;
using StreamMasterApplication.Icons;
using StreamMasterApplication.LogApp;
using StreamMasterApplication.M3UFiles;
using StreamMasterApplication.Programmes;
using StreamMasterApplication.SchedulesDirect;
using StreamMasterApplication.Settings;
using StreamMasterApplication.StreamGroupChannelGroups;
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
    ISettingHub,
    IStreamGroupChannelGroupHub
{
}

public interface ISharedTasks :
    IChannelGroupTasks,
    IEPGFileTasks,
    IM3UFileTasks,
    ILogTasks,
    IVideoStreamTasks,
    IIconTasks,
    IStreamGroupTasks,
    IProgrammeChannelTasks,
    ISettingTasks
{
}

