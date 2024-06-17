import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,SetTestTaskRequest,SDSystemStatus } from '@lib/smAPI/smapiTypes';

export const GetIsSystemReady = async (): Promise<boolean | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<boolean>('GetIsSystemReady');
};

export const GetSystemStatus = async (): Promise<SDSystemStatus | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<SDSystemStatus>('GetSystemStatus');
};

export const GetTaskIsRunning = async (): Promise<boolean | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<boolean>('GetTaskIsRunning');
};

export const SetTestTask = async (request: SetTestTaskRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('SetTestTask', request);
};

