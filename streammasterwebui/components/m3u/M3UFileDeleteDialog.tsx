import { memo } from 'react';
import { DeleteM3UFileRequest, M3UFileDto } from '@lib/smAPI/smapiTypes';
import { SMPopUp } from '@components/sm/SMPopUp';
import { DeleteM3UFile } from '@lib/smAPI/M3UFiles/M3UFilesCommands';

interface M3UFileDeleteDialogProperties {
  readonly selectedFile: M3UFileDto;
}

const M3UFileDeleteDialog = ({ selectedFile }: M3UFileDeleteDialogProperties) => {
  const deleteFile = () => {
    if (!selectedFile) {
      return;
    }

    const toSend = {} as DeleteM3UFileRequest;

    toSend.Id = selectedFile.Id;
    toSend.DeleteFile = true;

    DeleteM3UFile(toSend)
      .then(() => {})
      .catch((error) => {
        console.error(error);
        throw error;
      });
  };

  return (
    <SMPopUp title="Delete M3U" OK={() => deleteFile()} icon="pi-times" severity="danger" tooltip="Delete M3U">
      <div>
        "{selectedFile.Name}"
        <br />
        Are you sure?
      </div>
    </SMPopUp>
  );
};

export default memo(M3UFileDeleteDialog);
