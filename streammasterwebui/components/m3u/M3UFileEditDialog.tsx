import SMDialog, { SMDialogRef } from '@components/sm/SMDialog';
import { UpdateM3UFile } from '@lib/smAPI/M3UFiles/M3UFilesCommands';
import { M3UFileDto, UpdateM3UFileRequest } from '@lib/smAPI/smapiTypes';
import { memo, useRef } from 'react';
import M3UFileDialog from './M3UFileDialog';

interface M3UFileEditDialogProperties {
  readonly selectedFile: M3UFileDto;
}

const M3UFileEditDialog = ({ selectedFile }: M3UFileEditDialogProperties) => {
  const smDialogRef = useRef<SMDialogRef>(null);
  function onUpdated(request: UpdateM3UFileRequest): void {
    if (request.Id === undefined) {
      return;
    }

    try {
      UpdateM3UFile(request);
    } catch (error) {
      console.error(error);
    } finally {
      smDialogRef.current?.close();
      console.log('finally');
    }
  }

  return (
    <SMDialog
      ref={smDialogRef}
      widthSize={6}
      position="top-right"
      title="EDIT M3U"
      icon="pi-pencil"
      iconFilled={false}
      buttonClassName="icon-yellow"
      tooltip="Add M3U"
      info="General"
    >
      <M3UFileDialog selectedFile={selectedFile} onUpdated={onUpdated} />
    </SMDialog>
  );
};

M3UFileEditDialog.displayName = 'M3UFileEditDialog';

export default memo(M3UFileEditDialog);
