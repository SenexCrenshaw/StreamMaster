import { memo, useRef } from 'react';

import { SMCard } from '@components/sm/SMCard';
import EditButton from '@components/buttons/EditButton';
import XButton from '@components/buttons/XButton';
import { UpdateM3UFile } from '@lib/smAPI/M3UFiles/M3UFilesCommands';
import { M3UFileDto, UpdateM3UFileRequest } from '@lib/smAPI/smapiTypes';
import { OverlayPanel } from 'primereact/overlaypanel';
import M3UFileDialog from './M3UFileDialog';

interface M3UFileEditDialogProperties {
  readonly selectedFile: M3UFileDto;
}

const M3UFileEditDialog = ({ selectedFile }: M3UFileEditDialogProperties) => {
  const op = useRef<OverlayPanel>(null);

  function onUpdated(request: UpdateM3UFileRequest): void {
    if (request.Id === undefined) {
      return;
    }

    UpdateM3UFile(request)
      .then(() => {})
      .catch((error) => {
        console.error(error);
      })
      .finally(() => {
        op.current?.hide();
      });
  }

  return (
    <>
      <EditButton iconFilled={false} onClick={(e) => op.current?.toggle(e)} tooltip="Edit" />
      <OverlayPanel className="col-5 p-0 sm-fileupload-panel default-border" ref={op} closeOnEscape>
        <SMCard title="EDIT M3U" header={<XButton iconFilled={false} onClick={(e) => op.current?.toggle(e)} tooltip="Close" />}>
          <div className="sm-fileupload col-12 p-0 m-0 ">
            <M3UFileDialog selectedFile={selectedFile} onUpdated={onUpdated} />
          </div>
        </SMCard>
      </OverlayPanel>
    </>
  );
};

M3UFileEditDialog.displayName = 'M3UFileEditDialog';

export default memo(M3UFileEditDialog);
