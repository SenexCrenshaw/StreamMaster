import { isSkipToken } from '@lib/common/isSkipToken';
import SignalRService from '@lib/signalr/SignalRService';
import { ChannelMetric,ImageDownloadServiceStatus,StreamConnectionMetricData,SDSystemStatus,VideoInfo,VideoInfoDto,GetStreamConnectionMetricDataRequest,GetVideoInfoRequest } from '@lib/smAPI/smapiTypes';

export const GetChannelMetrics = async (): Promise<ChannelMetric[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<ChannelMetric[]>('GetChannelMetrics');
};

export const GetDownloadServiceStatus = async (): Promise<ImageDownloadServiceStatus | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<ImageDownloadServiceStatus>('GetDownloadServiceStatus');
};

export const GetIsSystemReady = async (): Promise<boolean | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<boolean>('GetIsSystemReady');
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

export const GetSystemStatus = async (): Promise<SDSystemStatus | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<SDSystemStatus>('GetSystemStatus');
};

export const GetTaskIsRunning = async (): Promise<boolean | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<boolean>('GetTaskIsRunning');
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

