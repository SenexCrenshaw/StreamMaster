import {APIResponse, PagedResponse, DefaultAPIResponse, SMStreamDto } from '@lib/apiDefs';
import { invokeHubCommand } from '@lib/signalr/signalr';

export const GetPagedSMStreams = async (parameters: QueryStringParameters): Promise<PagedResponse<SMStreamDto> | undefined> => {
  return await invokeHubCommand<APIResponse<SMStreamDto>>('GetPagedSMStreams', parameters)
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

export const ToggleSMStreamVisibleById = async (id: string): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('ToggleSMStreamVisibleById', id);
};

