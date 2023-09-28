import { apiKey, baseHostURL } from '@/lib/settings';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

const url = baseHostURL === undefined || baseHostURL === '' ? '/streammasterhub' : baseHostURL + '/streammasterhub';

export const hubConnection = new HubConnectionBuilder()
  .configureLogging(LogLevel.Information)
  // .withHubProtocol(new MessagePackHubProtocol())
  .withUrl(url, {
    accessTokenFactory: () => apiKey,
    headers: { 'X-Api-Key': apiKey },
  })
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

export function isSignalRConnected () {
  return hubConnection && hubConnection.state === 'Connected';
}


