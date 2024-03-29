import { M3UFileStreamUrlPrefix } from '@lib/common/streammaster_enums';

import React, { useState } from 'react';
import FileDialog, { type FileDialogProperties } from '../sharedEPGM3U/FileDialog';

const EPGFileDialog: React.FC<Partial<FileDialogProperties>> = () => {
  const [infoMessage, setInfoMessage] = useState('');

  const [epgFilesCreateEpgFileMutation] = useEpgFilesCreateEpgFileMutation();

  const createEpg = async (
    name: string,
    source: string,
    maxStreams: number,
    startingChannelNumber: number,
    streamURLPrefix: M3UFileStreamUrlPrefix,
    vodTags: string[],
    epgNumber: number,
    timeShift: number,
    color: string
  ) => {
    const addEpgFileRequest = {} as CreateEpgFileRequest;

    addEpgFileRequest.color = color;
    addEpgFileRequest.epgNumber = epgNumber;
    addEpgFileRequest.name = name;
    addEpgFileRequest.formFile = null;
    addEpgFileRequest.urlSource = source;
    addEpgFileRequest.timeShift = timeShift;

    await epgFilesCreateEpgFileMutation(addEpgFileRequest)
      .then(() => {
        setInfoMessage('Uploaded EPG');
      })
      .catch((error) => {
        setInfoMessage(`Error Uploading EPG: ${error.message}`);
      });
  };

  return <FileDialog fileType="epg" infoMessage={infoMessage} onCreateFromSource={createEpg} />;
};

EPGFileDialog.displayName = 'EPGFileDialog';

export default React.memo(EPGFileDialog);
