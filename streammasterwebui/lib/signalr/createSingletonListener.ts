

export type SingletonListener = {
  addListener: (callback: (data:  any ) => void) => void,
  removeListener: (callback: (data: any) => void) => void,
};

export function createSingletonListener<T> (messageName: string, connection: signalR.HubConnection): SingletonListener {
  let listenerCount = 0;

  return {
    addListener: (callback: (data: T) => void) => {
      if (listenerCount === 0) {
        console.log('Add listener for ' + messageName);
        connection.on(messageName, callback);
      }

      listenerCount++;
    },
    removeListener: (callback: (data: T) => void) => {
      listenerCount--;
      if (listenerCount === 0) {
        console.log('Remove listener for ' + messageName);
        connection.off(messageName, callback);
      }
    },
  };
}
