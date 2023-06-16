import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { apiKey, baseHostURL } from '../settings';



const url = baseHostURL === undefined || baseHostURL === '' ? '/streammasterhub' : baseHostURL + '/streammasterhub';
console.log('baseHostURL: ', baseHostURL)
console.log('url: ', url)

export const hubConnection = new HubConnectionBuilder()
  .configureLogging(LogLevel.Critical)
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


hubConnection.onclose(() => {
  console.log('Connection closed. Attempting to reconnect...');
  hubConnection.start()
    .then(() => {
    })
    .catch((error) => {
      console.log('Reconnection error:', error);
    }).finally(() => {
      // if (hubConnection.state === HubConnectionState.Connected) {
      //   console.log('Hub Connected');
      // }
    });
});

hubConnection.start()
  .then(() => {
  })
  .catch((error) => {
    console.log('Connection error:', error);
  })
  .finally(() => {
    // if (hubConnection.state === HubConnectionState.Connected) {
    //   console.log('Hub Connected');
    // }
  });



