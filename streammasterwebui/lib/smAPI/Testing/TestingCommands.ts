import {SMMessage,DefaultAPIResponse} from '@lib/apiDefs';
import { invokeHubCommand } from '@lib/signalr/signalr';

import { SendSMMessageRequest } from './TestingTypes';
export const SendSMMessage = async (request: SendSMMessageRequest): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('SendSMMessage', request);
};

