import { memo, useCallback, useState } from 'react';

import { RefreshM3UFile } from '@lib/smAPI/M3UFiles/M3UFilesCommands';
import { M3UFileDto, RefreshM3UFileRequest } from '@lib/smAPI/smapiTypes';
import FileRefreshDialog from '../sharedEPGM3U/FileRefreshDialog';

interface M3UFileRefreshDialogProperties {
  readonly selectedFile: M3UFileDto;
}

const M3UFileRefreshDialog = ({ selectedFile }: M3UFileRefreshDialogProperties) => {
  const [infoMessage, setInfoMessage] = useState('');

  const OnClose = useCallback(() => {
    setInfoMessage('');
  }, []);

  const refreshFile = async () => {
    if (!selectedFile?.id) {
      return;
    }

    const toSend = {} as RefreshM3UFileRequest;
    toSend.id = selectedFile.id;

    RefreshM3UFile(toSend)
      .then(() => {
        setInfoMessage('M3U Refresh Successfully');
      })
      .catch((error) => {
        setInfoMessage(`M3U Refresh Error: ${error.message}`);
      });
  };

  return <FileRefreshDialog fileType="m3u" inputInfoMessage={infoMessage} onRefreshFile={refreshFile} OnClose={OnClose} />;
};

M3UFileRefreshDialog.displayName = 'M3UFileRefreshDialog';

export default memo(M3UFileRefreshDialog);
