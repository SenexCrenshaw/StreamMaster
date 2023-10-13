'use client';
import { baseHostURL, isDev } from '@/lib/settings';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

const url = baseHostURL + '/streammasterhub';

export const hubConnection = new HubConnectionBuilder()
  .configureLogging(LogLevel.Error)
  // .withHubProtocol(new MessagePackHubProtocol())
  .withUrl(url)
  .withAutomaticReconnect({
    nextRetryDelayInMilliseconds: (retryContext) => {
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

export const invokeHubConnection = async <T>(methodName: string, arg?: any): Promise<T | null> => {
  if (hubConnection.state !== 'Connected') return null;
  if (isDev && methodName !== 'GetLog') console.log(methodName);
  const result = await hubConnection.invoke<T>(methodName, arg);
  return result;
};
