import { QueryStringParameters,SMChannelRankRequest } from '@lib/apiDefs';
export interface AddSMStreamToSMChannelRequest {
  SMChannelId: number;
  SMStreamId: string;
 }

export interface CreateSMChannelFromStreamRequest {
  streamId: string;
 }

export interface DeleteSMChannelRequest {
  smChannelId: number;
 }

export interface DeleteSMChannelsFromParametersRequest {
  Parameters: QueryStringParameters;
 }

export interface DeleteSMChannelsRequest {
  smChannelIds: number[];
 }

export interface GetPagedSMChannelsRequest {
  Parameters: QueryStringParameters;
 }

export interface RemoveSMStreamFromSMChannelRequest {
  SMChannelId: number;
  SMStreamId: string;
 }

export interface SetSMChannelLogoRequest {
  SMChannelId: number;
  logo: string;
 }

export interface SetSMStreamRanksRequest {
  requests: SMChannelRankRequest[];
 }

