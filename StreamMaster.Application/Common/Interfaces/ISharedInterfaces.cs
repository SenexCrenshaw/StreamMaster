using StreamMaster.Application.CustomPlayLists;
using StreamMaster.Application.EPGFiles;
using StreamMaster.Application.Icons;
using StreamMaster.Application.M3UFiles;
using StreamMaster.Application.Programmes;
using StreamMaster.Application.Settings;

namespace StreamMaster.Application.Common.Interfaces;

public interface ISharedTasks :

    IEPGFileTasks,
    IM3UFileTasks,
    IIconTasks,
    ICustomPlayListsTasks,
    IProgrammeChannelTasks,
    ISettingTasks
{
}