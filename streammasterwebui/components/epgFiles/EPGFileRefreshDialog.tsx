import { memo } from 'react';

import SMPopUp from '@components/sm/SMPopUp';
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
    <SMPopUp info="" title="Refresh EPG" onOkClick={() => accept()} icon="pi-sync" buttonClassName="icon-yellow" zIndex={10}>
      <div className="sm-center-stuff">
        <div className="text-container">{selectedFile.Name}</div>
      </div>
    </SMPopUp>
  );
};

EPGFileRefreshDialog.displayName = 'EPGFileRefreshDialog';

export default memo(EPGFileRefreshDialog);
