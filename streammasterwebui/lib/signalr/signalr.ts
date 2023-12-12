import { baseHostURL, isDev } from '@lib/settings';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

const url = `${baseHostURL}/streammasterhub`;

export const hubConnection = new HubConnectionBuilder()
  .configureLogging(LogLevel.Error)
  // .withHubProtocol(new MessagePackHubProtocol())
  .withUrl(url)
  .withAutomaticReconnect({
    nextRetryDelayInMilliseconds: (retryContext) => {
      if (retryContext.elapsedMilliseconds < 60_000) {
        return 2000;
      }
      return 2000;
    }
  })
  .build();

export function isSignalRConnected() {
  return hubConnection && hubConnection.state === 'Connected';
}

const blacklistedMethods: string[] = ['GetLog', 'GetIconFromSource'];
const whitelistedMethods: string[] = [];

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const invokeHubConnection = async <T>(methodName: string, argument?: any): Promise<T | null> => {
  if (hubConnection.state !== 'Connected') return null;

  if (isDev && !blacklistedMethods.includes(methodName)) {
    //console.groupCollapsed(`%c${methodName}`, 'color: green; font-weight: bold;');
    if (whitelistedMethods.includes(methodName)) {
      console.info('Arguments:', argument);
    }
    console.groupEnd();
  }

  const result = await hubConnection.invoke<T>(methodName, argument);
  return result;
};
