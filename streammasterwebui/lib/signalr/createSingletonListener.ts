import { isDev } from '../settings';

export type SingletonListener = {
  addListener: (callback: (data: any) => void) => void;
  removeListener: (callback: (data: any) => void) => void;
};

export function createSingletonListener<T>(messageName: string, connection: signalR.HubConnection): SingletonListener {
  let listenerCount = 0;

  return {
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    addListener: (callback: (data: T) => void) => {
      if (listenerCount === 0) {
        if (isDev) {
          console.log('Add listener for ' + messageName);
        }
        connection.on(messageName, callback);
      }

      listenerCount++;
    },
    removeListener: (callback: (data: T) => void) => {
      listenerCount--;
      if (listenerCount === 0) {
        if (isDev) {
          console.log('Remove listener for ' + messageName);
        }
        connection.off(messageName, callback);
      }
    },
  };
}
