// useSignalRConnection.ts
import { HubConnectionState } from '@microsoft/signalr';
import { useEffect } from 'react';
import { hubConnection } from './signalr';
import { useAppInfo } from './slices/useAppInfo';

export const useSignalRConnection = () => {
  const { setHubConnected, setHubDisconnected } = useAppInfo();

  useEffect(() => {
    if (!hubConnection) {
      setHubDisconnected(true);
      return;
    }

    if (hubConnection && hubConnection.state === HubConnectionState.Disconnected) {
      hubConnection.start()
        .then(() => { }).catch((error) => {
          console.log('Hub Connection error:', error);
          setHubConnected(false);
        })
        .finally(() => {
          if (hubConnection.state === HubConnectionState.Connected) {
            console.log('Hub Connected');
            setHubConnected(true);
            setHubDisconnected(false);
          }
        });
      return;
    }

    if (hubConnection && hubConnection.state === HubConnectionState.Connected) {
      hubConnection.onclose(() => {
        console.log('Hub Connection closed. Attempting to reconnect...');
        hubConnection.start()
          .then(() => {
          })
          .catch((error) => {
            console.log('Hub Reconnection error:', error);
          }).finally(() => { });
      });
      return;
    }

    return () => {
      hubConnection.off('onclose');
    };
  }, [hubConnection, hubConnection.state, setHubConnected, setHubDisconnected]);
};
