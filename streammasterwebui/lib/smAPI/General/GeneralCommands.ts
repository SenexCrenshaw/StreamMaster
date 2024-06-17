import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,SetTestTaskRequest } from '@lib/smAPI/smapiTypes';

export const SetTestTask = async (request: SetTestTaskRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('SetTestTask', request);
};

