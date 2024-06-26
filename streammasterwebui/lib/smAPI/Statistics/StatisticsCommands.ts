import SignalRService from '@lib/signalr/SignalRService';
import { ChannelStreamingStatistics,ClientStreamingStatistics,StreamStreamingStatistic,GetStreamingStatisticsForChannelRequest } from '@lib/smAPI/smapiTypes';

export const GetChannelStreamingStatistics = async (): Promise<ChannelStreamingStatistics[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<ChannelStreamingStatistics[]>('GetChannelStreamingStatistics');
};

export const GetClientStreamingStatistics = async (): Promise<ClientStreamingStatistics[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<ClientStreamingStatistics[]>('GetClientStreamingStatistics');
};

export const GetStreamingStatisticsForChannel = async (request: GetStreamingStatisticsForChannelRequest): Promise<StreamStreamingStatistic[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<StreamStreamingStatistic[]>('GetStreamingStatisticsForChannel', request);
};

export const GetStreamStreamingStatistics = async (): Promise<StreamStreamingStatistic[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<StreamStreamingStatistic[]>('GetStreamStreamingStatistics');
};

