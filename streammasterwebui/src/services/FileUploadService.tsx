import { type AxiosProgressEvent } from 'axios';
import http from '../app/axios';

export const upload = async (
  name: string | null,
  source: string | null,
  description: string | null,
  file: File | undefined,
  fileType: string,
  onUploadProgress: (progressEvent: AxiosProgressEvent) => void
) => {
  const formData = new FormData();

  if (file) {
    formData.append('FormFile', file);
  }

  if (name) formData.append('name', name);

  if (source) {
    formData.append('fileSource', source);
  } else {
    if (file) formData.append('fileSource', file.name);
  }

  if (description) formData.append('description', description);

  let url = '';
  switch (fileType) {
    case 'epg':
      url = '/api/epgfiles/addepgfilefromform/'
      break;
    case 'm3u':
      url = '/api/m3ufiles/addm3ufilefromform/'
      break;
    case 'icon':
      url = '/api/icons/addiconfilefromform/'
      break;
  }

  return await http.post(url, formData, {
    headers: {
      'Content-Type': 'multipart/form-data',
    },
    onUploadProgress,
  });
};
