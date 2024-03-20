import {APIResponse, PagedResponse, DefaultAPIResponse,QueryStringParameters,SMStreamSMChannelRequest, SMChannelDto } from '@lib/apiDefs';
import { invokeHubCommand } from '@lib/signalr/signalr';

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

export const CreateSMChannelFromStream = async (streamId: string): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('CreateSMChannelFromStream', streamId);
};

export const DeleteSMChannels = async (smchannelIds: number[]): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('DeleteSMChannels', smchannelIds);
};

export const DeleteSMChannel = async (smchannelId: number): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('DeleteSMChannel', smchannelId);
};

export const DeleteAllSMChannelsFromParameters = async (Parameters: QueryStringParameters): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('DeleteAllSMChannelsFromParameters', Parameters);
};

export const AddSMStreamToSMChannel = async (request: SMStreamSMChannelRequest): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('AddSMStreamToSMChannel', request);
};

export const RemoveSMStreamFromSMChannel = async (request: SMStreamSMChannelRequest): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('RemoveSMStreamFromSMChannel', request);
};

