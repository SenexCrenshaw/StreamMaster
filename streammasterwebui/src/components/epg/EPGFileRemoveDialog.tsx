import { memo, useState } from "react";

import { type DeleteEpgFileRequest, type EpgFileDto } from '@/lib/iptvApi';
import { DeleteEpgFile } from "@/lib/smAPI/EpgFiles/EpgFilesMutateAPI";
import FileRemoveDialog from "../sharedEPGM3U/FileRemoveDialog";

const EPGFileRemoveDialog = (props: EPGFileRemoveDialogProps) => {
  const [infoMessage, setInfoMessage] = useState('');

  const deleteFile = () => {
    if (!props.selectedFile) {
      return;
    }

    const toSend = {} as DeleteEpgFileRequest;

    toSend.id = props.selectedFile.id;
    toSend.deleteFile = true;

    DeleteEpgFile(toSend)
      .then(() => {
        setInfoMessage('EPG Removed Successfully');
      }).catch((e) => {
        setInfoMessage('EPG Removed Error: ' + e.message);
      });

  };

  return (
    <FileRemoveDialog fileType="epg" infoMessage={infoMessage} onDeleteFile={deleteFile} />
  );
}

EPGFileRemoveDialog.displayName = 'EPGFileRemoveDialog';

type EPGFileRemoveDialogProps = {
  readonly selectedFile?: EpgFileDto;
};

export default memo(EPGFileRemoveDialog);
