import { isSkipToken } from '@lib/common/isSkipToken';
import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,AddSMChannelsToStreamGroupByParametersRequest,AddSMChannelsToStreamGroupRequest,AddSMChannelToStreamGroupRequest,RemoveSMChannelFromStreamGroupRequest,SMChannelDto,GetStreamGroupSMChannelsRequest } from '@lib/smAPI/smapiTypes';

export const GetStreamGroupSMChannels = async (request: GetStreamGroupSMChannelsRequest): Promise<SMChannelDto[] | undefined> => {
  if ( request === undefined ) {
    return undefined;
  }
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<SMChannelDto[]>('GetStreamGroupSMChannels', request);
};

export const AddSMChannelsToStreamGroupByParameters = async (request: AddSMChannelsToStreamGroupByParametersRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('AddSMChannelsToStreamGroupByParameters', request);
};

export const AddSMChannelsToStreamGroup = async (request: AddSMChannelsToStreamGroupRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('AddSMChannelsToStreamGroup', request);
};

export const AddSMChannelToStreamGroup = async (request: AddSMChannelToStreamGroupRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('AddSMChannelToStreamGroup', request);
};

export const RemoveSMChannelFromStreamGroup = async (request: RemoveSMChannelFromStreamGroupRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('RemoveSMChannelFromStreamGroup', request);
};

