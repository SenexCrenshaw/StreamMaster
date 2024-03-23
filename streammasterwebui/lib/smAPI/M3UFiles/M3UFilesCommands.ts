import {DefaultAPIResponse,QueryStringParameters,M3UFileDto,APIResponse,PagedResponse} from '@lib/apiDefs';
import { invokeHubCommand } from '@lib/signalr/signalr';

import { CreateM3UFileRequest,ProcessM3UFileRequest } from './M3UFilesTypes';
export const CreateM3UFile = async (request: CreateM3UFileRequest): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('CreateM3UFile', request);
};

export const GetPagedM3UFiles = async (parameters: QueryStringParameters): Promise<PagedResponse<M3UFileDto> | undefined> => {
  return await invokeHubCommand<APIResponse<M3UFileDto>>('GetPagedM3UFiles', parameters)
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

export const ProcessM3UFile = async (request: ProcessM3UFileRequest): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('ProcessM3UFile', request);
};

