using StreamMaster.Application.Custom;
using StreamMaster.Application.EPGFiles;
using StreamMaster.Application.Logos;
using StreamMaster.Application.M3UFiles;
using StreamMaster.Application.Settings;
using StreamMaster.Application.StreamGroups;

namespace StreamMaster.Application.Interfaces;

public interface ISharedTasks :

    IEPGFileTasks,
    IM3UFileTasks,
    ILogoTasks,
    IStreamGroupTasks,
    ICustomPlayListsTasks,
    ISettingTasks;