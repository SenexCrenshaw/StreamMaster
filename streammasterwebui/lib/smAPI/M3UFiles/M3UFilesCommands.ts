import {DefaultAPIResponse} from '@lib/apiDefs';
import { invokeHubCommand } from '@lib/signalr/signalr';

import { ProcessM3UFileRequest } from './M3UFilesTypes';
export const ProcessM3UFile = async (request: ProcessM3UFileRequest): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('ProcessM3UFile', request);
};

