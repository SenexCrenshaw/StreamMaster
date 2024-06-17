import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,AddFileProfileRequest,AddVideoProfileRequest,RemoveFileProfileRequest,RemoveVideoProfileRequest,UpdateFileProfileRequest,UpdateVideoProfileRequest,FileOutputProfileDto,VideoOutputProfileDto } from '@lib/smAPI/smapiTypes';

export const GetFileProfiles = async (): Promise<FileOutputProfileDto[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<FileOutputProfileDto[]>('GetFileProfiles');
};

export const GetVideoProfiles = async (): Promise<VideoOutputProfileDto[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<VideoOutputProfileDto[]>('GetVideoProfiles');
};

export const AddFileProfile = async (request: AddFileProfileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('AddFileProfile', request);
};

export const AddVideoProfile = async (request: AddVideoProfileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('AddVideoProfile', request);
};

export const RemoveFileProfile = async (request: RemoveFileProfileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('RemoveFileProfile', request);
};

export const RemoveVideoProfile = async (request: RemoveVideoProfileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('RemoveVideoProfile', request);
};

export const UpdateFileProfile = async (request: UpdateFileProfileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('UpdateFileProfile', request);
};

export const UpdateVideoProfile = async (request: UpdateVideoProfileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('UpdateVideoProfile', request);
};

