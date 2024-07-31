import { isSkipToken } from '@lib/common/isSkipToken';
import SignalRService from '@lib/signalr/SignalRService';
import { ChannelDistributorDto,ChannelStreamingStatistics,ClientStreamingStatistics,StreamStreamingStatistic,GetStreamingStatisticsForChannelRequest } from '@lib/smAPI/smapiTypes';

export const GetChannelDistributors = async (): Promise<ChannelDistributorDto[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<ChannelDistributorDto[]>('GetChannelDistributors');
};

export const GetChannelStreamingStatistics = async (): Promise<ChannelStreamingStatistics[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<ChannelStreamingStatistics[]>('GetChannelStreamingStatistics');
};

export const GetClientStreamingStatistics = async (): Promise<ClientStreamingStatistics[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<ClientStreamingStatistics[]>('GetClientStreamingStatistics');
};

export const GetStreamingStatisticsForChannel = async (request: GetStreamingStatisticsForChannelRequest): Promise<StreamStreamingStatistic[] | undefined> => {
  if ( request === undefined ) {
    return undefined;
  }
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<StreamStreamingStatistic[]>('GetStreamingStatisticsForChannel', request);
};

export const GetStreamStreamingStatistics = async (): Promise<StreamStreamingStatistic[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<StreamStreamingStatistic[]>('GetStreamStreamingStatistics');
};

