import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,SendSMErrorRequest,SendSMInfoRequest,SendSMMessageRequest,SendSMWarnRequest,SendSuccessRequest } from '@lib/smAPI/smapiTypes';

export const SendSMError = async (request: SendSMErrorRequest): Promise<APIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('SendSMError', request);
};

export const SendSMInfo = async (request: SendSMInfoRequest): Promise<APIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('SendSMInfo', request);
};

export const SendSMMessage = async (request: SendSMMessageRequest): Promise<APIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('SendSMMessage', request);
};

export const SendSMWarn = async (request: SendSMWarnRequest): Promise<APIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('SendSMWarn', request);
};

export const SendSuccess = async (request: SendSuccessRequest): Promise<APIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('SendSuccess', request);
};

