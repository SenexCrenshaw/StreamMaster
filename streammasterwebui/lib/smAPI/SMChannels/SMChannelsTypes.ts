import { QueryStringParameters } from '@lib/apiDefs';
export interface AddSMStreamToSMChannelRequest {
  sMChannelId: number;
  sMStreamId?: string;
}

export interface CreateSMChannelFromStreamRequest {
  streamId?: string;
}

export interface DeleteSMChannelRequest {
  smChannelId: number;
}

export interface DeleteSMChannelsFromParametersRequest {
  parameters?: QueryStringParameters;
}

export interface DeleteSMChannelsRequest {
  smChannelIds?: number[];
}

export interface GetPagedSMChannelsRequest {
  parameters?: QueryStringParameters;
}

export interface RemoveSMStreamFromSMChannelRequest {
  sMChannelId: number;
  sMStreamId?: string;
}

export interface SetSMChannelLogoRequest {
  sMChannelId: number;
  logo?: string;
}

export interface SetSMStreamRanksRequest {
  requests?: AddSMStreamToSMChannelRequest[];
}
