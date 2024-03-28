import {DefaultAPIResponse,SMMessage,SendSMErrorRequest,SendSMInfoRequest,SendSMMessageRequest,SendSMWarnRequest,SendSuccessRequest} from '@lib/smAPI/smapiTypes';
import SignalRService from '@lib/signalr/SignalRService';

export const SendSMError = async (request: SendSMErrorRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('SendSMError', request);
};

export const SendSMInfo = async (request: SendSMInfoRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('SendSMInfo', request);
};

export const SendSMMessage = async (request: SendSMMessageRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('SendSMMessage', request);
};

export const SendSMWarn = async (request: SendSMWarnRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('SendSMWarn', request);
};

export const SendSuccess = async (request: SendSuccessRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('SendSuccess', request);
};

