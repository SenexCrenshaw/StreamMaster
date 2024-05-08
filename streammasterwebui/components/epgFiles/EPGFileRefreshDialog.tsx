import { memo } from 'react';

import { SMPopUp } from '@components/sm/SMPopUp';
import { RefreshEPGFile } from '@lib/smAPI/EPGFiles/EPGFilesCommands';
import { EPGFileDto, RefreshEPGFileRequest } from '@lib/smAPI/smapiTypes';

interface EPGFileRefreshDialogProperties {
  readonly selectedFile: EPGFileDto;
}

const EPGFileRefreshDialog = ({ selectedFile }: EPGFileRefreshDialogProperties) => {
  const accept = () => {
    const toSend = {} as RefreshEPGFileRequest;
    toSend.Id = selectedFile.Id;

    RefreshEPGFile(toSend)
      .then(() => {})
      .catch((error) => {
        console.error('Error refreshing EPG file', error);
      });
  };

  return (
    <SMPopUp title="Refresh EPG" OK={() => accept()} icon="pi-sync" buttonClassName="icon-yellow">
      <div>
        "{selectedFile.Name}"
        <br />
        Are you sure?
      </div>
    </SMPopUp>
  );
};

EPGFileRefreshDialog.displayName = 'EPGFileRefreshDialog';

export default memo(EPGFileRefreshDialog);
