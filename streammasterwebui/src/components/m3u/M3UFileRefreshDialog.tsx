import { memo, useState } from "react";

import { useM3UFilesRefreshM3UFileMutation, type EpgFileDto, type EpgFilesRefreshEpgFileApiArg } from '@/lib/iptvApi';
import FileRefreshDialog from "../sharedEPGM3U/FileRefreshDialog";

type M3UFileRefreshDialogProps = {
  readonly selectedFile: EpgFileDto;
}

const M3UFileRefreshDialog = ({ selectedFile }: M3UFileRefreshDialogProps) => {
  const [infoMessage, setInfoMessage] = useState('');
  const [m3uFilesRefreshM3UFileMutation] = useM3UFilesRefreshM3UFileMutation();

  const refreshFile = async () => {
    if (!selectedFile?.id) {
      return;
    }

    const toSend = {} as EpgFilesRefreshEpgFileApiArg;
    toSend.id = selectedFile.id;

    m3uFilesRefreshM3UFileMutation(toSend)
      .then(() => {
        setInfoMessage('M3U Refresh Successfully');
      }).catch((error) => {
        setInfoMessage('M3U Refresh Error: ' + error.message);
      });
  };

  return (
    <FileRefreshDialog fileType="m3u" inputInfoMessage={infoMessage} onRefreshFile={refreshFile} />
  );
}

M3UFileRefreshDialog.displayName = 'M3UFileRefreshDialog';


export default memo(M3UFileRefreshDialog);
