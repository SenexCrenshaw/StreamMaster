'use client';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

const url = 'http://127.0.0.1:7095/streammasterhub';

export const hubConnection = new HubConnectionBuilder()
  .configureLogging(LogLevel.Information)
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
