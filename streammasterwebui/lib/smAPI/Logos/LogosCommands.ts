import { isSkipToken } from '@lib/common/isSkipToken';
import SignalRService from '@lib/signalr/SignalRService';
import { LogoFileDto } from '@lib/smAPI/smapiTypes';

export const GetLogos = async (): Promise<LogoFileDto[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<LogoFileDto[]>('GetLogos');
};

