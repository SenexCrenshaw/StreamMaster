import { QueryStringParameters } from '@lib/smAPI/smapiTypes';
export interface GetPagedSMStreamsRequest {
  parameters?: QueryStringParameters;
 }

export interface ToggleSMStreamVisibleByIdRequest {
  id?: string;
 }

