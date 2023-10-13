import { type AxiosProgressEvent } from 'axios';
import http from './axios';
import { M3UFileStreamUrlPrefix } from './common/streammaster_enums';

export type UploadProps = {
  name: string | null;
  source: string | null;
  description: string | null;
  streamURLPrefix: M3UFileStreamUrlPrefix | null;
  file: File | undefined;
  fileType: 'epg' | 'm3u';
  onUploadProgress: (progressEvent: AxiosProgressEvent) => void;
};

export const upload = async ({ name, source, description, streamURLPrefix, file, fileType, onUploadProgress }: UploadProps) => {
  const formData = new FormData();

  if (file) {
    formData.append('FormFile', file);
  }

  if (name) formData.append('name', name);

  if (streamURLPrefix) formData.append('streamURLPrefixInt', streamURLPrefix?.toString() ?? '0');

  if (source) {
    formData.append('fileSource', source);
  } else {
    if (file) formData.append('fileSource', file.name);
  }

  if (description) formData.append('description', description);

  let url = '';

  switch (fileType) {
    case 'epg':
      url = '/api/epgfiles/createepgfilefromform/';
      break;
    case 'm3u':
      url = '/api/m3ufiles/createm3ufilefromform';
      break;
  }

  return await http.post(url, formData, {
    headers: {
      'Content-Type': 'multipart/form-data',
    },
    onUploadProgress,
  });
};
