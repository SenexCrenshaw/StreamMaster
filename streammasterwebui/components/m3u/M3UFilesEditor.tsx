import { memo, useRef } from 'react';
import M3UFileCreateDialog from './M3UFileCreateDialog';
import M3UFilesDataSelector from './M3UFilesDataSelector';
import { OverlayPanel } from 'primereact/overlaypanel';
import UploadButton from '@components/buttons/UploadButton';

const M3UFilesEditor = () => {
  const op = useRef<OverlayPanel>(null);
  const closeOverlay = () => op.current?.hide();
  return (
    <>
      <UploadButton outlined={true} label="M3U" onClick={(e) => op.current?.toggle(e)} />
      <OverlayPanel className="col-5 p-0" ref={op} showCloseIcon={false}>
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

M3UFilesEditor.displayName = 'M3UFilesEditor';

export interface M3UFilesEditorProperties {}

export default memo(M3UFilesEditor);
