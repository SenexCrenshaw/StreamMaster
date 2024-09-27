import { isSkipToken } from '@lib/common/isSkipToken';
import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,AddSMChannelToSMChannelRequest,RemoveSMChannelFromSMChannelRequest,SetSMChannelRanksRequest,SMChannelDto,GetSMChannelChannelsRequest } from '@lib/smAPI/smapiTypes';

export const GetSMChannelChannels = async (request: GetSMChannelChannelsRequest): Promise<SMChannelDto[] | undefined> => {
  if ( request === undefined ) {
    return undefined;
  }
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<SMChannelDto[]>('GetSMChannelChannels', request);
};

export const AddSMChannelToSMChannel = async (request: AddSMChannelToSMChannelRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('AddSMChannelToSMChannel', request);
};

export const RemoveSMChannelFromSMChannel = async (request: RemoveSMChannelFromSMChannelRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('RemoveSMChannelFromSMChannel', request);
};

export const SetSMChannelRanks = async (request: SetSMChannelRanksRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('SetSMChannelRanks', request);
};

