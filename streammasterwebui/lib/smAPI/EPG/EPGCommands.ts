import { isSkipToken } from '@lib/common/isSkipToken';
import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,EPGSyncRequest,EPGColorDto } from '@lib/smAPI/smapiTypes';

export const GetEPGColors = async (): Promise<EPGColorDto[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<EPGColorDto[]>('GetEPGColors');
};

export const EPGSync = async (): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('EPGSync');
};

