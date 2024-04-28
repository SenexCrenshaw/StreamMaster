import { memo, useState } from 'react';

import { SMDialog } from '@components/sm/SMDialog';
import { UpdateEPGFile } from '@lib/smAPI/EPGFiles/EPGFilesCommands';
import { EPGFileDto, UpdateEPGFileRequest } from '@lib/smAPI/smapiTypes';
import EPGFileDialog from './EPGFileDialog';

interface EPGFileEditDialogProperties {
  readonly selectedFile: EPGFileDto;
}

const EPGFileEditDialog = ({ selectedFile }: EPGFileEditDialogProperties) => {
  const [close, setClose] = useState<boolean>(false);

  function onUpdated(request: UpdateEPGFileRequest): void {
    if (request.Id === undefined) {
      return;
    }

    UpdateEPGFile(request)
      .then(() => {})
      .catch((error) => {
        console.error(error);
      })
      .finally(() => {
        setClose(true);
      });
  }

  return (
    <SMDialog
      close={close}
      title="EDIT EPG"
      onHide={() => {
        setClose(false);
      }}
      icon="pi-pencil"
      iconFilled={false}
      buttonClassName="icon-yellow"
      tooltip="Add EPG"
      info="General"
    >
      <EPGFileDialog selectedFile={selectedFile} onUpdated={onUpdated} />
    </SMDialog>
  );
};

EPGFileEditDialog.displayName = 'EPGFileEditDialog';

export default memo(EPGFileEditDialog);
