import SignalRService from '@lib/signalr/SignalRService';
import { SMStreamDto,GetSMChannelStreamsRequest } from '@lib/smAPI/smapiTypes';

export const GetSMChannelStreams = async (request: GetSMChannelStreamsRequest): Promise<SMStreamDto[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<SMStreamDto[]>('GetSMChannelStreams', request);
};

