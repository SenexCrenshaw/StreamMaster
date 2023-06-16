﻿using StreamMasterApplication.ChannelGroups;
using StreamMasterApplication.EPGFiles;
using StreamMasterApplication.Icons;
using StreamMasterApplication.M3UFiles;
using StreamMasterApplication.Programmes;
using StreamMasterApplication.Settings;
using StreamMasterApplication.StreamGroups;
using StreamMasterApplication.VideoStreams;

namespace StreamMasterApplication.Common.Interfaces;

public interface ISharedHub :
    IChannelGroupHub,
    IEPGFileHub,
    IM3UFileHub,
    IVideoStreamHub,
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
    IVideoStreamTasks,
    IIconTasks,
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
    IStreamGroupDB,
    IProgrammeChannelDB,
    ISettingDB
{
}
