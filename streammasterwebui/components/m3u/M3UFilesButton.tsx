import UploadButton from '@components/buttons/UploadButton';
import { OverlayPanel } from 'primereact/overlaypanel';
import { memo, useRef } from 'react';
import M3UFileCreateDialog from './M3UFileCreateDialog';
import M3UFilesDataSelector from './M3UFilesDataSelector';

const M3UFilesButton = () => {
  const op = useRef<OverlayPanel>(null);
  const closeOverlay = () => op.current?.hide();
  return (
    <>
      <UploadButton outlined={true} label="M3U" onClick={(e) => op.current?.toggle(e)} />
      <OverlayPanel className="col-6 p-0" ref={op} showCloseIcon={false}>
        <div className="filesEditor border-1 border-100 border-round-md">
          <div className="flex justify-content-between align-items-center px-1 header border-round-md">
            <span className="sm-text-color">M3U Files</span>
            <M3UFileCreateDialog onUploadComplete={closeOverlay} />
          </div>
          <M3UFilesDataSelector />
        </div>
      </OverlayPanel>
    </>
  );
};

M3UFilesButton.displayName = 'M3UFilesButton';

export interface M3UFilesEditorProperties {}

export default memo(M3UFilesButton);
