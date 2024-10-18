import { isSkipToken } from '@lib/common/isSkipToken';
import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,AddProfileToStreamGroupRequest,CreateStreamGroupRequest,DeleteStreamGroupRequest,RemoveStreamGroupProfileRequest,UpdateStreamGroupProfileRequest,UpdateStreamGroupRequest,StreamGroupDto,StreamGroupProfile,GetStreamGroupRequest,PagedResponse,QueryStringParameters } from '@lib/smAPI/smapiTypes';

export const GetPagedStreamGroups = async (parameters: QueryStringParameters): Promise<PagedResponse<StreamGroupDto> | undefined> => {
  if (isSkipToken(parameters) || parameters === undefined) {
    return undefined;
  }
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<PagedResponse<StreamGroupDto>>('GetPagedStreamGroups', parameters);
};

export const GetStreamGroupProfiles = async (): Promise<StreamGroupProfile[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<StreamGroupProfile[]>('GetStreamGroupProfiles');
};

export const GetStreamGroup = async (request: GetStreamGroupRequest): Promise<StreamGroupDto | undefined> => {
  if ( request === undefined ) {
    return undefined;
  }
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<StreamGroupDto>('GetStreamGroup', request);
};

export const GetStreamGroups = async (): Promise<StreamGroupDto[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<StreamGroupDto[]>('GetStreamGroups');
};

export const AddProfileToStreamGroup = async (request: AddProfileToStreamGroupRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('AddProfileToStreamGroup', request);
};

export const CreateStreamGroup = async (request: CreateStreamGroupRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('CreateStreamGroup', request);
};

export const DeleteStreamGroup = async (request: DeleteStreamGroupRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('DeleteStreamGroup', request);
};

export const RemoveStreamGroupProfile = async (request: RemoveStreamGroupProfileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('RemoveStreamGroupProfile', request);
};

export const UpdateStreamGroupProfile = async (request: UpdateStreamGroupProfileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('UpdateStreamGroupProfile', request);
};

export const UpdateStreamGroup = async (request: UpdateStreamGroupRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('UpdateStreamGroup', request);
};

