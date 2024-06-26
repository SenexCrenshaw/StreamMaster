import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,AddSMStreamToSMChannelRequest,RemoveSMStreamFromSMChannelRequest,SetSMStreamRanksRequest,SMStreamDto,GetSMChannelStreamsRequest } from '@lib/smAPI/smapiTypes';

export const GetSMChannelStreams = async (request: GetSMChannelStreamsRequest): Promise<SMStreamDto[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<SMStreamDto[]>('GetSMChannelStreams', request);
};

export const AddSMStreamToSMChannel = async (request: AddSMStreamToSMChannelRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('AddSMStreamToSMChannel', request);
};

export const RemoveSMStreamFromSMChannel = async (request: RemoveSMStreamFromSMChannelRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('RemoveSMStreamFromSMChannel', request);
};

export const SetSMStreamRanks = async (request: SetSMStreamRanksRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('SetSMStreamRanks', request);
};

