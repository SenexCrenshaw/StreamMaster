/* eslint-disable @typescript-eslint/no-explicit-any */
import { hubConnection } from './signalr';

export type SingletonListener = {
  addListener: (callback: (data:  any ) => void) => void,
  removeListener: (callback: (data: any) => void) => void,
};

function createSingletonListener<T> (messageName: string, connection: signalR.HubConnection): SingletonListener {
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

export const singletonChannelGroupsListener = createSingletonListener('ChannelGroupsRefresh', hubConnection);
export const singletonEPGFilesListener = createSingletonListener('EPGFilesRefresh', hubConnection);
export const singletonM3UFilesListener = createSingletonListener('M3UFilesRefresh', hubConnection);
export const singletonProgrammesListener = createSingletonListener('ProgrammesRefresh', hubConnection);
export const singletonSchedulesDirectListener = createSingletonListener('SchedulesDirectsRefresh', hubConnection);
export const singletonSettingsListener = createSingletonListener('SettingsRefresh', hubConnection);
export const singletonStreamGroupChannelGroupListener = createSingletonListener('StreamGroupChannelGroupsRefresh', hubConnection);
export const singletonStreamGroupVideoStreamsListener = createSingletonListener('StreamGroupVideoStreamsRefresh', hubConnection);
export const singletonStreamGroupsListener = createSingletonListener('StreamGroupsRefresh', hubConnection);
export const singletonVideoStreamLinksListener = createSingletonListener('VideoStreamLinksRefresh', hubConnection);
export const singletonVideoStreamLinksRemoveListener = createSingletonListener('VideoStreamLinksRemove', hubConnection);
export const singletonVideoStreamsListener = createSingletonListener('VideoStreamsRefresh', hubConnection);
export const singletonIconsListener = createSingletonListener('IconsRefresh', hubConnection);
export const singletonLogsListener  = createSingletonListener('LogsRefresh', hubConnection);
export const singletonStatisticListener  = createSingletonListener('streamstatisticsresultsupdate', hubConnection);
