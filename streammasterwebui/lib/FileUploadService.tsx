import { type AxiosProgressEvent } from 'axios';
import http from './axios';
import { M3UField, M3UKey } from './smAPI/smapiTypes';

export interface UploadProperties {
  autoSetChannelNumbers?: boolean;
  color?: string;
  defaultStreamGroupName?: string;
  epgNumber?: number;
  file: File;
  fileType: 'epg' | 'm3u';
  maxStreamCount?: number;
  m3uKey?: M3UKey;
  m3uName?: M3UField;
  m3U8OutPutProfile?: string;
  name: string;
  overWriteChannels?: boolean;
  startingChannelNumber?: number;
  syncChannels?: boolean;
  timeShift?: number;
  vodTags?: string[];
  onUploadProgress: (progressEvent: AxiosProgressEvent) => void;
}

export const uploadToAPI = async ({
  autoSetChannelNumbers,
  color,
  defaultStreamGroupName,
  epgNumber,
  file,
  fileType,
  maxStreamCount,
  m3uKey,
  m3uName,
  m3U8OutPutProfile,
  name,
  overWriteChannels,
  onUploadProgress,
  startingChannelNumber,
  syncChannels,
  timeShift,
  vodTags
}: UploadProperties) => {
  const formData = new FormData();

  formData.append('FormFile', file);
  formData.append('name', name);

  if (defaultStreamGroupName) formData.append('defaultStreamGroupName', defaultStreamGroupName.toString());
  if (syncChannels) formData.append('syncChannels', syncChannels?.toString() ?? 'false');

  if (color) formData.append('color', color);
  if (overWriteChannels) formData.append('overWriteChannels', overWriteChannels?.toString() ?? 'true');
  if (maxStreamCount) formData.append('maxStreamCount', maxStreamCount?.toString() ?? '1');
  if (startingChannelNumber) formData.append('startingChannelNumber', startingChannelNumber?.toString() ?? '1');

  if (m3uKey) formData.append('m3uKey', m3uKey.toString());
  if (m3uName) formData.append('m3uName', m3uName.toString());
  if (m3U8OutPutProfile) formData.append('m3U8OutPutProfile', m3U8OutPutProfile);

  if (autoSetChannelNumbers) formData.append('autoSetChannelNumbers', autoSetChannelNumbers?.toString() ?? 'false');
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

  return await http
    .post(url, formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      },
      onUploadProgress
    })
    .then((response) => {
      return response.data;
    })
    .catch((error) => {
      console.error('Error uploading file', error);
      throw error;
    });
};
