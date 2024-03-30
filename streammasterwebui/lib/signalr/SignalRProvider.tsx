import { FieldData } from '@lib/apiDefs';
import { useSMMessages } from '@lib/redux/slices/messagesSlice';
import useGetPagedChannelGroups from '@lib/smAPI/ChannelGroups/useGetPagedChannelGroups';
import useGetPagedM3UFiles from '@lib/smAPI/M3UFiles/useGetPagedM3UFiles';
import useGetPagedSMChannels from '@lib/smAPI/SMChannels/useGetPagedSMChannels';
import useGetPagedSMStreams from '@lib/smAPI/SMStreams/useGetPagedSMStreams';
import React, { ReactNode, createContext, useCallback, useContext, useEffect } from 'react';
import { SMMessage } from './SMMessage';
import SignalRService from './SignalRService';

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
  const getPagedSMStreams = useGetPagedSMStreams();
  const getPagedSMChannels = useGetPagedSMChannels();
  const getPagedM3UFiles = useGetPagedM3UFiles();
  const getPagedChannelGroups = useGetPagedChannelGroups();

  const addMessage = useCallback(
    (entity: SMMessage): void => {
      smMessages.AddMessage(entity);
    },
    [smMessages]
  );

  const dataRefresh = useCallback(
    (entity: string): void => {
      if (entity === 'SMStreamDto') {
        getPagedSMStreams.refreshGetPagedSMStreams();
        return;
      }
      if (entity === 'SMChannelDto') {
        getPagedSMChannels.SetIsForced(true);
        return;
      }
      if (entity === 'M3UFileDto') {
        getPagedM3UFiles.refreshGetPagedM3UFiles();
        return;
      }
      if (entity === 'ChannelGroupDto') {
        getPagedChannelGroups.refreshGetPagedChannelGroups();
        return;
      }
    },
    [getPagedSMStreams, getPagedSMChannels, getPagedM3UFiles, getPagedChannelGroups]
  );

  const setField = useCallback(
    (fieldDatas: FieldData[]): void => {
      fieldDatas.forEach((fieldData) => {
        if (fieldData.entity === 'SMStreamDto') {
          getPagedSMStreams.setGetPagedSMStreamsField(fieldData);
          return;
        }
        if (fieldData.entity === 'SMChannelDto') {
          getPagedSMChannels.SetField(fieldData);
          return;
        }
        if (fieldData.entity === 'M3UFileDto') {
          getPagedM3UFiles.setGetPagedM3UFilesField(fieldData);
          return;
        }
        if (fieldData.entity === 'ChannelGroupDto') {
          getPagedChannelGroups.setGetPagedChannelGroupsField(fieldData);
          return;
        }
      });
    },
    [getPagedSMStreams, getPagedSMChannels, getPagedM3UFiles, getPagedChannelGroups]
  );

  const RemoveConnections = useCallback(() => {
    console.log('SignalR RemoveConnections');
    signalRService.removeListener('SendMessage', addMessage);
    signalRService.removeListener('DataRefresh', dataRefresh);
    signalRService.removeListener('SetField', setField);
  }, [addMessage, dataRefresh, setField, signalRService]);

  const CheckAndAddConnections = useCallback(() => {
    console.log('SignalR AddConnections');
    signalRService.addListener('SendMessage', addMessage);
    signalRService.addListener('DataRefresh', dataRefresh);
    signalRService.addListener('SetField', setField);
  }, [addMessage, dataRefresh, setField, signalRService]);

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
};
