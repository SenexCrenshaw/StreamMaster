import SMPopUp from '@components/sm/SMPopUp';
import { DeleteM3UFile } from '@lib/smAPI/M3UFiles/M3UFilesCommands';
import { DeleteM3UFileRequest, M3UFileDto } from '@lib/smAPI/smapiTypes';
import { memo } from 'react';

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
    <SMPopUp
      buttonClassName="icon-red"
      modal
      zIndex={11}
      info=""
      placement="bottom-end"
      title="Delete"
      onOkClick={() => deleteFile()}
      icon="pi-times"
      tooltip="DeleteM3U"
    >
      <div className="sm-center-stuff">
        <div className="text-container">{selectedFile.Name}</div>
      </div>
    </SMPopUp>
  );
};

export default memo(M3UFileDeleteDialog);
