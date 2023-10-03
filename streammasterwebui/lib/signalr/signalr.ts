'use client';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { baseHostURL } from '@/lib/settings'

const url =baseHostURL+'/streammasterhub';

export const hubConnection = new HubConnectionBuilder()
  .configureLogging(LogLevel.Error)
  // .withHubProtocol(new MessagePackHubProtocol())
  .withUrl(url)
  .withAutomaticReconnect({
    nextRetryDelayInMilliseconds: retryContext => {
      if (retryContext.elapsedMilliseconds < 60000) {
        return 2000;
      } else {
        return 2000;
      }
    },
  })
  .build();

export function isSignalRConnected() {
  return hubConnection && hubConnection.state === 'Connected';
}
