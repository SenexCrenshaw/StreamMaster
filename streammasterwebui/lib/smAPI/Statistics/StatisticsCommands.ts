import { isSkipToken } from '@lib/common/isSkipToken';
import SignalRService from '@lib/signalr/SignalRService';
import { ChannelMetric,VideoInfo,GetVideoInfoRequest } from '@lib/smAPI/smapiTypes';

export const GetChannelMetrics = async (): Promise<ChannelMetric[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<ChannelMetric[]>('GetChannelMetrics');
};

export const GetVideoInfo = async (request: GetVideoInfoRequest): Promise<VideoInfo | undefined> => {
  if ( request === undefined ) {
    return undefined;
  }
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<VideoInfo>('GetVideoInfo', request);
};

