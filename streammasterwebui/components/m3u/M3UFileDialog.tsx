import { memo, useCallback, useRef } from 'react';

import EditButton from '@components/buttons/EditButton';
import XButton from '@components/buttons/XButton';
import { M3UFileDto } from '@lib/smAPI/smapiTypes';
import { OverlayPanel } from 'primereact/overlaypanel';
import M3UFileEditDialog from './M3UFileEditDialog';

interface M3UFileDialogProperties {
  readonly selectedFile: M3UFileDto;
}

const M3UFileDialog = ({ selectedFile }: M3UFileDialogProperties) => {
  const op = useRef<OverlayPanel>(null);

  const ReturnToParent = useCallback(() => {}, []);

  return (
    <>
      <EditButton iconFilled={false} onClick={(e) => op.current?.toggle(e)} tooltip="Edit" />
      <OverlayPanel className="col-5 p-0 smfileupload-panel streammaster-border" ref={op} closeOnEscape>
        <div className="smfileupload col-12 p-0 m-0 ">
          <div className="smfileupload-header">
            <div className="flex justify-content-between align-items-center px-1 header">
              <span className="sm-text-color">EDIT M3U FILE</span>
              <span className="col-1">
                <XButton iconFilled={false} onClick={(e) => op.current?.toggle(e)} />
              </span>
            </div>
          </div>
          <M3UFileEditDialog selectedFile={selectedFile} />
        </div>
      </OverlayPanel>
    </>
  );
};

M3UFileDialog.displayName = 'M3UFileDialog';

export default memo(M3UFileDialog);
