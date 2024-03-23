using StreamMaster.Application.ChannelGroups;
using StreamMaster.Application.EPGFiles;
using StreamMaster.Application.Icons;
using StreamMaster.Application.LogApp;
using StreamMaster.Application.M3UFiles;
using StreamMaster.Application.Programmes;
using StreamMaster.Application.SchedulesDirect;
using StreamMaster.Application.Settings;
using StreamMaster.Application.StreamGroupChannelGroups;
using StreamMaster.Application.StreamGroups;
using StreamMaster.Application.VideoStreams;

namespace StreamMaster.Application.Common.Interfaces;

public interface ISharedHub :

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

