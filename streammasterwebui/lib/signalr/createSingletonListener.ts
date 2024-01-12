import { isDev as isDevelopment } from '../settings';

export interface SingletonListener {
  addListener: (callback: (data: any) => void) => void;
  removeListener: (callback: (data: any) => void) => void;
}

export function createSingletonListener<T>(messageName: string, connection: signalR.HubConnection): SingletonListener {
  const listenerCounts = new Map<string, number>();

  const addListener = (callback: (data: T) => void) => {
    const currentCount = listenerCounts.get(messageName) || 0;
    if (currentCount === 0) {
      if (isDevelopment) {
        console.log(`Add listener for ${messageName}`);
      }
      connection.on(messageName, callback);
    }
    listenerCounts.set(messageName, currentCount + 1);
  };

  const removeListener = (callback: (data: T) => void) => {
    const currentCount = listenerCounts.get(messageName) || 0;
    if (currentCount > 0) {
      listenerCounts.set(messageName, currentCount - 1);
      if (currentCount - 1 === 0) {
        if (isDevelopment) {
          console.log(`Remove listener for ${messageName}`);
        }
        connection.off(messageName, callback);
      }
    }
  };

  return { addListener, removeListener };
}
