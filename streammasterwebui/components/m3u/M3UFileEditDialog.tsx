import { memo, useRef } from 'react';

import EditButton from '@components/buttons/EditButton';
import XButton from '@components/buttons/XButton';
import { UpdateM3UFile } from '@lib/smAPI/M3UFiles/M3UFilesCommands';
import { M3UFileDto, UpdateM3UFileRequest } from '@lib/smAPI/smapiTypes';
import { OverlayPanel } from 'primereact/overlaypanel';
import M3UFileDialog from './M3UFileDialog';

interface MM3UFileEditDialogProperties {
  readonly selectedFile: M3UFileDto;
}

const M3UFileEditDialog = ({ selectedFile }: MM3UFileEditDialogProperties) => {
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
      <OverlayPanel className="col-5 p-0 smfileupload-panel default-border" ref={op} closeOnEscape>
        <div className="smfileupload col-12 p-0 m-0 ">
          <div className="smfileupload-header">
            <div className="flex justify-content-between align-items-center px-1 header">
              <span className="sm-text-color">EDIT M3U FILE</span>
              <span className="col-1">
                <XButton iconFilled={false} onClick={(e) => op.current?.toggle(e)} tooltip="Close" />
              </span>
            </div>
          </div>
          <M3UFileDialog selectedFile={selectedFile} onUpdated={onUpdated} />
        </div>
      </OverlayPanel>
    </>
  );
};

M3UFileEditDialog.displayName = 'M3UFileEditDialog';

export default memo(M3UFileEditDialog);
