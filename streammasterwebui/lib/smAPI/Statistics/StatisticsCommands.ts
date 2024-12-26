import { isSkipToken } from '@lib/common/isSkipToken';
import SignalRService from '@lib/signalr/SignalRService';
import { ChannelMetric,StreamConnectionMetric,VideoInfo,VideoInfoDto,GetStreamConnectionMetricRequest,GetVideoInfoRequest } from '@lib/smAPI/smapiTypes';

export const GetChannelMetrics = async (): Promise<ChannelMetric[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<ChannelMetric[]>('GetChannelMetrics');
};

export const GetStreamConnectionMetric = async (request: GetStreamConnectionMetricRequest): Promise<StreamConnectionMetric | undefined> => {
  if ( request === undefined ) {
    return undefined;
  }
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<StreamConnectionMetric>('GetStreamConnectionMetric', request);
};

export const GetStreamConnectionMetrics = async (): Promise<StreamConnectionMetric[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<StreamConnectionMetric[]>('GetStreamConnectionMetrics');
};

export const GetVideoInfo = async (request: GetVideoInfoRequest): Promise<VideoInfo | undefined> => {
  if ( request === undefined ) {
    return undefined;
  }
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<VideoInfo>('GetVideoInfo', request);
};

export const GetVideoInfos = async (): Promise<VideoInfoDto[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<VideoInfoDto[]>('GetVideoInfos');
};

