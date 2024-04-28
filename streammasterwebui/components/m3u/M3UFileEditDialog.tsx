import { SMDialog } from '@components/sm/SMDialog';
import { UpdateM3UFile } from '@lib/smAPI/M3UFiles/M3UFilesCommands';
import { M3UFileDto, UpdateM3UFileRequest } from '@lib/smAPI/smapiTypes';
import { memo, useState } from 'react';
import M3UFileDialog from './M3UFileDialog';

interface M3UFileEditDialogProperties {
  readonly selectedFile: M3UFileDto;
}

const M3UFileEditDialog = ({ selectedFile }: M3UFileEditDialogProperties) => {
  const [close, setClose] = useState<boolean>(false);

  function onUpdated(request: UpdateM3UFileRequest): void {
    if (request.Id === undefined) {
      return;
    }

    try {
      UpdateM3UFile(request);
    } catch (error) {
      console.error(error);
    } finally {
      setClose(true);
      console.log('finally');
    }
  }

  return (
    <SMDialog
      close={close}
      widthSize={6}
      position="top-right"
      title="EDIT M3U"
      onHide={() => {
        setClose(false);
      }}
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
