import { SMCard } from '@components/SMCard';
import UploadButton from '@components/buttons/UploadButton';
import XButton from '@components/buttons/XButton';
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
      <OverlayPanel className="sm-overlay col-6 p-0 default-border" ref={op} showCloseIcon={false}>
        <SMCard
          title="M3U Files"
          header={
            <div className="justify-content-end flex-row flex">
              <M3UFileCreateDialog onUploadComplete={closeOverlay} />
              <XButton iconFilled={false} onClick={(e) => op.current?.toggle(e)} />
            </div>
          }
        >
          <M3UFilesDataSelector />
        </SMCard>
      </OverlayPanel>
    </>
  );
};

M3UFilesButton.displayName = 'M3UFilesButton';

export interface M3UFilesEditorProperties {}

export default memo(M3UFilesButton);
