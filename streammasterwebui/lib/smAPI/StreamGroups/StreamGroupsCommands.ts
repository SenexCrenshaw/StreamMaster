import SignalRService from '@lib/signalr/SignalRService';
import { StreamGroupDto,GetPagedStreamGroupsRequest,APIResponse,PagedResponse,QueryStringParameters } from '@lib/smAPI/smapiTypes';

export const GetPagedStreamGroups = async (parameters: QueryStringParameters): Promise<PagedResponse<StreamGroupDto> | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<PagedResponse<StreamGroupDto>>('GetPagedStreamGroups', parameters);
};

