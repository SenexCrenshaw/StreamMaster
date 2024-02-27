import { type AxiosProgressEvent } from 'axios';
import http from './axios';

export interface UploadProperties {
  name: string;
  source: string;
  fileName: string;
  maxStreams: number;
  epgNumber: number | undefined;
  timeShift: number | undefined;
  color: string;
  startingChannelNumber: number;
  overwriteChannelNumbers: boolean;
  vodTags: string[];
  file: File | undefined;
  fileType: 'epg' | 'm3u';
  onUploadProgress: (progressEvent: AxiosProgressEvent) => void;
}

export const upload = async ({
  name,
  source,
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

  if (file) {
    formData.append('FormFile', file);
  }

  formData.append('name', name);

  if (source) {
    formData.append('fileSource', source);
  } else if (file) formData.append('fileSource', file.name);

  formData.append('timeShift', timeShift?.toString() ?? '0');

  formData.append('fileName', fileName);
  formData.append('color', color);

  formData.append('color', color);
  formData.append('overWriteChannels', overwriteChannelNumbers?.toString() ?? 'true');
  formData.append('maxStreamCount', maxStreams?.toString() ?? '1');
  formData.append('startingChannelNumber', startingChannelNumber?.toString() ?? '1');
  formData.append('epgNumber', epgNumber?.toString() ?? '1');
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
