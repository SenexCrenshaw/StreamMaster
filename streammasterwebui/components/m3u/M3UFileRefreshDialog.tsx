import { SMPopUp } from '@components/sm/SMPopUp';
import { RefreshM3UFile } from '@lib/smAPI/M3UFiles/M3UFilesCommands';
import { M3UFileDto, RefreshM3UFileRequest } from '@lib/smAPI/smapiTypes';
import { memo } from 'react';

interface M3UFileRefreshDialogProperties {
  readonly selectedFile: M3UFileDto;
}

const M3UFileRefreshDialog = ({ selectedFile }: M3UFileRefreshDialogProperties) => {
  const accept = () => {
    const toSend = {} as RefreshM3UFileRequest;
    toSend.Id = selectedFile.Id;
    toSend.ForceRun = true;

    RefreshM3UFile(toSend)
      .then(() => {})
      .catch((error) => {
        console.error('Error refreshing M3U file', error);
      });
  };

  return (
    <SMPopUp placement="bottom-end" title="Refresh M3U" onOkClick={() => accept()} icon="pi-sync" buttonClassName="icon-orange" tooltip="Refresh M3U">
      <div className="sm-center-stuff">
        <div className="text-container">{selectedFile.Name}</div>
      </div>
    </SMPopUp>
  );
};

M3UFileRefreshDialog.displayName = 'M3UFileRefreshDialog';

export default memo(M3UFileRefreshDialog);
