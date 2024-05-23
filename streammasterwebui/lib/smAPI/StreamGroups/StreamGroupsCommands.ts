import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,CreateStreamGroupRequest,DeleteStreamGroupRequest,StreamGroupDto,GetStreamGroupRequest,PagedResponse,QueryStringParameters } from '@lib/smAPI/smapiTypes';

export const GetPagedStreamGroups = async (parameters: QueryStringParameters): Promise<PagedResponse<StreamGroupDto> | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<PagedResponse<StreamGroupDto>>('GetPagedStreamGroups', parameters);
};

export const GetStreamGroup = async (request: GetStreamGroupRequest): Promise<StreamGroupDto | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<StreamGroupDto>('GetStreamGroup', request);
};

export const GetStreamGroups = async (): Promise<StreamGroupDto[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<StreamGroupDto[]>('GetStreamGroups');
};

export const CreateStreamGroup = async (request: CreateStreamGroupRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('CreateStreamGroup', request);
};

export const DeleteStreamGroup = async (request: DeleteStreamGroupRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('DeleteStreamGroup', request);
};

