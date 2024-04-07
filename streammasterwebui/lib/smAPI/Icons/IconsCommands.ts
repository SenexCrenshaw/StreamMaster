import SignalRService from '@lib/signalr/SignalRService';
import { IconFileDto } from '@lib/smAPI/smapiTypes';

export const GetIcons = async (): Promise<IconFileDto[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<IconFileDto[]>('GetIcons');
};

