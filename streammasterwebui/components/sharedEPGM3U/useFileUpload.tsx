import { useState, useCallback } from 'react';
import * as axios from 'axios';
import { upload as uploadService } from '@lib/FileUploadService';
import { isValidUrl } from '@lib/common/common';
import { useM3UFilesCreateM3UFileMutation } from '@lib/iptvApi';

interface UploadParams {
  name: string;
  source: string;
  fileName: string;
  fileType: 'epg' | 'm3u';
  maxStreams?: number;
  epgNumber?: number;
  timeShift?: number;
  color?: string;
  startingChannelNumber?: number;
  overwriteChannelNumbers?: boolean;
  vodTags: string[];
  file?: File;
}

export function useFileUpload() {
  const [progress, setProgress] = useState<number>(0);
  const [uploadedBytes, setUploadedBytes] = useState<number>(0);
  const [infoMessage, setInfoMessage] = useState<string | undefined>(undefined);
  const [isUploading, setIsUploading] = useState<boolean>(false);
  const [createM3UFile] = useM3UFilesCreateM3UFileMutation();

  const resetUploadState = useCallback(() => {
    setProgress(0);
    setUploadedBytes(0);
    setInfoMessage(undefined);
    setIsUploading(false);
  }, []);

  const doUpload = useCallback(
    async (params: UploadParams) => {
      setIsUploading(true);

      if (params.source && isValidUrl(params.source)) {
        // Handle creation from a source URL
        try {
          await createM3UFile({
            name: params.name,
            formFile: null,
            urlSource: params.source,
            maxStreamCount: params.maxStreams,
            startingChannelNumber: params.startingChannelNumber,
            vodTags: params.vodTags
          });
          setInfoMessage('M3U file created successfully.');
        } catch (error) {
          setInfoMessage(`Error creating ${params.fileType} file: ${error instanceof Error ? error.message : 'Unknown error'}`);
        } finally {
          setIsUploading(false);
        }
      } else if (params.file) {
        // Handle direct file upload
        try {
          await uploadService({
            name: params.name,
            source: params.source,
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
    },
    [createM3UFile]
  );

  return { doUpload, progress, uploadedBytes, infoMessage, isUploading, resetUploadState };
}
