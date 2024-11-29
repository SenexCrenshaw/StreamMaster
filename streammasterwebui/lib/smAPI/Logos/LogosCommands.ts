import { isSkipToken } from '@lib/common/isSkipToken';
import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,RemoveCustomLogoRequest,AddCustomLogoRequest,CustomLogoDto,LogoDto,GetLogoForChannelRequest,GetLogoRequest } from '@lib/smAPI/smapiTypes';

export const GetCustomLogos = async (): Promise<CustomLogoDto[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<CustomLogoDto[]>('GetCustomLogos');
};

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

export const GetLogos = async (): Promise<CustomLogoDto[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<CustomLogoDto[]>('GetLogos');
};

export const RemoveCustomLogo = async (request: RemoveCustomLogoRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('RemoveCustomLogo', request);
};

export const AddCustomLogo = async (request: AddCustomLogoRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('AddCustomLogo', request);
};

