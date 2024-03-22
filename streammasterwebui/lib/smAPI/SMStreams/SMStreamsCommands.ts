import {QueryStringParameters,SMStreamDto,DefaultAPIResponse,APIResponse,PagedResponse} from '@lib/apiDefs';
import { invokeHubCommand } from '@lib/signalr/signalr';

import { ToggleSMStreamVisibleByIdRequest } from './SMStreamsTypes';
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

export const ToggleSMStreamVisibleById = async (request: ToggleSMStreamVisibleByIdRequest): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('ToggleSMStreamVisibleById', request);
};

