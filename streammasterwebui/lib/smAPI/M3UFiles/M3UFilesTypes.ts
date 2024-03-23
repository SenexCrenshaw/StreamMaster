import { IFormFile,QueryStringParameters } from '@lib/apiDefs';
export interface CreateM3UFileRequest {
  name: string;
  maxStreamCount: number;
  urlSource?: string;
  overWriteChannels?: boolean;
  startingChannelNumber?: number;
  formFile?: IFormFile;
  vODTags?: string[];
 }

export interface DeleteM3UFileRequest {
  deleteFile: boolean;
  id: number;
 }

export interface GetPagedM3UFilesRequest {
  parameters?: QueryStringParameters;
 }

export interface ProcessM3UFileRequest {
  m3UFileId: number;
  forceRun: boolean;
 }

