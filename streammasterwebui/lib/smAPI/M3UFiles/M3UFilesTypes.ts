import { IFormFile } from '@lib/apiDefs';
export interface CreateM3UFileRequest {
  name: string;
  maxStreamCount: number;
  urlSource?: string;
  overWriteChannels?: boolean;
  startingChannelNumber?: number;
  formFile?: IFormFile;
  vODTags?: string[];
 }

export interface ProcessM3UFileRequest {
  m3UFileId: number;
  forceRun: boolean;
 }

