import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,SendSMTasksRequest,SMTask } from '@lib/smAPI/smapiTypes';

export const GetSMTasks = async (): Promise<SMTask[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<SMTask[]>('GetSMTasks');
};

export const SendSMTasks = async (request: SendSMTasksRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('SendSMTasks', request);
};

