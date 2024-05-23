import { memo } from 'react';
import { SMPopUp } from '@components/sm/SMPopUp';
import { DeleteEPGFileRequest, EPGFileDto } from '@lib/smAPI/smapiTypes';
import { DeleteEPGFile } from '@lib/smAPI/EPGFiles/EPGFilesCommands';

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
    <SMPopUp title="Delete EPG" OK={() => deleteFile()} icon="pi-times" severity="danger">
      <div>
        "{selectedFile.Name}"
        <br />
        Are you sure?
      </div>
    </SMPopUp>
  );
};

export default memo(EPGFileDeleteDialog);
