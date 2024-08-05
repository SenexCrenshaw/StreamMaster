import { isSkipToken } from '@lib/common/isSkipToken';
import SignalRService from '@lib/signalr/SignalRService';
import { ChannelMetric } from '@lib/smAPI/smapiTypes';

export const GetChannelMetrics = async (): Promise<ChannelMetric[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<ChannelMetric[]>('GetChannelMetrics');
};

