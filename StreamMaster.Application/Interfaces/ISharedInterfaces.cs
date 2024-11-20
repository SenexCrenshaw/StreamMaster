using StreamMaster.Application.Custom;
using StreamMaster.Application.EPGFiles;
using StreamMaster.Application.Logos;
using StreamMaster.Application.M3UFiles;
using StreamMaster.Application.Settings;

namespace StreamMaster.Application.Interfaces;

public interface ISharedTasks :

    IEPGFileTasks,
    IM3UFileTasks,
    ILogoTasks,
    ICustomPlayListsTasks,
    ISettingTasks;