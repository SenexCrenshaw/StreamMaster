import SMPopUp from '@components/sm/SMPopUp';
import { DeleteEPGFile } from '@lib/smAPI/EPGFiles/EPGFilesCommands';
import { DeleteEPGFileRequest, EPGFileDto } from '@lib/smAPI/smapiTypes';
import { memo } from 'react';

interface EPGFileDeleteDialogProperties {
  readonly selectedFile: EPGFileDto;
}

const EPGFileDeleteDialog = ({ selectedFile }: EPGFileDeleteDialogProperties) => {
  const deleteFile = () => {
    if (!selectedFile) {
      return;
    }

    const toSend = {} as DeleteEPGFileRequest;

    toSend.Id = selectedFile.Id;
    toSend.DeleteFile = true;

    DeleteEPGFile(toSend)
      .then(() => {})
      .catch((error) => {
        console.error(error);
        throw error;
      });
  };

  return (
    <SMPopUp info="" title="Delete EPG" onOkClick={() => deleteFile()} icon="pi-times" zIndex={10}>
      <div className="sm-center-stuff">
        <div className="text-container">{selectedFile.Name}</div>
      </div>
    </SMPopUp>
  );
};

export default memo(EPGFileDeleteDialog);
