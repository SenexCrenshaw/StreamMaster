import {IconFileDto} from '@lib/smAPI/smapiTypes';
import SignalRService from '@lib/signalr/SignalRService';

export const GetIcons = async (): Promise<IconFileDto[] | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<IconFileDto[]>('GetIcons');
};

