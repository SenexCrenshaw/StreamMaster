using StreamMaster.Application.Custom;
using StreamMaster.Application.EPGFiles;
using StreamMaster.Application.Icons;
using StreamMaster.Application.M3UFiles;
using StreamMaster.Application.Programmes;
using StreamMaster.Application.Settings;

namespace StreamMaster.Application.Interfaces;

public interface ISharedTasks :

    IEPGFileTasks,
    IM3UFileTasks,
    IIconTasks,
    ICustomPlayListsTasks,
    IProgrammeChannelTasks,
    ISettingTasks
{
}