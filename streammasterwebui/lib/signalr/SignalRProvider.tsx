import React, { ReactNode, createContext, useCallback, useContext, useEffect } from 'react';
import SignalRService from './SignalRService';
import useGetAvailableCountries from '@lib/smAPI/SchedulesDirect/useGetAvailableCountries';
import useGetChannelGroups from '@lib/smAPI/ChannelGroups/useGetChannelGroups';
import useGetChannelGroupsFromSMChannels from '@lib/smAPI/ChannelGroups/useGetChannelGroupsFromSMChannels';
import useGetChannelNames from '@lib/smAPI/SchedulesDirect/useGetChannelNames';
import useGetChannelStreamingStatistics from '@lib/smAPI/Statistics/useGetChannelStreamingStatistics';
import useGetClientStreamingStatistics from '@lib/smAPI/Statistics/useGetClientStreamingStatistics';
import useGetEPGColors from '@lib/smAPI/EPG/useGetEPGColors';
import useGetEPGFilePreviewById from '@lib/smAPI/EPGFiles/useGetEPGFilePreviewById';
import useGetEPGFiles from '@lib/smAPI/EPGFiles/useGetEPGFiles';
import useGetEPGNextEPGNumber from '@lib/smAPI/EPGFiles/useGetEPGNextEPGNumber';
import useGetHeadends from '@lib/smAPI/SchedulesDirect/useGetHeadends';
import useGetIcons from '@lib/smAPI/Icons/useGetIcons';
import useGetIsSystemReady from '@lib/smAPI/General/useGetIsSystemReady';
import useGetLineupPreviewChannel from '@lib/smAPI/SchedulesDirect/useGetLineupPreviewChannel';
import useGetLineups from '@lib/smAPI/SchedulesDirect/useGetLineups';
import useGetM3UFileNames from '@lib/smAPI/M3UFiles/useGetM3UFileNames';
import useGetM3UFiles from '@lib/smAPI/M3UFiles/useGetM3UFiles';
import useGetOutputProfile from '@lib/smAPI/Profiles/useGetOutputProfile';
import useGetOutputProfiles from '@lib/smAPI/Profiles/useGetOutputProfiles';
import useGetPagedChannelGroups from '@lib/smAPI/ChannelGroups/useGetPagedChannelGroups';
import useGetPagedEPGFiles from '@lib/smAPI/EPGFiles/useGetPagedEPGFiles';
import useGetPagedM3UFiles from '@lib/smAPI/M3UFiles/useGetPagedM3UFiles';
import useGetPagedSMChannels from '@lib/smAPI/SMChannels/useGetPagedSMChannels';
import useGetPagedSMStreams from '@lib/smAPI/SMStreams/useGetPagedSMStreams';
import useGetPagedStreamGroups from '@lib/smAPI/StreamGroups/useGetPagedStreamGroups';
import useGetSelectedStationIds from '@lib/smAPI/SchedulesDirect/useGetSelectedStationIds';
import useGetService from '@lib/smAPI/SchedulesDirect/useGetService';
import useGetSettings from '@lib/smAPI/Settings/useGetSettings';
import useGetSMChannel from '@lib/smAPI/SMChannels/useGetSMChannel';
import useGetSMChannelNames from '@lib/smAPI/SMChannels/useGetSMChannelNames';
import useGetSMChannelStreams from '@lib/smAPI/SMChannelStreamLinks/useGetSMChannelStreams';
import useGetSMTasks from '@lib/smAPI/SMTasks/useGetSMTasks';
import useGetStationChannelMaps from '@lib/smAPI/SchedulesDirect/useGetStationChannelMaps';
import useGetStationChannelNames from '@lib/smAPI/SchedulesDirect/useGetStationChannelNames';
import useGetStationPreviews from '@lib/smAPI/SchedulesDirect/useGetStationPreviews';
import useGetStreamGroup from '@lib/smAPI/StreamGroups/useGetStreamGroup';
import useGetStreamGroupProfiles from '@lib/smAPI/StreamGroups/useGetStreamGroupProfiles';
import useGetStreamGroups from '@lib/smAPI/StreamGroups/useGetStreamGroups';
import useGetStreamGroupSMChannels from '@lib/smAPI/StreamGroupSMChannelLinks/useGetStreamGroupSMChannels';
import useGetStreamingStatisticsForChannel from '@lib/smAPI/Statistics/useGetStreamingStatisticsForChannel';
import useGetStreamStreamingStatistics from '@lib/smAPI/Statistics/useGetStreamStreamingStatistics';
import useGetSystemStatus from '@lib/smAPI/General/useGetSystemStatus';
import useGetTaskIsRunning from '@lib/smAPI/General/useGetTaskIsRunning';
import useGetUserStatus from '@lib/smAPI/SchedulesDirect/useGetUserStatus';
import useGetVideoInfoFromId from '@lib/smAPI/SMChannels/useGetVideoInfoFromId';
import useGetVideoProfiles from '@lib/smAPI/Profiles/useGetVideoProfiles';
import { useSMMessages } from '@lib/redux/hooks/useSMMessages';
import { ClearByTag, FieldData, SMMessage } from '@lib/smAPI/smapiTypes';

const SignalRContext = createContext<SignalRService | undefined>(undefined);

export const useSignalRService = () => {
  const context = useContext(SignalRContext);
  if (context === undefined) {
    throw new Error('useSignalRService must be used within a SignalRProvider');
  }
  return context;
};

interface SignalRProviderProps {
  children: ReactNode;
}
export const SignalRProvider: React.FC<SignalRProviderProps> = ({ children }) => {
  const smMessages = useSMMessages();
  const signalRService = SignalRService.getInstance();
  const getAvailableCountries = useGetAvailableCountries();
  const getChannelGroups = useGetChannelGroups();
  const getChannelGroupsFromSMChannels = useGetChannelGroupsFromSMChannels();
  const getChannelNames = useGetChannelNames();
  const getChannelStreamingStatistics = useGetChannelStreamingStatistics();
  const getClientStreamingStatistics = useGetClientStreamingStatistics();
  const getEPGColors = useGetEPGColors();
  const getEPGFilePreviewById = useGetEPGFilePreviewById();
  const getEPGFiles = useGetEPGFiles();
  const getEPGNextEPGNumber = useGetEPGNextEPGNumber();
  const getHeadends = useGetHeadends();
  const getIcons = useGetIcons();
  const getIsSystemReady = useGetIsSystemReady();
  const getLineupPreviewChannel = useGetLineupPreviewChannel();
  const getLineups = useGetLineups();
  const getM3UFileNames = useGetM3UFileNames();
  const getM3UFiles = useGetM3UFiles();
  const getOutputProfile = useGetOutputProfile();
  const getOutputProfiles = useGetOutputProfiles();
  const getPagedChannelGroups = useGetPagedChannelGroups();
  const getPagedEPGFiles = useGetPagedEPGFiles();
  const getPagedM3UFiles = useGetPagedM3UFiles();
  const getPagedSMChannels = useGetPagedSMChannels();
  const getPagedSMStreams = useGetPagedSMStreams();
  const getPagedStreamGroups = useGetPagedStreamGroups();
  const getSelectedStationIds = useGetSelectedStationIds();
  const getService = useGetService();
  const getSettings = useGetSettings();
  const getSMChannel = useGetSMChannel();
  const getSMChannelNames = useGetSMChannelNames();
  const getSMChannelStreams = useGetSMChannelStreams();
  const getSMTasks = useGetSMTasks();
  const getStationChannelMaps = useGetStationChannelMaps();
  const getStationChannelNames = useGetStationChannelNames();
  const getStationPreviews = useGetStationPreviews();
  const getStreamGroup = useGetStreamGroup();
  const getStreamGroupProfiles = useGetStreamGroupProfiles();
  const getStreamGroups = useGetStreamGroups();
  const getStreamGroupSMChannels = useGetStreamGroupSMChannels();
  const getStreamingStatisticsForChannel = useGetStreamingStatisticsForChannel();
  const getStreamStreamingStatistics = useGetStreamStreamingStatistics();
  const getSystemStatus = useGetSystemStatus();
  const getTaskIsRunning = useGetTaskIsRunning();
  const getUserStatus = useGetUserStatus();
  const getVideoInfoFromId = useGetVideoInfoFromId();
  const getVideoProfiles = useGetVideoProfiles();

  const addMessage = useCallback(
    (entity: SMMessage): void => {
      smMessages.AddMessage(entity);
    },
    [smMessages]
  );

  const dataRefresh = useCallback(
    (entity: string): void => {
      if (entity === 'GetAvailableCountries') {
        getAvailableCountries.SetIsForced(true);
        return;
      }
      if (entity === 'GetChannelGroups') {
        getChannelGroups.SetIsForced(true);
        return;
      }
      if (entity === 'GetChannelGroupsFromSMChannels') {
        getChannelGroupsFromSMChannels.SetIsForced(true);
        return;
      }
      if (entity === 'GetChannelNames') {
        getChannelNames.SetIsForced(true);
        return;
      }
      if (entity === 'GetChannelStreamingStatistics') {
        getChannelStreamingStatistics.SetIsForced(true);
        return;
      }
      if (entity === 'GetClientStreamingStatistics') {
        getClientStreamingStatistics.SetIsForced(true);
        return;
      }
      if (entity === 'GetEPGColors') {
        getEPGColors.SetIsForced(true);
        return;
      }
      if (entity === 'GetEPGFilePreviewById') {
        getEPGFilePreviewById.SetIsForced(true);
        return;
      }
      if (entity === 'GetEPGFiles') {
        getEPGFiles.SetIsForced(true);
        return;
      }
      if (entity === 'GetEPGNextEPGNumber') {
        getEPGNextEPGNumber.SetIsForced(true);
        return;
      }
      if (entity === 'GetHeadends') {
        getHeadends.SetIsForced(true);
        return;
      }
      if (entity === 'GetIcons') {
        getIcons.SetIsForced(true);
        return;
      }
      if (entity === 'GetIsSystemReady') {
        getIsSystemReady.SetIsForced(true);
        return;
      }
      if (entity === 'GetLineupPreviewChannel') {
        getLineupPreviewChannel.SetIsForced(true);
        return;
      }
      if (entity === 'GetLineups') {
        getLineups.SetIsForced(true);
        return;
      }
      if (entity === 'GetM3UFileNames') {
        getM3UFileNames.SetIsForced(true);
        return;
      }
      if (entity === 'GetM3UFiles') {
        getM3UFiles.SetIsForced(true);
        return;
      }
      if (entity === 'GetOutputProfile') {
        getOutputProfile.SetIsForced(true);
        return;
      }
      if (entity === 'GetOutputProfiles') {
        getOutputProfiles.SetIsForced(true);
        return;
      }
      if (entity === 'GetPagedChannelGroups') {
        getPagedChannelGroups.SetIsForced(true);
        return;
      }
      if (entity === 'GetPagedEPGFiles') {
        getPagedEPGFiles.SetIsForced(true);
        return;
      }
      if (entity === 'GetPagedM3UFiles') {
        getPagedM3UFiles.SetIsForced(true);
        return;
      }
      if (entity === 'GetPagedSMChannels') {
        getPagedSMChannels.SetIsForced(true);
        return;
      }
      if (entity === 'GetPagedSMStreams') {
        getPagedSMStreams.SetIsForced(true);
        return;
      }
      if (entity === 'GetPagedStreamGroups') {
        getPagedStreamGroups.SetIsForced(true);
        return;
      }
      if (entity === 'GetSelectedStationIds') {
        getSelectedStationIds.SetIsForced(true);
        return;
      }
      if (entity === 'GetService') {
        getService.SetIsForced(true);
        return;
      }
      if (entity === 'GetSettings') {
        getSettings.SetIsForced(true);
        return;
      }
      if (entity === 'GetSMChannel') {
        getSMChannel.SetIsForced(true);
        return;
      }
      if (entity === 'GetSMChannelNames') {
        getSMChannelNames.SetIsForced(true);
        return;
      }
      if (entity === 'GetSMChannelStreams') {
        getSMChannelStreams.SetIsForced(true);
        return;
      }
      if (entity === 'GetSMTasks') {
        getSMTasks.SetIsForced(true);
        return;
      }
      if (entity === 'GetStationChannelMaps') {
        getStationChannelMaps.SetIsForced(true);
        return;
      }
      if (entity === 'GetStationChannelNames') {
        getStationChannelNames.SetIsForced(true);
        return;
      }
      if (entity === 'GetStationPreviews') {
        getStationPreviews.SetIsForced(true);
        return;
      }
      if (entity === 'GetStreamGroup') {
        getStreamGroup.SetIsForced(true);
        return;
      }
      if (entity === 'GetStreamGroupProfiles') {
        getStreamGroupProfiles.SetIsForced(true);
        return;
      }
      if (entity === 'GetStreamGroups') {
        getStreamGroups.SetIsForced(true);
        return;
      }
      if (entity === 'GetStreamGroupSMChannels') {
        getStreamGroupSMChannels.SetIsForced(true);
        return;
      }
      if (entity === 'GetStreamingStatisticsForChannel') {
        getStreamingStatisticsForChannel.SetIsForced(true);
        return;
      }
      if (entity === 'GetStreamStreamingStatistics') {
        getStreamStreamingStatistics.SetIsForced(true);
        return;
      }
      if (entity === 'GetSystemStatus') {
        getSystemStatus.SetIsForced(true);
        return;
      }
      if (entity === 'GetTaskIsRunning') {
        getTaskIsRunning.SetIsForced(true);
        return;
      }
      if (entity === 'GetUserStatus') {
        getUserStatus.SetIsForced(true);
        return;
      }
      if (entity === 'GetVideoInfoFromId') {
        getVideoInfoFromId.SetIsForced(true);
        return;
      }
      if (entity === 'GetVideoProfiles') {
        getVideoProfiles.SetIsForced(true);
        return;
      }
      if (entity === 'SchedulesDirect') {
        getAvailableCountries.SetIsForced(true);
        getChannelNames.SetIsForced(true);
        getHeadends.SetIsForced(true);
        getLineupPreviewChannel.SetIsForced(true);
        getLineups.SetIsForced(true);
        getSelectedStationIds.SetIsForced(true);
        getService.SetIsForced(true);
        getStationChannelMaps.SetIsForced(true);
        getStationChannelNames.SetIsForced(true);
        getStationPreviews.SetIsForced(true);
        getUserStatus.SetIsForced(true);
        return;
      }
      if (entity === 'ChannelGroups') {
        getChannelGroups.SetIsForced(true);
        getChannelGroupsFromSMChannels.SetIsForced(true);
        getPagedChannelGroups.SetIsForced(true);
        return;
      }
      if (entity === 'Statistics') {
        getChannelStreamingStatistics.SetIsForced(true);
        getClientStreamingStatistics.SetIsForced(true);
        getStreamingStatisticsForChannel.SetIsForced(true);
        getStreamStreamingStatistics.SetIsForced(true);
        return;
      }
      if (entity === 'EPG') {
        getEPGColors.SetIsForced(true);
        return;
      }
      if (entity === 'EPGFiles') {
        getEPGFilePreviewById.SetIsForced(true);
        getEPGFiles.SetIsForced(true);
        getEPGNextEPGNumber.SetIsForced(true);
        getPagedEPGFiles.SetIsForced(true);
        return;
      }
      if (entity === 'Icons') {
        getIcons.SetIsForced(true);
        return;
      }
      if (entity === 'General') {
        getIsSystemReady.SetIsForced(true);
        getSystemStatus.SetIsForced(true);
        getTaskIsRunning.SetIsForced(true);
        return;
      }
      if (entity === 'M3UFiles') {
        getM3UFileNames.SetIsForced(true);
        getM3UFiles.SetIsForced(true);
        getPagedM3UFiles.SetIsForced(true);
        return;
      }
      if (entity === 'Profiles') {
        getOutputProfile.SetIsForced(true);
        getOutputProfiles.SetIsForced(true);
        getVideoProfiles.SetIsForced(true);
        return;
      }
      if (entity === 'SMChannels') {
        getPagedSMChannels.SetIsForced(true);
        getSMChannel.SetIsForced(true);
        getSMChannelNames.SetIsForced(true);
        getVideoInfoFromId.SetIsForced(true);
        return;
      }
      if (entity === 'SMStreams') {
        getPagedSMStreams.SetIsForced(true);
        return;
      }
      if (entity === 'StreamGroups') {
        getPagedStreamGroups.SetIsForced(true);
        getStreamGroup.SetIsForced(true);
        getStreamGroupProfiles.SetIsForced(true);
        getStreamGroups.SetIsForced(true);
        return;
      }
      if (entity === 'Settings') {
        getSettings.SetIsForced(true);
        return;
      }
      if (entity === 'SMChannelStreamLinks') {
        getSMChannelStreams.SetIsForced(true);
        return;
      }
      if (entity === 'SMTasks') {
        getSMTasks.SetIsForced(true);
        return;
      }
      if (entity === 'StreamGroupSMChannelLinks') {
        getStreamGroupSMChannels.SetIsForced(true);
        return;
      }
    },
    [getAvailableCountries,getChannelGroups,getChannelGroupsFromSMChannels,getChannelNames,getChannelStreamingStatistics,getClientStreamingStatistics,getEPGColors,getEPGFilePreviewById,getEPGFiles,getEPGNextEPGNumber,getHeadends,getIcons,getIsSystemReady,getLineupPreviewChannel,getLineups,getM3UFileNames,getM3UFiles,getOutputProfile,getOutputProfiles,getPagedChannelGroups,getPagedEPGFiles,getPagedM3UFiles,getPagedSMChannels,getPagedSMStreams,getPagedStreamGroups,getSelectedStationIds,getService,getSettings,getSMChannel,getSMChannelNames,getSMChannelStreams,getSMTasks,getStationChannelMaps,getStationChannelNames,getStationPreviews,getStreamGroup,getStreamGroupProfiles,getStreamGroups,getStreamGroupSMChannels,getStreamingStatisticsForChannel,getStreamStreamingStatistics,getSystemStatus,getTaskIsRunning,getUserStatus,getVideoInfoFromId,getVideoProfiles]
  );

  const setField = useCallback(
    (fieldDatas: FieldData[]): void => {
      fieldDatas.forEach((fieldData) => {
        if (fieldData.Entity === 'GetAvailableCountries') {
          getAvailableCountries.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetChannelGroups') {
          getChannelGroups.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetChannelGroupsFromSMChannels') {
          getChannelGroupsFromSMChannels.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetChannelNames') {
          getChannelNames.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetChannelStreamingStatistics') {
          getChannelStreamingStatistics.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetClientStreamingStatistics') {
          getClientStreamingStatistics.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetEPGColors') {
          getEPGColors.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetEPGFilePreviewById') {
          getEPGFilePreviewById.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetEPGFiles') {
          getEPGFiles.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetEPGNextEPGNumber') {
          getEPGNextEPGNumber.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetHeadends') {
          getHeadends.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetIcons') {
          getIcons.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetIsSystemReady') {
          getIsSystemReady.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetLineupPreviewChannel') {
          getLineupPreviewChannel.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetLineups') {
          getLineups.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetM3UFileNames') {
          getM3UFileNames.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetM3UFiles') {
          getM3UFiles.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetOutputProfile') {
          getOutputProfile.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetOutputProfiles') {
          getOutputProfiles.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetPagedChannelGroups') {
          getPagedChannelGroups.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetPagedEPGFiles') {
          getPagedEPGFiles.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetPagedM3UFiles') {
          getPagedM3UFiles.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetPagedSMChannels') {
          getPagedSMChannels.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetPagedSMStreams') {
          getPagedSMStreams.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetPagedStreamGroups') {
          getPagedStreamGroups.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetSelectedStationIds') {
          getSelectedStationIds.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetService') {
          getService.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetSettings') {
          getSettings.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetSMChannel') {
          getSMChannel.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetSMChannelNames') {
          getSMChannelNames.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetSMChannelStreams') {
          getSMChannelStreams.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetSMTasks') {
          getSMTasks.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetStationChannelMaps') {
          getStationChannelMaps.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetStationChannelNames') {
          getStationChannelNames.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetStationPreviews') {
          getStationPreviews.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetStreamGroup') {
          getStreamGroup.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetStreamGroupProfiles') {
          getStreamGroupProfiles.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetStreamGroups') {
          getStreamGroups.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetStreamGroupSMChannels') {
          getStreamGroupSMChannels.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetStreamingStatisticsForChannel') {
          getStreamingStatisticsForChannel.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetStreamStreamingStatistics') {
          getStreamStreamingStatistics.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetSystemStatus') {
          getSystemStatus.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetTaskIsRunning') {
          getTaskIsRunning.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetUserStatus') {
          getUserStatus.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetVideoInfoFromId') {
          getVideoInfoFromId.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetVideoProfiles') {
          getVideoProfiles.SetField(fieldData)
          return;
        }
      if ( fieldData.Entity === 'SchedulesDirect') {
        getAvailableCountries.SetField(fieldData);
        getChannelNames.SetField(fieldData);
        getHeadends.SetField(fieldData);
        getLineupPreviewChannel.SetField(fieldData);
        getLineups.SetField(fieldData);
        getSelectedStationIds.SetField(fieldData);
        getService.SetField(fieldData);
        getStationChannelMaps.SetField(fieldData);
        getStationChannelNames.SetField(fieldData);
        getStationPreviews.SetField(fieldData);
        getUserStatus.SetField(fieldData);
        return;
      }
      if ( fieldData.Entity === 'ChannelGroups') {
        getChannelGroups.SetField(fieldData);
        getChannelGroupsFromSMChannels.SetField(fieldData);
        getPagedChannelGroups.SetField(fieldData);
        return;
      }
      if ( fieldData.Entity === 'Statistics') {
        getChannelStreamingStatistics.SetField(fieldData);
        getClientStreamingStatistics.SetField(fieldData);
        getStreamingStatisticsForChannel.SetField(fieldData);
        getStreamStreamingStatistics.SetField(fieldData);
        return;
      }
      if ( fieldData.Entity === 'EPG') {
        getEPGColors.SetField(fieldData);
        return;
      }
      if ( fieldData.Entity === 'EPGFiles') {
        getEPGFilePreviewById.SetField(fieldData);
        getEPGFiles.SetField(fieldData);
        getEPGNextEPGNumber.SetField(fieldData);
        getPagedEPGFiles.SetField(fieldData);
        return;
      }
      if ( fieldData.Entity === 'Icons') {
        getIcons.SetField(fieldData);
        return;
      }
      if ( fieldData.Entity === 'General') {
        getIsSystemReady.SetField(fieldData);
        getSystemStatus.SetField(fieldData);
        getTaskIsRunning.SetField(fieldData);
        return;
      }
      if ( fieldData.Entity === 'M3UFiles') {
        getM3UFileNames.SetField(fieldData);
        getM3UFiles.SetField(fieldData);
        getPagedM3UFiles.SetField(fieldData);
        return;
      }
      if ( fieldData.Entity === 'Profiles') {
        getOutputProfile.SetField(fieldData);
        getOutputProfiles.SetField(fieldData);
        getVideoProfiles.SetField(fieldData);
        return;
      }
      if ( fieldData.Entity === 'SMChannels') {
        getPagedSMChannels.SetField(fieldData);
        getSMChannel.SetField(fieldData);
        getSMChannelNames.SetField(fieldData);
        getVideoInfoFromId.SetField(fieldData);
        return;
      }
      if ( fieldData.Entity === 'SMStreams') {
        getPagedSMStreams.SetField(fieldData);
        return;
      }
      if ( fieldData.Entity === 'StreamGroups') {
        getPagedStreamGroups.SetField(fieldData);
        getStreamGroup.SetField(fieldData);
        getStreamGroupProfiles.SetField(fieldData);
        getStreamGroups.SetField(fieldData);
        return;
      }
      if ( fieldData.Entity === 'Settings') {
        getSettings.SetField(fieldData);
        return;
      }
      if ( fieldData.Entity === 'SMChannelStreamLinks') {
        getSMChannelStreams.SetField(fieldData);
        return;
      }
      if ( fieldData.Entity === 'SMTasks') {
        getSMTasks.SetField(fieldData);
        return;
      }
      if ( fieldData.Entity === 'StreamGroupSMChannelLinks') {
        getStreamGroupSMChannels.SetField(fieldData);
        return;
      }
      });
    },
    [getAvailableCountries,getChannelGroups,getChannelGroupsFromSMChannels,getChannelNames,getChannelStreamingStatistics,getClientStreamingStatistics,getEPGColors,getEPGFilePreviewById,getEPGFiles,getEPGNextEPGNumber,getHeadends,getIcons,getIsSystemReady,getLineupPreviewChannel,getLineups,getM3UFileNames,getM3UFiles,getOutputProfile,getOutputProfiles,getPagedChannelGroups,getPagedEPGFiles,getPagedM3UFiles,getPagedSMChannels,getPagedSMStreams,getPagedStreamGroups,getSelectedStationIds,getService,getSettings,getSMChannel,getSMChannelNames,getSMChannelStreams,getSMTasks,getStationChannelMaps,getStationChannelNames,getStationPreviews,getStreamGroup,getStreamGroupProfiles,getStreamGroups,getStreamGroupSMChannels,getStreamingStatisticsForChannel,getStreamStreamingStatistics,getSystemStatus,getTaskIsRunning,getUserStatus,getVideoInfoFromId,getVideoProfiles]
  );

  const clearByTag = useCallback((data: ClearByTag): void => {
    const { Entity, Tag } = data;
    if (Entity === 'GetAvailableCountries') {
      getAvailableCountries.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetChannelGroups') {
      getChannelGroups.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetChannelGroupsFromSMChannels') {
      getChannelGroupsFromSMChannels.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetChannelNames') {
      getChannelNames.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetChannelStreamingStatistics') {
      getChannelStreamingStatistics.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetClientStreamingStatistics') {
      getClientStreamingStatistics.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetEPGColors') {
      getEPGColors.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetEPGFilePreviewById') {
      getEPGFilePreviewById.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetEPGFiles') {
      getEPGFiles.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetEPGNextEPGNumber') {
      getEPGNextEPGNumber.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetHeadends') {
      getHeadends.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetIcons') {
      getIcons.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetIsSystemReady') {
      getIsSystemReady.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetLineupPreviewChannel') {
      getLineupPreviewChannel.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetLineups') {
      getLineups.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetM3UFileNames') {
      getM3UFileNames.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetM3UFiles') {
      getM3UFiles.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetOutputProfile') {
      getOutputProfile.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetOutputProfiles') {
      getOutputProfiles.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetPagedChannelGroups') {
      getPagedChannelGroups.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetPagedEPGFiles') {
      getPagedEPGFiles.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetPagedM3UFiles') {
      getPagedM3UFiles.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetPagedSMChannels') {
      getPagedSMChannels.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetPagedSMStreams') {
      getPagedSMStreams.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetPagedStreamGroups') {
      getPagedStreamGroups.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetSelectedStationIds') {
      getSelectedStationIds.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetService') {
      getService.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetSettings') {
      getSettings.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetSMChannel') {
      getSMChannel.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetSMChannelNames') {
      getSMChannelNames.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetSMChannelStreams') {
      getSMChannelStreams.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetSMTasks') {
      getSMTasks.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetStationChannelMaps') {
      getStationChannelMaps.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetStationChannelNames') {
      getStationChannelNames.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetStationPreviews') {
      getStationPreviews.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetStreamGroup') {
      getStreamGroup.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetStreamGroupProfiles') {
      getStreamGroupProfiles.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetStreamGroups') {
      getStreamGroups.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetStreamGroupSMChannels') {
      getStreamGroupSMChannels.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetStreamingStatisticsForChannel') {
      getStreamingStatisticsForChannel.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetStreamStreamingStatistics') {
      getStreamStreamingStatistics.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetSystemStatus') {
      getSystemStatus.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetTaskIsRunning') {
      getTaskIsRunning.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetUserStatus') {
      getUserStatus.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetVideoInfoFromId') {
      getVideoInfoFromId.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetVideoProfiles') {
      getVideoProfiles.ClearByTag(Tag)
      return;
    }
  }
,
    [getAvailableCountries,getChannelGroups,getChannelGroupsFromSMChannels,getChannelNames,getChannelStreamingStatistics,getClientStreamingStatistics,getEPGColors,getEPGFilePreviewById,getEPGFiles,getEPGNextEPGNumber,getHeadends,getIcons,getIsSystemReady,getLineupPreviewChannel,getLineups,getM3UFileNames,getM3UFiles,getOutputProfile,getOutputProfiles,getPagedChannelGroups,getPagedEPGFiles,getPagedM3UFiles,getPagedSMChannels,getPagedSMStreams,getPagedStreamGroups,getSelectedStationIds,getService,getSettings,getSMChannel,getSMChannelNames,getSMChannelStreams,getSMTasks,getStationChannelMaps,getStationChannelNames,getStationPreviews,getStreamGroup,getStreamGroupProfiles,getStreamGroups,getStreamGroupSMChannels,getStreamingStatisticsForChannel,getStreamStreamingStatistics,getSystemStatus,getTaskIsRunning,getUserStatus,getVideoInfoFromId,getVideoProfiles]
  );

  const RemoveConnections = useCallback(() => {
    console.log('SignalR RemoveConnections');
    signalRService.removeListener('ClearByTag', clearByTag);
    signalRService.removeListener('SendMessage', addMessage);
    signalRService.removeListener('DataRefresh', dataRefresh);
    signalRService.removeListener('SetField', setField);
  }, [addMessage, clearByTag, dataRefresh, setField, signalRService]);

  const CheckAndAddConnections = useCallback(() => {
    console.log('SignalR AddConnections');
    signalRService.addListener('ClearByTag', clearByTag);
    signalRService.addListener('SendMessage', addMessage);
    signalRService.addListener('DataRefresh', dataRefresh);
    signalRService.addListener('SetField', setField);
  }, [addMessage, clearByTag, dataRefresh, setField, signalRService]);

useEffect(() => {
    const handleConnect = () => {
      // setIsConnected(true);
      CheckAndAddConnections();
    };
    const handleDisconnect = () => {
      // setIsConnected(false);
      RemoveConnections();
    };

    // Add event listeners
    signalRService.addEventListener('signalr_connected', handleConnect);
    signalRService.addEventListener('signalr_disconnected', handleDisconnect);

    // Remove event listeners on cleanup
    return () => {
      signalRService.removeEventListener('signalr_connected', handleConnect);
      signalRService.removeEventListener('signalr_disconnected', handleDisconnect);
    };
  }, [CheckAndAddConnections, RemoveConnections, signalRService]);

  return <SignalRContext.Provider value={signalRService}>{children}</SignalRContext.Provider>;
}
