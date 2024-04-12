import { memo } from 'react';
import { RefreshM3UFile } from '@lib/smAPI/M3UFiles/M3UFilesCommands';
import { M3UFileDto, RefreshM3UFileRequest } from '@lib/smAPI/smapiTypes';
import { SMPopUp } from '@components/sm/SMPopUp';

interface M3UFileRefreshDialogProperties {
  readonly selectedFile: M3UFileDto;
}

const M3UFileRefreshDialog = ({ selectedFile }: M3UFileRefreshDialogProperties) => {
  const accept = () => {
    const toSend = {} as RefreshM3UFileRequest;
    toSend.Id = selectedFile.Id;

    RefreshM3UFile(toSend)
      .then(() => {})
      .catch((error) => {
        console.error('Error refreshing M3U file', error);
      });
  };

  return (
    <SMPopUp title="Refresh M3U" OK={() => accept()} icon="pi-sync">
      <div>
        "{selectedFile.Name}"
        <br />
        Are you sure?
      </div>
    </SMPopUp>
  );
};

M3UFileRefreshDialog.displayName = 'M3UFileRefreshDialog';

export default memo(M3UFileRefreshDialog);
