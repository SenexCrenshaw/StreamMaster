import { M3UFileStreamUrlPrefix } from '@lib/common/streammaster_enums';
import React, { useState } from 'react';
import FileDialog, { type FileDialogProperties } from '../sharedEPGM3U/FileDialog';

const M3UFileDialog: React.FC<Partial<FileDialogProperties>> = () => {
  const [infoMessage, setInfoMessage] = useState('');

  const [M3UFilesCreateM3UFileMutation] = useM3UFilesCreateM3UFileMutation();

  const createM3U = async (
    name: string,
    source: string,
    maxStreams: number,
    startingChannelNumber: number,
    streamURLPrefix: M3UFileStreamUrlPrefix,
    vodTags: string[]
  ) => {
    const addM3UFileRequest = {} as CreateM3UFileRequest;

    addM3UFileRequest.name = name;
    addM3UFileRequest.formFile = null;
    addM3UFileRequest.urlSource = source;
    addM3UFileRequest.maxStreamCount = maxStreams;
    addM3UFileRequest.startingChannelNumber = startingChannelNumber;
    addM3UFileRequest.vodTags = vodTags;
    // addM3UFileRequest.streamURLPrefixInt = parseInt(streamURLPrefix?.toString() ?? '0');

    await M3UFilesCreateM3UFileMutation(addM3UFileRequest)
      .then(() => {
        setInfoMessage('Uploaded M3U');
      })
      .catch((error) => {
        setInfoMessage(`Error Uploading M3U: ${error.message}`);
      });
  };

  return <FileDialog fileType="m3u" infoMessage={infoMessage} onCreateFromSource={createM3U} />;
};

M3UFileDialog.displayName = 'M3UFileDialog';

export default React.memo(M3UFileDialog);
