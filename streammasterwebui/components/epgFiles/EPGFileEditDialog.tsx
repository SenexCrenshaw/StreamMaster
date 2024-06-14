import { memo, useRef } from 'react';

import { SMDialogRef } from '@components/sm/SMDialog';
import SMPopUp from '@components/sm/SMPopUp';
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
        smDialogRef.current?.hide();
      });
  }

  return (
    <SMPopUp
      contentWidthSize="4"
      title="EDIT EPG"
      icon="pi-pencil"
      modal
      placement="bottom-end"
      iconFilled={false}
      buttonClassName="icon-yellow"
      tooltip="Add EPG"
    >
      <EPGFileDialog selectedFile={selectedFile} onUpdated={onUpdated} />
    </SMPopUp>
  );
};

EPGFileEditDialog.displayName = 'EPGFileEditDialog';

export default memo(EPGFileEditDialog);
