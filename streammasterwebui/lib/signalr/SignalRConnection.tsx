import { FieldData } from '@lib/apiDefs';
import useSMChannels from '@lib/smAPI/SMChannels/useSMChannels';
import useSMStreams from '@lib/smAPI/SMStreams/useSMStreams';
import { HubConnectionState } from '@microsoft/signalr';
import { useEffect, useRef } from 'react';
import { useAppInfo } from '../redux/slices/useAppInfo';
import { isClient } from '../settings';
import { hubConnection } from './signalr';
import { dataRefreshListener, setFieldListener } from './singletonListeners';

export const SignalRConnection = ({ children }: React.PropsWithChildren): JSX.Element => {
  const { setHubConnected, setHubDisconnected } = useAppInfo();
  const { setSMStreamsField } = useSMStreams();
  const { refreshSMChannels } = useSMChannels();

  const retries = useRef(0); // store the retry count
  const maxRetries = 5; // define a maximum number of retry attempts
  const initialDelay = 1000; // start with 1 second delay
  const maxDelay = 30000; // max delay is 30 seconds

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
        }
      });
    };

    const dataRefresh = (entity: string): void => {
      if (entity === 'SMStreamDto') {
        refreshSMChannels();
      }
    };

    const AddConnections = () => {
      setFieldListener.addListener(setField);
      dataRefreshListener.addListener(dataRefresh);
    };

    const RemoveConnections = () => {
      setFieldListener.removeListener(setField);
      dataRefreshListener.removeListener(dataRefresh);
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

  return <div>{children}</div>;
};
