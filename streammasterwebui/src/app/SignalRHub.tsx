import React, { useEffect } from 'react';


import { HubConnectionState, type HubConnection } from '@microsoft/signalr';
import { hubConnection } from './signalr';

type SignalRHubProps = {
  // onClose?: () => void;
  readonly onConnected?: (state: boolean) => void;
  // onReconnected?: () => void;
  // onReconnecting?: () => void;
}

export const SignalRHub = (props: SignalRHubProps) => {
  const [hub, setHub] = React.useState<HubConnection | null>(null);
  const [hubConnected, setHubConnected] = React.useState<boolean>(false);

  useEffect(() => {
    if (hubConnection && hub === null) {
      if (hubConnection.state === HubConnectionState.Connected && !hubConnected) {
        setHubConnected(true);
        setHub(hubConnection);
        props.onConnected?.(true);
        console.log("App Connected");

      } else if (hubConnection.state !== HubConnectionState.Connected && hubConnected) {
        setHubConnected(false);
        props.onConnected?.(false);
        console.log("App Disconnected");
      }
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [hubConnected, props]);

  return <div />;
};


export default SignalRHub;
