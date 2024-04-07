import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,CreateChannelGroupRequest,DeleteAllChannelGroupsFromParametersRequest,DeleteChannelGroupRequest,UpdateChannelGroupRequest,ChannelGroupDto,GetPagedChannelGroupsRequest,PagedResponse,QueryStringParameters } from '@lib/smAPI/smapiTypes';

export const GetPagedChannelGroups = async (parameters: QueryStringParameters): Promise<PagedResponse<ChannelGroupDto> | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<PagedResponse<ChannelGroupDto>>('GetPagedChannelGroups', parameters);
};

export const CreateChannelGroup = async (request: CreateChannelGroupRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('CreateChannelGroup', request);
};

export const DeleteAllChannelGroupsFromParameters = async (request: DeleteAllChannelGroupsFromParametersRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('DeleteAllChannelGroupsFromParameters', request);
};

export const DeleteChannelGroup = async (request: DeleteChannelGroupRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('DeleteChannelGroup', request);
};

export const UpdateChannelGroup = async (request: UpdateChannelGroupRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('UpdateChannelGroup', request);
};

