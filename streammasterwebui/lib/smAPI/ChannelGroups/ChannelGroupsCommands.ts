import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,CreateChannelGroupRequest,DeleteAllChannelGroupsFromParametersRequest,DeleteChannelGroupRequest,UpdateChannelGroupRequest,ChannelGroupDto,GetPagedChannelGroupsRequest,PagedResponse,QueryStringParameters } from '@lib/smAPI/smapiTypes';

export const GetPagedChannelGroups = async (parameters: QueryStringParameters): Promise<PagedResponse<ChannelGroupDto> | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<PagedResponse<ChannelGroupDto>>('GetPagedChannelGroups', parameters)
    .then((response) => {
      if (response) {
        return response;
      }
      return undefined;
    })
    .catch((error) => {
      console.error(error);
      return undefined;
    });
};

export const CreateChannelGroup = async (request: CreateChannelGroupRequest): Promise<APIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('CreateChannelGroup', request);
};

export const DeleteAllChannelGroupsFromParameters = async (request: DeleteAllChannelGroupsFromParametersRequest): Promise<APIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('DeleteAllChannelGroupsFromParameters', request);
};

export const DeleteChannelGroup = async (request: DeleteChannelGroupRequest): Promise<APIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('DeleteChannelGroup', request);
};

export const UpdateChannelGroup = async (request: UpdateChannelGroupRequest): Promise<APIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('UpdateChannelGroup', request);
};

