import SignalRService from '@lib/signalr/SignalRService';
import { DefaultAPIResponse,AddSMStreamToSMChannelRequest,CreateSMChannelFromStreamRequest,DeleteSMChannelRequest,DeleteSMChannelsFromParametersRequest,DeleteSMChannelsRequest,RemoveSMStreamFromSMChannelRequest,SetSMChannelLogoRequest,SetSMChannelNameRequest,SetSMChannelNumberRequest,SetSMStreamRanksRequest,SMChannelDto,GetPagedSMChannelsRequest,APIResponse,PagedResponse,QueryStringParameters } from '@lib/smAPI/smapiTypes';

export const GetPagedSMChannels = async (parameters: QueryStringParameters): Promise<PagedResponse<SMChannelDto> | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<PagedResponse<SMChannelDto>>('GetPagedSMChannels', parameters)
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

export const AddSMStreamToSMChannel = async (request: AddSMStreamToSMChannelRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('AddSMStreamToSMChannel', request);
};

export const CreateSMChannelFromStream = async (request: CreateSMChannelFromStreamRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('CreateSMChannelFromStream', request);
};

export const DeleteSMChannel = async (request: DeleteSMChannelRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('DeleteSMChannel', request);
};

export const DeleteSMChannelsFromParameters = async (request: DeleteSMChannelsFromParametersRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('DeleteSMChannelsFromParameters', request);
};

export const DeleteSMChannels = async (request: DeleteSMChannelsRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('DeleteSMChannels', request);
};

export const RemoveSMStreamFromSMChannel = async (request: RemoveSMStreamFromSMChannelRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('RemoveSMStreamFromSMChannel', request);
};

export const SetSMChannelLogo = async (request: SetSMChannelLogoRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('SetSMChannelLogo', request);
};

export const SetSMChannelName = async (request: SetSMChannelNameRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('SetSMChannelName', request);
};

export const SetSMChannelNumber = async (request: SetSMChannelNumberRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('SetSMChannelNumber', request);
};

export const SetSMStreamRanks = async (request: SetSMStreamRanksRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('SetSMStreamRanks', request);
};

