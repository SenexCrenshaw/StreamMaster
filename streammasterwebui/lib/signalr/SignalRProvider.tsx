import React, { ReactNode, createContext, useCallback, useContext, useEffect } from 'react';
import SignalRService from './SignalRService';
import useGetAvailableCountries from '@lib/smAPI/SchedulesDirect/useGetAvailableCountries';
import useGetChannelGroups from '@lib/smAPI/ChannelGroups/useGetChannelGroups';
import useGetChannelGroupsFromSMChannels from '@lib/smAPI/ChannelGroups/useGetChannelGroupsFromSMChannels';
import useGetChannelMetrics from '@lib/smAPI/Statistics/useGetChannelMetrics';
import useGetCommandProfiles from '@lib/smAPI/Profiles/useGetCommandProfiles';
import useGetCustomPlayList from '@lib/smAPI/Custom/useGetCustomPlayList';
import useGetCustomPlayLists from '@lib/smAPI/Custom/useGetCustomPlayLists';
import useGetDownloadServiceStatus from '@lib/smAPI/General/useGetDownloadServiceStatus';
import useGetEPGColors from '@lib/smAPI/EPG/useGetEPGColors';
import useGetEPGFilePreviewById from '@lib/smAPI/EPGFiles/useGetEPGFilePreviewById';
import useGetEPGFiles from '@lib/smAPI/EPGFiles/useGetEPGFiles';
import useGetEPGNextEPGNumber from '@lib/smAPI/EPGFiles/useGetEPGNextEPGNumber';
import useGetHeadendsByCountryPostal from '@lib/smAPI/SchedulesDirect/useGetHeadendsByCountryPostal';
import useGetHeadendsToView from '@lib/smAPI/SchedulesDirect/useGetHeadendsToView';
import useGetIntroPlayLists from '@lib/smAPI/Custom/useGetIntroPlayLists';
import useGetIsSystemReady from '@lib/smAPI/General/useGetIsSystemReady';
import useGetLineupPreviewChannel from '@lib/smAPI/SchedulesDirect/useGetLineupPreviewChannel';
import useGetLogos from '@lib/smAPI/Logos/useGetLogos';
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
import useGetSettings from '@lib/smAPI/Settings/useGetSettings';
import useGetSMChannelChannels from '@lib/smAPI/SMChannelChannelLinks/useGetSMChannelChannels';
import useGetSMChannelNameLogos from '@lib/smAPI/SMChannels/useGetSMChannelNameLogos';
import useGetSMChannelNames from '@lib/smAPI/SMChannels/useGetSMChannelNames';
import useGetSMChannelStreams from '@lib/smAPI/SMChannelStreamLinks/useGetSMChannelStreams';
import useGetSMTasks from '@lib/smAPI/SMTasks/useGetSMTasks';
import useGetStationChannelNames from '@lib/smAPI/SchedulesDirect/useGetStationChannelNames';
import useGetStationPreviews from '@lib/smAPI/SchedulesDirect/useGetStationPreviews';
import useGetStreamGroup from '@lib/smAPI/StreamGroups/useGetStreamGroup';
import useGetStreamGroupProfiles from '@lib/smAPI/StreamGroups/useGetStreamGroupProfiles';
import useGetStreamGroups from '@lib/smAPI/StreamGroups/useGetStreamGroups';
import useGetStreamGroupSMChannels from '@lib/smAPI/StreamGroupSMChannelLinks/useGetStreamGroupSMChannels';
import useGetSubScribedHeadends from '@lib/smAPI/SchedulesDirect/useGetSubScribedHeadends';
import useGetSubscribedLineups from '@lib/smAPI/SchedulesDirect/useGetSubscribedLineups';
import useGetSystemStatus from '@lib/smAPI/General/useGetSystemStatus';
import useGetTaskIsRunning from '@lib/smAPI/General/useGetTaskIsRunning';
import useGetVideoInfo from '@lib/smAPI/Statistics/useGetVideoInfo';
import useGetVideoInfos from '@lib/smAPI/Statistics/useGetVideoInfos';
import useGetVideoStreamNamesAndUrls from '@lib/smAPI/SMChannels/useGetVideoStreamNamesAndUrls';
import useGetVs from '@lib/smAPI/Vs/useGetVs';
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
  const getChannelMetrics = useGetChannelMetrics();
  const getCommandProfiles = useGetCommandProfiles();
  const getCustomPlayList = useGetCustomPlayList();
  const getCustomPlayLists = useGetCustomPlayLists();
  const getDownloadServiceStatus = useGetDownloadServiceStatus();
  const getEPGColors = useGetEPGColors();
  const getEPGFilePreviewById = useGetEPGFilePreviewById();
  const getEPGFiles = useGetEPGFiles();
  const getEPGNextEPGNumber = useGetEPGNextEPGNumber();
  const getHeadendsByCountryPostal = useGetHeadendsByCountryPostal();
  const getHeadendsToView = useGetHeadendsToView();
  const getIntroPlayLists = useGetIntroPlayLists();
  const getIsSystemReady = useGetIsSystemReady();
  const getLineupPreviewChannel = useGetLineupPreviewChannel();
  const getLogos = useGetLogos();
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
  const getSettings = useGetSettings();
  const getSMChannelChannels = useGetSMChannelChannels();
  const getSMChannelNameLogos = useGetSMChannelNameLogos();
  const getSMChannelNames = useGetSMChannelNames();
  const getSMChannelStreams = useGetSMChannelStreams();
  const getSMTasks = useGetSMTasks();
  const getStationChannelNames = useGetStationChannelNames();
  const getStationPreviews = useGetStationPreviews();
  const getStreamGroup = useGetStreamGroup();
  const getStreamGroupProfiles = useGetStreamGroupProfiles();
  const getStreamGroups = useGetStreamGroups();
  const getStreamGroupSMChannels = useGetStreamGroupSMChannels();
  const getSubScribedHeadends = useGetSubScribedHeadends();
  const getSubscribedLineups = useGetSubscribedLineups();
  const getSystemStatus = useGetSystemStatus();
  const getTaskIsRunning = useGetTaskIsRunning();
  const getVideoInfo = useGetVideoInfo();
  const getVideoInfos = useGetVideoInfos();
  const getVideoStreamNamesAndUrls = useGetVideoStreamNamesAndUrls();
  const getVs = useGetVs();

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
      if (entity === 'GetChannelMetrics') {
        getChannelMetrics.SetIsForced(true);
        return;
      }
      if (entity === 'GetCommandProfiles') {
        getCommandProfiles.SetIsForced(true);
        return;
      }
      if (entity === 'GetCustomPlayList') {
        getCustomPlayList.SetIsForced(true);
        return;
      }
      if (entity === 'GetCustomPlayLists') {
        getCustomPlayLists.SetIsForced(true);
        return;
      }
      if (entity === 'GetDownloadServiceStatus') {
        getDownloadServiceStatus.SetIsForced(true);
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
      if (entity === 'GetHeadendsByCountryPostal') {
        getHeadendsByCountryPostal.SetIsForced(true);
        return;
      }
      if (entity === 'GetHeadendsToView') {
        getHeadendsToView.SetIsForced(true);
        return;
      }
      if (entity === 'GetIntroPlayLists') {
        getIntroPlayLists.SetIsForced(true);
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
      if (entity === 'GetLogos') {
        getLogos.SetIsForced(true);
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
      if (entity === 'GetSettings') {
        getSettings.SetIsForced(true);
        return;
      }
      if (entity === 'GetSMChannelChannels') {
        getSMChannelChannels.SetIsForced(true);
        return;
      }
      if (entity === 'GetSMChannelNameLogos') {
        getSMChannelNameLogos.SetIsForced(true);
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
      if (entity === 'GetSubScribedHeadends') {
        getSubScribedHeadends.SetIsForced(true);
        return;
      }
      if (entity === 'GetSubscribedLineups') {
        getSubscribedLineups.SetIsForced(true);
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
      if (entity === 'GetVideoInfo') {
        getVideoInfo.SetIsForced(true);
        return;
      }
      if (entity === 'GetVideoInfos') {
        getVideoInfos.SetIsForced(true);
        return;
      }
      if (entity === 'GetVideoStreamNamesAndUrls') {
        getVideoStreamNamesAndUrls.SetIsForced(true);
        return;
      }
      if (entity === 'GetVs') {
        getVs.SetIsForced(true);
        return;
      }
      if (entity === 'SchedulesDirect') {
        getAvailableCountries.SetIsForced(true);
        getHeadendsToView.SetIsForced(true);
        getSelectedStationIds.SetIsForced(true);
        getStationChannelNames.SetIsForced(true);
        getStationPreviews.SetIsForced(true);
        getSubScribedHeadends.SetIsForced(true);
        getSubscribedLineups.SetIsForced(true);
        return;
      }
      if (entity === 'ChannelGroups') {
        getChannelGroups.SetIsForced(true);
        getChannelGroupsFromSMChannels.SetIsForced(true);
        getPagedChannelGroups.SetIsForced(true);
        return;
      }
      if (entity === 'Statistics') {
        getChannelMetrics.SetIsForced(true);
        getVideoInfos.SetIsForced(true);
        return;
      }
      if (entity === 'Profiles') {
        getCommandProfiles.SetIsForced(true);
        getOutputProfiles.SetIsForced(true);
        return;
      }
      if (entity === 'Custom') {
        getCustomPlayLists.SetIsForced(true);
        getIntroPlayLists.SetIsForced(true);
        return;
      }
      if (entity === 'General') {
        getDownloadServiceStatus.SetIsForced(true);
        getIsSystemReady.SetIsForced(true);
        getSystemStatus.SetIsForced(true);
        getTaskIsRunning.SetIsForced(true);
        return;
      }
      if (entity === 'EPG') {
        getEPGColors.SetIsForced(true);
        return;
      }
      if (entity === 'EPGFiles') {
        getEPGFiles.SetIsForced(true);
        getEPGNextEPGNumber.SetIsForced(true);
        getPagedEPGFiles.SetIsForced(true);
        return;
      }
      if (entity === 'Logos') {
        getLogos.SetIsForced(true);
        return;
      }
      if (entity === 'M3UFiles') {
        getM3UFileNames.SetIsForced(true);
        getM3UFiles.SetIsForced(true);
        getPagedM3UFiles.SetIsForced(true);
        return;
      }
      if (entity === 'SMChannels') {
        getPagedSMChannels.SetIsForced(true);
        getSMChannelNameLogos.SetIsForced(true);
        getSMChannelNames.SetIsForced(true);
        getVideoStreamNamesAndUrls.SetIsForced(true);
        return;
      }
      if (entity === 'SMStreams') {
        getPagedSMStreams.SetIsForced(true);
        return;
      }
      if (entity === 'StreamGroups') {
        getPagedStreamGroups.SetIsForced(true);
        getStreamGroupProfiles.SetIsForced(true);
        getStreamGroups.SetIsForced(true);
        return;
      }
      if (entity === 'Settings') {
        getSettings.SetIsForced(true);
        return;
      }
      if (entity === 'SMTasks') {
        getSMTasks.SetIsForced(true);
        return;
      }
    },
    [getAvailableCountries,getChannelGroups,getChannelGroupsFromSMChannels,getChannelMetrics,getCommandProfiles,getCustomPlayList,getCustomPlayLists,getDownloadServiceStatus,getEPGColors,getEPGFilePreviewById,getEPGFiles,getEPGNextEPGNumber,getHeadendsByCountryPostal,getHeadendsToView,getIntroPlayLists,getIsSystemReady,getLineupPreviewChannel,getLogos,getM3UFileNames,getM3UFiles,getOutputProfile,getOutputProfiles,getPagedChannelGroups,getPagedEPGFiles,getPagedM3UFiles,getPagedSMChannels,getPagedSMStreams,getPagedStreamGroups,getSelectedStationIds,getSettings,getSMChannelChannels,getSMChannelNameLogos,getSMChannelNames,getSMChannelStreams,getSMTasks,getStationChannelNames,getStationPreviews,getStreamGroup,getStreamGroupProfiles,getStreamGroups,getStreamGroupSMChannels,getSubScribedHeadends,getSubscribedLineups,getSystemStatus,getTaskIsRunning,getVideoInfo,getVideoInfos,getVideoStreamNamesAndUrls,getVs]
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
        if (fieldData.Entity === 'GetChannelMetrics') {
          getChannelMetrics.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetCommandProfiles') {
          getCommandProfiles.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetCustomPlayList') {
          getCustomPlayList.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetCustomPlayLists') {
          getCustomPlayLists.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetDownloadServiceStatus') {
          getDownloadServiceStatus.SetField(fieldData)
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
        if (fieldData.Entity === 'GetHeadendsByCountryPostal') {
          getHeadendsByCountryPostal.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetHeadendsToView') {
          getHeadendsToView.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetIntroPlayLists') {
          getIntroPlayLists.SetField(fieldData)
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
        if (fieldData.Entity === 'GetLogos') {
          getLogos.SetField(fieldData)
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
        if (fieldData.Entity === 'GetSettings') {
          getSettings.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetSMChannelChannels') {
          getSMChannelChannels.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetSMChannelNameLogos') {
          getSMChannelNameLogos.SetField(fieldData)
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
        if (fieldData.Entity === 'GetSubScribedHeadends') {
          getSubScribedHeadends.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetSubscribedLineups') {
          getSubscribedLineups.SetField(fieldData)
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
        if (fieldData.Entity === 'GetVideoInfo') {
          getVideoInfo.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetVideoInfos') {
          getVideoInfos.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetVideoStreamNamesAndUrls') {
          getVideoStreamNamesAndUrls.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetVs') {
          getVs.SetField(fieldData)
          return;
        }
      if ( fieldData.Entity === 'SchedulesDirect') {
        getAvailableCountries.SetField(fieldData);
        getHeadendsByCountryPostal.SetField(fieldData);
        getHeadendsToView.SetField(fieldData);
        getLineupPreviewChannel.SetField(fieldData);
        getSelectedStationIds.SetField(fieldData);
        getStationChannelNames.SetField(fieldData);
        getStationPreviews.SetField(fieldData);
        getSubScribedHeadends.SetField(fieldData);
        getSubscribedLineups.SetField(fieldData);
        return;
      }
      if ( fieldData.Entity === 'ChannelGroups') {
        getChannelGroups.SetField(fieldData);
        getChannelGroupsFromSMChannels.SetField(fieldData);
        getPagedChannelGroups.SetField(fieldData);
        return;
      }
      if ( fieldData.Entity === 'Statistics') {
        getChannelMetrics.SetField(fieldData);
        getVideoInfo.SetField(fieldData);
        getVideoInfos.SetField(fieldData);
        return;
      }
      if ( fieldData.Entity === 'Profiles') {
        getCommandProfiles.SetField(fieldData);
        getOutputProfile.SetField(fieldData);
        getOutputProfiles.SetField(fieldData);
        return;
      }
      if ( fieldData.Entity === 'Custom') {
        getCustomPlayList.SetField(fieldData);
        getCustomPlayLists.SetField(fieldData);
        getIntroPlayLists.SetField(fieldData);
        return;
      }
      if ( fieldData.Entity === 'General') {
        getDownloadServiceStatus.SetField(fieldData);
        getIsSystemReady.SetField(fieldData);
        getSystemStatus.SetField(fieldData);
        getTaskIsRunning.SetField(fieldData);
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
      if ( fieldData.Entity === 'Logos') {
        getLogos.SetField(fieldData);
        return;
      }
      if ( fieldData.Entity === 'M3UFiles') {
        getM3UFileNames.SetField(fieldData);
        getM3UFiles.SetField(fieldData);
        getPagedM3UFiles.SetField(fieldData);
        return;
      }
      if ( fieldData.Entity === 'SMChannels') {
        getPagedSMChannels.SetField(fieldData);
        getSMChannelNameLogos.SetField(fieldData);
        getSMChannelNames.SetField(fieldData);
        getVideoStreamNamesAndUrls.SetField(fieldData);
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
      if ( fieldData.Entity === 'SMChannelChannelLinks') {
        getSMChannelChannels.SetField(fieldData);
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
      if ( fieldData.Entity === 'Vs') {
        getVs.SetField(fieldData);
        return;
      }
      });
    },
    [getAvailableCountries,getChannelGroups,getChannelGroupsFromSMChannels,getChannelMetrics,getCommandProfiles,getCustomPlayList,getCustomPlayLists,getDownloadServiceStatus,getEPGColors,getEPGFilePreviewById,getEPGFiles,getEPGNextEPGNumber,getHeadendsByCountryPostal,getHeadendsToView,getIntroPlayLists,getIsSystemReady,getLineupPreviewChannel,getLogos,getM3UFileNames,getM3UFiles,getOutputProfile,getOutputProfiles,getPagedChannelGroups,getPagedEPGFiles,getPagedM3UFiles,getPagedSMChannels,getPagedSMStreams,getPagedStreamGroups,getSelectedStationIds,getSettings,getSMChannelChannels,getSMChannelNameLogos,getSMChannelNames,getSMChannelStreams,getSMTasks,getStationChannelNames,getStationPreviews,getStreamGroup,getStreamGroupProfiles,getStreamGroups,getStreamGroupSMChannels,getSubScribedHeadends,getSubscribedLineups,getSystemStatus,getTaskIsRunning,getVideoInfo,getVideoInfos,getVideoStreamNamesAndUrls,getVs]
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
    if (Entity === 'GetChannelMetrics') {
      getChannelMetrics.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetCommandProfiles') {
      getCommandProfiles.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetCustomPlayList') {
      getCustomPlayList.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetCustomPlayLists') {
      getCustomPlayLists.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetDownloadServiceStatus') {
      getDownloadServiceStatus.ClearByTag(Tag)
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
    if (Entity === 'GetHeadendsByCountryPostal') {
      getHeadendsByCountryPostal.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetHeadendsToView') {
      getHeadendsToView.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetIntroPlayLists') {
      getIntroPlayLists.ClearByTag(Tag)
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
    if (Entity === 'GetLogos') {
      getLogos.ClearByTag(Tag)
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
    if (Entity === 'GetSettings') {
      getSettings.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetSMChannelChannels') {
      getSMChannelChannels.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetSMChannelNameLogos') {
      getSMChannelNameLogos.ClearByTag(Tag)
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
    if (Entity === 'GetSubScribedHeadends') {
      getSubScribedHeadends.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetSubscribedLineups') {
      getSubscribedLineups.ClearByTag(Tag)
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
    if (Entity === 'GetVideoInfo') {
      getVideoInfo.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetVideoInfos') {
      getVideoInfos.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetVideoStreamNamesAndUrls') {
      getVideoStreamNamesAndUrls.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetVs') {
      getVs.ClearByTag(Tag)
      return;
    }
  }
,
    [getAvailableCountries,getChannelGroups,getChannelGroupsFromSMChannels,getChannelMetrics,getCommandProfiles,getCustomPlayList,getCustomPlayLists,getDownloadServiceStatus,getEPGColors,getEPGFilePreviewById,getEPGFiles,getEPGNextEPGNumber,getHeadendsByCountryPostal,getHeadendsToView,getIntroPlayLists,getIsSystemReady,getLineupPreviewChannel,getLogos,getM3UFileNames,getM3UFiles,getOutputProfile,getOutputProfiles,getPagedChannelGroups,getPagedEPGFiles,getPagedM3UFiles,getPagedSMChannels,getPagedSMStreams,getPagedStreamGroups,getSelectedStationIds,getSettings,getSMChannelChannels,getSMChannelNameLogos,getSMChannelNames,getSMChannelStreams,getSMTasks,getStationChannelNames,getStationPreviews,getStreamGroup,getStreamGroupProfiles,getStreamGroups,getStreamGroupSMChannels,getSubScribedHeadends,getSubscribedLineups,getSystemStatus,getTaskIsRunning,getVideoInfo,getVideoInfos,getVideoStreamNamesAndUrls,getVs]
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
