import {DefaultAPIResponse,SMMessage} from '@lib/apiDefs';
import { invokeHubCommand } from '@lib/signalr/signalr';

import { SendSMErrorRequest,SendSMInfoRequest,SendSMMessageRequest,SendSMWarnRequest,SendSuccessRequest } from './SMMessagesTypes';
export const SendSMError = async (request: SendSMErrorRequest): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('SendSMError', request);
};

export const SendSMInfo = async (request: SendSMInfoRequest): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('SendSMInfo', request);
};

export const SendSMMessage = async (request: SendSMMessageRequest): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('SendSMMessage', request);
};

export const SendSMWarn = async (request: SendSMWarnRequest): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('SendSMWarn', request);
};

export const SendSuccess = async (request: SendSuccessRequest): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('SendSuccess', request);
};

