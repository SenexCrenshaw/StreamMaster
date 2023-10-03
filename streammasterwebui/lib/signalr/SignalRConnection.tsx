import { HubConnectionState } from '@microsoft/signalr';
import { useCallback, useEffect, useRef } from 'react';
import { useAppInfo } from '../redux/slices/useAppInfo';
import { hubConnection } from './signalr';
import { baseHostURL, apiKey } from '../settings';

export const SignalRConnection = (props: React.PropsWithChildren) => {
  const { setHubConnected, setHubDisconnected } = useAppInfo();
  const retries = useRef(0); // store the retry count

  const maxRetries = 5;  // define a maximum number of retry attempts
  const initialDelay = 1000;  // start with 1 second delay
  const maxDelay = 30000;  // max delay is 30 seconds

  const startConnection = useCallback(() => {

    console.log('baseHostURL', baseHostURL);
    console.log('apiKey', apiKey);

    const url = baseHostURL === undefined || baseHostURL === '' ? '/streammasterhub' : baseHostURL + '/streammasterhub';
    console.log('Connecting to hub at', url);
    hubConnection.baseUrl = url, {
    accessTokenFactory: () => apiKey,
    headers: { 'X-Api-Key': apiKey },
  }

    hubConnection.start()
      .then(() => {
        console.log('Hub Connected');
        retries.current = 0;  // reset retry count on success
        setHubConnected(true);
        setHubDisconnected(false);
      }).catch((error) => {
        console.log('Hub Connection error:', error);
        retries.current++;

        if (retries.current <= maxRetries) {
          const delay = Math.min(initialDelay * 2 ** retries.current, maxDelay);
          console.log(`Retry ${retries.current} in ${delay}ms`);
          setTimeout(startConnection, delay); // retry with exponential backoff
        } else {
          setHubConnected(false);  // max retries reached
        }
      });

    // hubConnection.on('ChannelGroupsRefresh', (data: iptv.ChannelGroupDto[]) => {
    //   console.log('hey handle', data);
    //   dispatch(handleChannelGroupsRefresh(data));
    // });
  }, [ setHubConnected, setHubDisconnected]);

  useEffect(() => {
    if (!hubConnection) {
      setHubDisconnected(true);
      return;
    }

    if (hubConnection.state === HubConnectionState.Disconnected && window.StreamMaster) {
      startConnection();
    }

    if (hubConnection.state === HubConnectionState.Connected) {
      const onClose = () => {
        console.log('Hub Connection closed. Attempting to reconnect...');
        startConnection();  // reset retries and start connection attempt
      };

      hubConnection.onclose(onClose);

    }

  }, [setHubConnected, setHubDisconnected, startConnection]);

  return (<div>{props.children}</div>)
};

