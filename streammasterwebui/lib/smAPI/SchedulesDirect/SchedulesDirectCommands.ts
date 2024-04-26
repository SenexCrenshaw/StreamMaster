import SignalRService from '@lib/signalr/SignalRService';
import { StationChannelName } from '@lib/smAPI/smapiTypes';

export const GetStationChannelNames = async (): Promise<StationChannelName[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<StationChannelName[]>('GetStationChannelNames');
};

