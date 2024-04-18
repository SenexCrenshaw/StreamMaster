import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,AddSMChannelToStreamGroupRequest,RemoveSMChannelFromStreamGroupRequest,SMChannelDto,GetStreamGroupSMChannelsRequest } from '@lib/smAPI/smapiTypes';

export const GetStreamGroupSMChannels = async (request: GetStreamGroupSMChannelsRequest): Promise<SMChannelDto[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<SMChannelDto[]>('GetStreamGroupSMChannels', request);
};

export const AddSMChannelToStreamGroup = async (request: AddSMChannelToStreamGroupRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('AddSMChannelToStreamGroup', request);
};

export const RemoveSMChannelFromStreamGroup = async (request: RemoveSMChannelFromStreamGroupRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('RemoveSMChannelFromStreamGroup', request);
};

