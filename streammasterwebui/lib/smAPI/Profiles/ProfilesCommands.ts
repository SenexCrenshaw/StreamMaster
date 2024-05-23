import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,AddFFMPEGProfileRequest,RemoveFFMPEGProfileRequest,UpdateFFMPEGProfileRequest } from '@lib/smAPI/smapiTypes';

export const AddFFMPEGProfile = async (request: AddFFMPEGProfileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('AddFFMPEGProfile', request);
};

export const RemoveFFMPEGProfile = async (request: RemoveFFMPEGProfileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('RemoveFFMPEGProfile', request);
};

export const UpdateFFMPEGProfile = async (request: UpdateFFMPEGProfileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('UpdateFFMPEGProfile', request);
};

