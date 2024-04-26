import { memo, useCallback, useState } from 'react';

import FileRefreshDialog from '../sharedEPGM3U/FileRefreshDialog';

interface EPGFileRefreshDialogProperties {
  readonly selectedFile: EpgFileDto;
}

const EPGFileRefreshDialog = ({ selectedFile }: EPGFileRefreshDialogProperties) => {
  const [infoMessage, setInfoMessage] = useState('');
  const [epgFilesRefreshEpgFileMutation] = useEpgFilesRefreshEpgFileMutation();

  const OnClose = useCallback(() => {
    setInfoMessage('');
  }, []);

  const refreshFile = async () => {
    if (!selectedFile?.id) {
      return;
    }

    const toSend = {} as EpgFilesRefreshEpgFileApiArg;
    toSend.id = selectedFile.id;

    epgFilesRefreshEpgFileMutation(toSend)
      .then(() => {
        setInfoMessage('EPG Refresh Successfully');
      })
      .catch((error) => {
        setInfoMessage(`EPG Refresh Error: ${error.message}`);
      });
  };

  return <FileRefreshDialog fileType="epg" inputInfoMessage={infoMessage} onRefreshFile={refreshFile} OnClose={OnClose} />;
};

EPGFileRefreshDialog.displayName = 'EPGFileRefreshDialog';

export default memo(EPGFileRefreshDialog);
