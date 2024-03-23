import { FieldData } from '@lib/apiDefs';
import useSMChannels from '@lib/smAPI/SMChannels/useSMChannels';
import useSMStreams from '@lib/smAPI/SMStreams/useSMStreams';
import { HubConnectionState } from '@microsoft/signalr';
import { useEffect, useRef } from 'react';
import { useAppInfo } from '../redux/slices/useAppInfo';
import { isClient } from '../settings';
import { hubConnection } from './signalr';
import { dataRefreshListener, setFieldListener, smMessagesListener } from './singletonListeners';
import { Toast } from 'primereact/toast';
import { useAppSelector } from '@lib/redux/hooks';
import { SMMessage } from './SMMessage';
import { useSMMessages } from '@lib/redux/slices/messagesSlice';
import useM3UFiles from '@lib/smAPI/M3UFiles/useM3UFiles';

export const SignalRConnection = ({ children }: React.PropsWithChildren): JSX.Element => {
  const toast = useRef<Toast>(null);
  const smMessages = useAppSelector((state) => state.messages);

  const { setHubConnected, setHubDisconnected } = useAppInfo();
  const { refreshSMStreams, setSMStreamsField } = useSMStreams();
  const { refreshSMChannels, setSMChannelsField } = useSMChannels();
  const { refreshM3UFiles, setM3UFilesField } = useM3UFiles();
  const { AddMessage, ClearMessages } = useSMMessages();

  const retries = useRef(0); // store the retry count
  const maxRetries = 5; // define a maximum number of retry attempts
  const initialDelay = 1000; // start with 1 second delay
  const maxDelay = 30000; // max delay is 30 seconds

  useEffect(() => {
    if (smMessages.length === 0) return;

    smMessages.forEach((message) => {
      toast?.current?.show({ severity: message.severity, summary: message.summary, detail: message.detail, life: message.life });
    });

    ClearMessages();
  }, [smMessages]);

  useEffect(() => {
    let isActive = true; // Flag to control async operations

    if (!hubConnection) {
      setHubDisconnected(true);
      return;
    }

    const startConnection = () => {
      if (!isClient) {
        return;
      }

      hubConnection
        .start()
        .then(() => {
          console.log('Hub Connected');
          retries.current = 0; // reset retry count on success
          setHubConnected(true);
          setHubDisconnected(false);
        })
        .catch((error) => {
          console.log('Hub Connection error:', error);
          retries.current += 1;

          if (retries.current <= maxRetries) {
            const delay = Math.min(initialDelay * 2 ** retries.current, maxDelay);
            console.log(`Retry ${retries.current} in ${delay}ms`);
            setTimeout(startConnection, delay); // retry with exponential backoff
          } else {
            setHubConnected(false); // max retries reached
          }
        });
    };

    const setField = (fieldDatas: FieldData[]): void => {
      fieldDatas.forEach((fieldData) => {
        if (fieldData.entity === 'SMStreamDto') {
          setSMStreamsField(fieldData);
          return;
        }
        if (fieldData.entity === 'SMChannelDto') {
          setSMChannelsField(fieldData);
          return;
        }
        if (fieldData.entity === 'M3UFileDto') {
          setM3UFilesField(fieldData);
          return;
        }
      });
    };

    const dataRefresh = (entity: string): void => {
      if (entity === 'SMStreamDto') {
        refreshSMStreams();
        return;
      }
      if (entity === 'SMChannelDto') {
        refreshSMChannels();
        return;
      }
      if (entity === 'M3UFileDto') {
        refreshM3UFiles();
        return;
      }
    };

    const smMessages = (entity: SMMessage): void => {
      AddMessage(entity);
    };

    const AddConnections = () => {
      setFieldListener.addListener(setField);
      dataRefreshListener.addListener(dataRefresh);
      smMessagesListener.addListener(smMessages);
    };

    const RemoveConnections = () => {
      setFieldListener.removeListener(setField);
      dataRefreshListener.removeListener(dataRefresh);
      smMessagesListener.removeListener(smMessages);
    };

    const startConnectionAsync = () => {
      if (hubConnection.state === HubConnectionState.Disconnected && isActive) {
        RemoveConnections();
        startConnection();
      }
    };

    const addConnectionsAndListeners = () => {
      if (hubConnection.state === HubConnectionState.Connected && isActive) {
        const onClose = async (): Promise<void> => {
          if (isActive) {
            RemoveConnections();
            console.log('Hub Connection closed. Attempting to reconnect...');
            startConnection(); // reset retries and start connection attempt
          }
        };

        AddConnections();
        hubConnection.onclose(onClose);
      }
    };

    startConnectionAsync();
    addConnectionsAndListeners();

    // Cleanup function
    return () => {
      isActive = false;

      if (hubConnection) {
        hubConnection.off('onClose');
        RemoveConnections();
      }
    };
  }, [setHubConnected, setHubDisconnected, setSMStreamsField]);

  //  const showContrast = () => {
  //    toast?.current?.show({ severity: 'contrast', summary: 'Contrast', detail: 'Message Content', life: 3000 });
  //  };

  return (
    <>
      <Toast ref={toast} />
      {children}
    </>
  );
};
