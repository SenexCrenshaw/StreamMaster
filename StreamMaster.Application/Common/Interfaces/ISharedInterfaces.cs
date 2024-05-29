using StreamMaster.Application.EPGFiles;
using StreamMaster.Application.Icons;
using StreamMaster.Application.LogApp;
using StreamMaster.Application.M3UFiles;
using StreamMaster.Application.Programmes;
using StreamMaster.Application.Settings;
using StreamMaster.Application.StreamGroups;

namespace StreamMaster.Application.Common.Interfaces;

public interface ISharedHub :
    ILogHub,
    IIconHub,
    IStreamGroupHub,
    IProgrammeChannelHub,
    ISettingHub
{
}

public interface ISharedTasks :

    IEPGFileTasks,
    IM3UFileTasks,
    ILogTasks,
    IIconTasks,
    IStreamGroupTasks,
    IProgrammeChannelTasks,
    ISettingTasks
{
}

