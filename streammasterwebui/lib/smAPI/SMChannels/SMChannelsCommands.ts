import {DefaultAPIResponse,QueryStringParameters,SMChannelDto,APIResponse,PagedResponse} from '@lib/apiDefs';
import { invokeHubCommand } from '@lib/signalr/signalr';

import { AddSMStreamToSMChannelRequest,CreateSMChannelFromStreamRequest,DeleteSMChannelRequest,DeleteSMChannelsFromParametersRequest,DeleteSMChannelsRequest,RemoveSMStreamFromSMChannelRequest,SetSMChannelLogoRequest,SetSMStreamRanksRequest } from './SMChannelsTypes';
export const AddSMStreamToSMChannel = async (request: AddSMStreamToSMChannelRequest): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('AddSMStreamToSMChannel', request);
};

export const CreateSMChannelFromStream = async (request: CreateSMChannelFromStreamRequest): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('CreateSMChannelFromStream', request);
};

export const DeleteSMChannel = async (request: DeleteSMChannelRequest): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('DeleteSMChannel', request);
};

export const DeleteSMChannelsFromParameters = async (request: DeleteSMChannelsFromParametersRequest): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('DeleteSMChannelsFromParameters', request);
};

export const DeleteSMChannels = async (request: DeleteSMChannelsRequest): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('DeleteSMChannels', request);
};

export const GetPagedSMChannels = async (parameters: QueryStringParameters): Promise<PagedResponse<SMChannelDto> | undefined> => {
  return await invokeHubCommand<APIResponse<SMChannelDto>>('GetPagedSMChannels', parameters)
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

export const RemoveSMStreamFromSMChannel = async (request: RemoveSMStreamFromSMChannelRequest): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('RemoveSMStreamFromSMChannel', request);
};

export const SetSMChannelLogo = async (request: SetSMChannelLogoRequest): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('SetSMChannelLogo', request);
};

export const SetSMStreamRanks = async (request: SetSMStreamRanksRequest): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('SetSMStreamRanks', request);
};

