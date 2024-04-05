import SignalRService from '@lib/signalr/SignalRService';
import { DefaultAPIResponse,CreateChannelGroupRequest,DeleteAllChannelGroupsFromParametersRequest,DeleteChannelGroupRequest,UpdateChannelGroupRequest,ChannelGroupDto,GetPagedChannelGroupsRequest,APIResponse,PagedResponse,QueryStringParameters } from '@lib/smAPI/smapiTypes';

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

export const CreateChannelGroup = async (request: CreateChannelGroupRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('CreateChannelGroup', request);
};

export const DeleteAllChannelGroupsFromParameters = async (request: DeleteAllChannelGroupsFromParametersRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('DeleteAllChannelGroupsFromParameters', request);
};

export const DeleteChannelGroup = async (request: DeleteChannelGroupRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('DeleteChannelGroup', request);
};

export const UpdateChannelGroup = async (request: UpdateChannelGroupRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('UpdateChannelGroup', request);
};

