import { isSkipToken } from '@lib/common/isSkipToken';
import SignalRService from '@lib/signalr/SignalRService';
import { LogoDto,LogoFileDto,GetLogoForChannelRequest,GetLogoRequest } from '@lib/smAPI/smapiTypes';

export const GetLogoForChannel = async (request: GetLogoForChannelRequest): Promise<LogoDto | undefined> => {
  if ( request === undefined ) {
    return undefined;
  }
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<LogoDto>('GetLogoForChannel', request);
};

export const GetLogo = async (request: GetLogoRequest): Promise<LogoDto | undefined> => {
  if ( request === undefined ) {
    return undefined;
  }
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<LogoDto>('GetLogo', request);
};

export const GetLogos = async (): Promise<LogoFileDto[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<LogoFileDto[]>('GetLogos');
};

