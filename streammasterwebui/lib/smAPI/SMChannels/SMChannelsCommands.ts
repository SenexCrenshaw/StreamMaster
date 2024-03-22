import {APIResponse, PagedResponse, DefaultAPIResponse,QueryStringParameters,SMChannelRankRequest, mainEntityName } from '@lib/apiDefs';
import { invokeHubCommand } from '@lib/signalr/signalr';

export const AddSMStreamToSMChannel = async (SMChannelId: number, SMStreamId: string): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('AddSMStreamToSMChannel', SMChannelId, SMStreamId);
};

export const CreateSMChannelFromStream = async (streamId: string): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('CreateSMChannelFromStream', streamId);
};

export const DeleteAllSMChannelsFromParameters = async (Parameters: QueryStringParameters): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('DeleteAllSMChannelsFromParameters', Parameters);
};

export const DeleteSMChannel = async (smChannelId: number): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('DeleteSMChannel', smChannelId);
};

export const DeleteSMChannels = async (smChannelIds: number[]): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('DeleteSMChannels', smChannelIds);
};

export const GetPagedSMChannels = async (parameters: QueryStringParameters): Promise<PagedResponse<SMChannelDto> | undefined> => {
  return await invokeHubCommand<APIResponse<mainEntityName>>('GetPagedSMChannels', parameters)
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

export const RemoveSMStreamFromSMChannel = async (SMChannelId: number, SMStreamId: string): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('RemoveSMStreamFromSMChannel', SMChannelId, SMStreamId);
};

export const SetSMChannelLogo = async (SMChannelId: number, logo: string): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('SetSMChannelLogo', SMChannelId, logo);
};

export const SetSMStreamRanks = async (requests: SMChannelRankRequest[]): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('SetSMStreamRanks', requests);
};

