import { useState, useCallback } from 'react';
import * as axios from 'axios';
import { upload as uploadService } from '@lib/FileUploadService';

export interface BaseUploadParams {
  name: string;
  fileName: string;
}

export type UploadParamsSettings = {
  fileType: 'epg' | 'm3u';
  maxStreams?: number;
  epgNumber?: number;
  timeShift?: number;
  color?: string;
  startingChannelNumber?: number;
  overwriteChannelNumbers?: boolean;
  vodTags?: string[];
  file?: File;
};

export type UploadParams = UploadParamsSettings & BaseUploadParams & {};

export function useFileUpload() {
  const [progress, setProgress] = useState<number>(0);
  const [uploadedBytes, setUploadedBytes] = useState<number>(0);
  const [infoMessage, setInfoMessage] = useState<string | undefined>(undefined);
  const [isUploading, setIsUploading] = useState<boolean>(false);

  const resetUploadState = useCallback(() => {
    setProgress(0);
    setUploadedBytes(0);
    setInfoMessage(undefined);
    setIsUploading(false);
  }, []);

  const doUpload = useCallback(async (params: UploadParams) => {
    if (params.file) {
      setIsUploading(true);
      try {
        await uploadService({
          name: params.name,
          fileName: params.fileName,
          maxStreams: params.maxStreams,
          epgNumber: params.epgNumber,
          timeShift: params.timeShift,
          color: params.color,
          startingChannelNumber: params.startingChannelNumber,
          overwriteChannelNumbers: params.overwriteChannelNumbers,
          vodTags: params.vodTags,
          file: params.file,
          fileType: params.fileType,
          onUploadProgress: (event: axios.AxiosProgressEvent) => {
            setUploadedBytes(event.loaded);
            const total = event.total === undefined ? 1 : event.total;
            const prog = Math.round((100 * event.loaded) / total);
            setProgress(prog);
          }
        });
        setInfoMessage('M3U file uploaded successfully.');
      } catch (error) {
        if (axios.isAxiosError(error)) {
          setInfoMessage(`Error uploading file: ${error.message}`);
        } else {
          setInfoMessage('An unexpected error occurred during upload.');
        }
      } finally {
        setIsUploading(false);
      }
    }
  }, []);

  return { doUpload, progress, uploadedBytes, infoMessage, isUploading, resetUploadState };
}
