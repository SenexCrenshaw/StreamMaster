using StreamMaster.Application.EPGFiles;
using StreamMaster.Application.Icons;
using StreamMaster.Application.LogApp;
using StreamMaster.Application.M3UFiles;
using StreamMaster.Application.Programmes;
using StreamMaster.Application.Settings;
using StreamMaster.Application.StreamGroupChannelGroupLinks;
using StreamMaster.Application.StreamGroups;
using StreamMaster.Application.VideoStreams;

namespace StreamMaster.Application.Common.Interfaces;

public interface ISharedHub :

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

