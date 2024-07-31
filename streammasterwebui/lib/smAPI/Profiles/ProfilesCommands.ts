import { isSkipToken } from '@lib/common/isSkipToken';
import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,AddCommandProfileRequest,AddOutputProfileRequest,RemoveCommandProfileRequest,RemoveOutputProfileRequest,UpdateCommandProfileRequest,UpdateOutputProfileRequest,CommandProfileDto,OutputProfileDto,GetOutputProfileRequest } from '@lib/smAPI/smapiTypes';

export const GetCommandProfiles = async (): Promise<CommandProfileDto[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<CommandProfileDto[]>('GetCommandProfiles');
};

export const GetOutputProfile = async (request: GetOutputProfileRequest): Promise<OutputProfileDto | undefined> => {
  if ( request === undefined ) {
    return undefined;
  }
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<OutputProfileDto>('GetOutputProfile', request);
};

export const GetOutputProfiles = async (): Promise<OutputProfileDto[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<OutputProfileDto[]>('GetOutputProfiles');
};

export const AddCommandProfile = async (request: AddCommandProfileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('AddCommandProfile', request);
};

export const AddOutputProfile = async (request: AddOutputProfileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('AddOutputProfile', request);
};

export const RemoveCommandProfile = async (request: RemoveCommandProfileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('RemoveCommandProfile', request);
};

export const RemoveOutputProfile = async (request: RemoveOutputProfileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('RemoveOutputProfile', request);
};

export const UpdateCommandProfile = async (request: UpdateCommandProfileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('UpdateCommandProfile', request);
};

export const UpdateOutputProfile = async (request: UpdateOutputProfileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('UpdateOutputProfile', request);
};

