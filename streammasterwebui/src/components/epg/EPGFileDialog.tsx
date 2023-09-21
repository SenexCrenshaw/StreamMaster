
import React, { useState } from 'react';
import { useEpgFilesCreateEpgFileMutation, type CreateEpgFileRequest } from '../../store/iptvApi';
import FileDialog, { type FileDialogProps } from '../sharedEPGM3U/FileDialog';


const EPGFileDialog: React.FC<Partial<FileDialogProps>> = () => {
  const [infoMessage, setInfoMessage] = useState('');

  const [epgFilesCreateEpgFileMutation] = useEpgFilesCreateEpgFileMutation();

  const createEpg = async (name: string, source: string) => {
    const addEpgFileRequest = {} as CreateEpgFileRequest;

    addEpgFileRequest.name = name;
    addEpgFileRequest.formFile = null;
    addEpgFileRequest.urlSource = source;

    await epgFilesCreateEpgFileMutation(addEpgFileRequest)
      .then(() => {
        setInfoMessage('Uploaded EPG');
      }).catch((e) => {
        setInfoMessage(`Error Uploading EPG: ${e.message}`);
      });

  }

  return (
    <FileDialog fileType="epg" infoMessage={infoMessage} onCreateFromSource={createEpg} />
  )
}

EPGFileDialog.displayName = 'EPGFileDialog';

export default React.memo(EPGFileDialog);
