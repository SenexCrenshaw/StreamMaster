import React, { ReactNode, createContext, useCallback, useContext, useEffect, useState } from 'react';
import SignalRService from './SignalRService';
import useSMStreams from '@lib/smAPI/SMStreams/useSMStreams';
import useSMChannels from '@lib/smAPI/SMChannels/useSMChannels';
import useM3UFiles from '@lib/smAPI/M3UFiles/useM3UFiles';
import { SMMessage } from './SMMessage';
import { FieldData } from '@lib/apiDefs';
import { useSMMessages } from '@lib/redux/slices/messagesSlice';

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
  const [isConnected, setIsConnected] = useState(false);
  const smStreams = useSMStreams();
  const smChannels = useSMChannels();
  const smM3UFiles = useM3UFiles();
  const smMessages = useSMMessages();
  const signalRService = SignalRService.getInstance();

  const addMessage = useCallback(
    (entity: SMMessage): void => {
      smMessages.AddMessage(entity);
    },
    [smMessages]
  );

  const dataRefresh = useCallback(
    (entity: string): void => {
      if (entity === 'SMStreamDto') {
        smStreams.refreshSMStreams();
        return;
      }
      if (entity === 'SMChannelDto') {
        smChannels.refreshSMChannels();
        return;
      }
      if (entity === 'M3UFileDto') {
        smM3UFiles.refreshM3UFiles();
        return;
      }
    },
    [smChannels, smM3UFiles, smStreams]
  );

  const setField = useCallback(
    (fieldDatas: FieldData[]): void => {
      fieldDatas.forEach((fieldData) => {
        if (fieldData.entity === 'SMStreamDto') {
          smStreams.setSMStreamsField(fieldData);
          return;
        }
        if (fieldData.entity === 'SMChannelDto') {
          smChannels.setSMChannelsField(fieldData);
          return;
        }
        if (fieldData.entity === 'M3UFileDto') {
          smM3UFiles.setM3UFilesField(fieldData);
          return;
        }
      });
    },
    [smChannels, smM3UFiles, smStreams]
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
      setIsConnected(true);
      CheckAndAddConnections();
    };
    const handleDisconnect = () => {
      setIsConnected(false);
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
  }, [CheckAndAddConnections, signalRService]);

  return <SignalRContext.Provider value={signalRService}>{children}</SignalRContext.Provider>;
};
