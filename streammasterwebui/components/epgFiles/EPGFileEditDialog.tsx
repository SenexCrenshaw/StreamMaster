import { memo, useRef } from 'react';

import SMDialog, { SMDialogRef } from '@components/sm/SMDialog';
import { UpdateEPGFile } from '@lib/smAPI/EPGFiles/EPGFilesCommands';
import { EPGFileDto, UpdateEPGFileRequest } from '@lib/smAPI/smapiTypes';
import EPGFileDialog from './EPGFileDialog';

interface EPGFileEditDialogProperties {
  readonly selectedFile: EPGFileDto;
}

const EPGFileEditDialog = ({ selectedFile }: EPGFileEditDialogProperties) => {
  const smDialogRef = useRef<SMDialogRef>(null);

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
        smDialogRef.current?.close();
      });
  }

  return (
    <SMDialog ref={smDialogRef} title="EDIT EPG" icon="pi-pencil" iconFilled={false} buttonClassName="icon-yellow" tooltip="Add EPG" info="General">
      <EPGFileDialog selectedFile={selectedFile} onUpdated={onUpdated} />
    </SMDialog>
  );
};

EPGFileEditDialog.displayName = 'EPGFileEditDialog';

export default memo(EPGFileEditDialog);
