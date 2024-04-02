import SignalRService from '@lib/signalr/SignalRService';
import { DefaultAPIResponse,CreateChannelGroupRequest,ChannelGroupDto,APIResponse,PagedResponse,QueryStringParameters } from '@lib/smAPI/smapiTypes';

export const CreateChannelGroup = async (request: CreateChannelGroupRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('CreateChannelGroup', request);
};

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

