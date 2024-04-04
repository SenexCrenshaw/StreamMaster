import SignalRService from '@lib/signalr/SignalRService';
import { DefaultAPIResponse,CreateChannelGroupRequest,DeleteAllChannelGroupsFromParametersRequest,DeleteChannelGroupRequest,ChannelGroupDto,UpdateChannelGroupRequest,APIResponse,PagedResponse,QueryStringParameters } from '@lib/smAPI/smapiTypes';

export const GetPagedChannelGroups = async (parameters: QueryStringParameters): Promise<PagedResponse<ChannelGroupDto> | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse<ChannelGroupDto>>('GetPagedChannelGroups', parameters)
    .then((response) => {
      if (response) {
        return response.pagedResponse;
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

export const DeleteAllChannelGroupsFromParameters = async (request: DeleteAllChannelGroupsFromParametersRequest): Promise<boolean | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<boolean>('DeleteAllChannelGroupsFromParameters', request);
};

export const DeleteChannelGroup = async (request: DeleteChannelGroupRequest): Promise<boolean | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<boolean>('DeleteChannelGroup', request);
};

export const UpdateChannelGroup = async (request: UpdateChannelGroupRequest): Promise<ChannelGroupDto | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<ChannelGroupDto>('UpdateChannelGroup', request);
};

