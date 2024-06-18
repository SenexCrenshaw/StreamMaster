import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,AddOutputProfileRequest,AddVideoProfileRequest,RemoveOutputProfileRequest,RemoveVideoProfileRequest,UpdateOutputProfileRequest,UpdateVideoProfileRequest,OutputProfileDto,VideoOutputProfileDto,GetOutputProfileRequest } from '@lib/smAPI/smapiTypes';

export const GetOutputProfile = async (request: GetOutputProfileRequest): Promise<OutputProfileDto | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<OutputProfileDto>('GetOutputProfile', request);
};

export const GetOutputProfiles = async (): Promise<OutputProfileDto[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<OutputProfileDto[]>('GetOutputProfiles');
};

export const GetVideoProfiles = async (): Promise<VideoOutputProfileDto[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<VideoOutputProfileDto[]>('GetVideoProfiles');
};

export const AddOutputProfile = async (request: AddOutputProfileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('AddOutputProfile', request);
};

export const AddVideoProfile = async (request: AddVideoProfileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('AddVideoProfile', request);
};

export const RemoveOutputProfile = async (request: RemoveOutputProfileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('RemoveOutputProfile', request);
};

export const RemoveVideoProfile = async (request: RemoveVideoProfileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('RemoveVideoProfile', request);
};

export const UpdateOutputProfile = async (request: UpdateOutputProfileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('UpdateOutputProfile', request);
};

export const UpdateVideoProfile = async (request: UpdateVideoProfileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('UpdateVideoProfile', request);
};

