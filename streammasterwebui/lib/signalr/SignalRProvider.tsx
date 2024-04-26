import React, { ReactNode, createContext, useCallback, useContext, useEffect } from 'react';
import SignalRService from './SignalRService';
import useGetEPGColors from '@lib/smAPI/EPGFiles/useGetEPGColors';
import useGetEPGFilePreviewById from '@lib/smAPI/EPGFiles/useGetEPGFilePreviewById';
import useGetEPGNextEPGNumber from '@lib/smAPI/EPGFiles/useGetEPGNextEPGNumber';
import useGetIcons from '@lib/smAPI/Icons/useGetIcons';
import useGetIsSystemReady from '@lib/smAPI/Settings/useGetIsSystemReady';
import useGetPagedChannelGroups from '@lib/smAPI/ChannelGroups/useGetPagedChannelGroups';
import useGetPagedEPGFiles from '@lib/smAPI/EPGFiles/useGetPagedEPGFiles';
import useGetPagedM3UFiles from '@lib/smAPI/M3UFiles/useGetPagedM3UFiles';
import useGetPagedSMChannels from '@lib/smAPI/SMChannels/useGetPagedSMChannels';
import useGetPagedSMStreams from '@lib/smAPI/SMStreams/useGetPagedSMStreams';
import useGetPagedStreamGroups from '@lib/smAPI/StreamGroups/useGetPagedStreamGroups';
import useGetSettings from '@lib/smAPI/Settings/useGetSettings';
import useGetSMChannelStreams from '@lib/smAPI/SMChannelStreamLinks/useGetSMChannelStreams';
import useGetStreamGroups from '@lib/smAPI/StreamGroups/useGetStreamGroups';
import useGetStreamGroupSMChannels from '@lib/smAPI/StreamGroupSMChannelLinks/useGetStreamGroupSMChannels';
import useGetSystemStatus from '@lib/smAPI/Settings/useGetSystemStatus';
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
  const getEPGColors = useGetEPGColors();
  const getEPGFilePreviewById = useGetEPGFilePreviewById();
  const getEPGNextEPGNumber = useGetEPGNextEPGNumber();
  const getIcons = useGetIcons();
  const getIsSystemReady = useGetIsSystemReady();
  const getPagedChannelGroups = useGetPagedChannelGroups();
  const getPagedEPGFiles = useGetPagedEPGFiles();
  const getPagedM3UFiles = useGetPagedM3UFiles();
  const getPagedSMChannels = useGetPagedSMChannels();
  const getPagedSMStreams = useGetPagedSMStreams();
  const getPagedStreamGroups = useGetPagedStreamGroups();
  const getSettings = useGetSettings();
  const getSMChannelStreams = useGetSMChannelStreams();
  const getStreamGroups = useGetStreamGroups();
  const getStreamGroupSMChannels = useGetStreamGroupSMChannels();
  const getSystemStatus = useGetSystemStatus();

  const addMessage = useCallback(
    (entity: SMMessage): void => {
      smMessages.AddMessage(entity);
    },
    [smMessages]
  );

  const dataRefresh = useCallback(
    (entity: string): void => {
      if (entity === 'GetEPGColors') {
        getEPGColors.SetIsForced(true);
        return;
      }
      if (entity === 'GetEPGFilePreviewById') {
        getEPGFilePreviewById.SetIsForced(true);
        return;
      }
      if (entity === 'GetEPGNextEPGNumber') {
        getEPGNextEPGNumber.SetIsForced(true);
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
      if (entity === 'GetSettings') {
        getSettings.SetIsForced(true);
        return;
      }
      if (entity === 'GetSMChannelStreams') {
        getSMChannelStreams.SetIsForced(true);
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
      if (entity === 'GetSystemStatus') {
        getSystemStatus.SetIsForced(true);
        return;
      }
    },
    [getEPGColors,getEPGFilePreviewById,getEPGNextEPGNumber,getIcons,getIsSystemReady,getPagedChannelGroups,getPagedEPGFiles,getPagedM3UFiles,getPagedSMChannels,getPagedSMStreams,getPagedStreamGroups,getSettings,getSMChannelStreams,getStreamGroups,getStreamGroupSMChannels,getSystemStatus]
  );

  const setField = useCallback(
    (fieldDatas: FieldData[]): void => {
      fieldDatas.forEach((fieldData) => {
        if (fieldData.Entity === 'GetEPGColors') {
          getEPGColors.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetEPGFilePreviewById') {
          getEPGFilePreviewById.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetEPGNextEPGNumber') {
          getEPGNextEPGNumber.SetField(fieldData)
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
        if (fieldData.Entity === 'GetSettings') {
          getSettings.SetField(fieldData)
          return;
        }
        if (fieldData.Entity === 'GetSMChannelStreams') {
          getSMChannelStreams.SetField(fieldData)
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
        if (fieldData.Entity === 'GetSystemStatus') {
          getSystemStatus.SetField(fieldData)
          return;
        }
      });
    },
    [getEPGColors,getEPGFilePreviewById,getEPGNextEPGNumber,getIcons,getIsSystemReady,getPagedChannelGroups,getPagedEPGFiles,getPagedM3UFiles,getPagedSMChannels,getPagedSMStreams,getPagedStreamGroups,getSettings,getSMChannelStreams,getStreamGroups,getStreamGroupSMChannels,getSystemStatus]
  );

  const clearByTag = useCallback((data: ClearByTag): void => {
    const { Entity, Tag } = data;
    if (Entity === 'GetEPGColors') {
      getEPGColors.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetEPGFilePreviewById') {
      getEPGFilePreviewById.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetEPGNextEPGNumber') {
      getEPGNextEPGNumber.ClearByTag(Tag)
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
    if (Entity === 'GetSettings') {
      getSettings.ClearByTag(Tag)
      return;
    }
    if (Entity === 'GetSMChannelStreams') {
      getSMChannelStreams.ClearByTag(Tag)
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
    if (Entity === 'GetSystemStatus') {
      getSystemStatus.ClearByTag(Tag)
      return;
    }
    console.log('ClearByTag', Entity, Tag);
  }
,
    [getEPGColors,getEPGFilePreviewById,getEPGNextEPGNumber,getIcons,getIsSystemReady,getPagedChannelGroups,getPagedEPGFiles,getPagedM3UFiles,getPagedSMChannels,getPagedSMStreams,getPagedStreamGroups,getSettings,getSMChannelStreams,getStreamGroups,getStreamGroupSMChannels,getSystemStatus]
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
