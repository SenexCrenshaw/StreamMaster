import SignalRService from '@lib/signalr/SignalRService';
import { StreamGroupDto,APIResponse,PagedResponse,QueryStringParameters } from '@lib/smAPI/smapiTypes';

export const GetPagedStreamGroups = async (parameters: QueryStringParameters): Promise<PagedResponse<StreamGroupDto> | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse<StreamGroupDto>>('GetPagedStreamGroups', parameters)
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

