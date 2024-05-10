import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,SetSMChannelsLogoFromEPGFromParametersRequest,SetSMChannelsLogoFromEPGRequest } from '@lib/smAPI/smapiTypes';

export const SetSMChannelsLogoFromEPGFromParameters = async (request: SetSMChannelsLogoFromEPGFromParametersRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('SetSMChannelsLogoFromEPGFromParameters', request);
};

export const SetSMChannelsLogoFromEPG = async (request: SetSMChannelsLogoFromEPGRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('SetSMChannelsLogoFromEPG', request);
};

