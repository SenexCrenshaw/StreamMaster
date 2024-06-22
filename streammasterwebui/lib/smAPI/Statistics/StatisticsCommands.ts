import SignalRService from '@lib/signalr/SignalRService';
import { ClientStreamingStatistics, InputStreamingStatistics } from '@lib/smAPI/smapiTypes';

export const GetClientStreamingStatistics = async (): Promise<ClientStreamingStatistics[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<ClientStreamingStatistics[]>('GetClientStreamingStatistics');
};

export const GetInputStatistics = async (): Promise<InputStreamingStatistics[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<InputStreamingStatistics[]>('GetInputStatistics');
};
