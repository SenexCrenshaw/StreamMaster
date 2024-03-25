import { type AxiosProgressEvent } from 'axios';
import http from './axios';

export interface UploadProperties {
  fileType: 'epg' | 'm3u';
  file: File;
  name: string;
  maxStreamCount?: number;
  epgNumber?: number;
  timeShift?: number;
  color?: string;
  startingChannelNumber?: number;
  overWriteChannels?: boolean;
  vodTags?: string[];
  onUploadProgress: (progressEvent: AxiosProgressEvent) => void;
}

export const upload = async ({
  name,
  maxStreamCount,
  epgNumber,
  timeShift,
  color,
  startingChannelNumber,
  overWriteChannels,
  vodTags,
  file,
  fileType,
  onUploadProgress
}: UploadProperties) => {
  const formData = new FormData();

  formData.append('FormFile', file);
  formData.append('name', name);

  if (color) formData.append('color', color);
  if (overWriteChannels) formData.append('overWriteChannels', overWriteChannels?.toString() ?? 'true');
  if (maxStreamCount) formData.append('maxStreamCount', maxStreamCount?.toString() ?? '1');
  if (startingChannelNumber) formData.append('startingChannelNumber', startingChannelNumber?.toString() ?? '1');
  if (epgNumber) formData.append('epgNumber', epgNumber?.toString() ?? '1');
  if (timeShift) formData.append('timeShift', timeShift?.toString() ?? '0');
  if (vodTags)
    vodTags.forEach((tag) => {
      formData.append('vodTags[]', tag);
    });

  let url = '';

  switch (fileType) {
    case 'epg': {
      url = '/api/epgfiles/createepgfilefromform/';
      break;
    }
    case 'm3u': {
      url = '/api/m3ufiles/createm3ufilefromform';
      break;
    }
  }

  return await http.post(url, formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    },
    onUploadProgress
  });
};
