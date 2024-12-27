import { isSkipToken } from '@lib/common/isSkipToken';
import SignalRService from '@lib/signalr/SignalRService';
import { ChannelMetric,StreamConnectionMetricData,VideoInfo,VideoInfoDto,GetStreamConnectionMetricDataRequest,GetVideoInfoRequest } from '@lib/smAPI/smapiTypes';

export const GetChannelMetrics = async (): Promise<ChannelMetric[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<ChannelMetric[]>('GetChannelMetrics');
};

export const GetStreamConnectionMetricData = async (request: GetStreamConnectionMetricDataRequest): Promise<StreamConnectionMetricData | undefined> => {
  if ( request === undefined ) {
    return undefined;
  }
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<StreamConnectionMetricData>('GetStreamConnectionMetricData', request);
};

export const GetStreamConnectionMetricDatas = async (): Promise<StreamConnectionMetricData[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<StreamConnectionMetricData[]>('GetStreamConnectionMetricDatas');
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

