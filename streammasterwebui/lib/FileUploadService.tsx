import { type AxiosProgressEvent } from 'axios';
import http from './axios';

export interface UploadProperties {
  fileType: 'epg' | 'm3u';
  file: File;
  name: string;
  fileName: string;
  maxStreams?: number;
  epgNumber?: number;
  timeShift?: number;
  color?: string;
  startingChannelNumber?: number;
  overwriteChannelNumbers?: boolean;
  vodTags?: string[];

  onUploadProgress: (progressEvent: AxiosProgressEvent) => void;
}

export const upload = async ({
  name,
  fileName,
  maxStreams,
  epgNumber,
  timeShift,
  color,
  startingChannelNumber,
  overwriteChannelNumbers,
  vodTags,
  file,
  fileType,
  onUploadProgress
}: UploadProperties) => {
  const formData = new FormData();

  formData.append('FormFile', file);

  formData.append('name', name);
  formData.append('fileName', fileName);
  formData.append('fileSource', file.name);

  if (color) formData.append('color', color);
  if (overwriteChannelNumbers) formData.append('overWriteChannels', overwriteChannelNumbers?.toString() ?? 'true');
  if (maxStreams) formData.append('maxStreamCount', maxStreams?.toString() ?? '1');
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
