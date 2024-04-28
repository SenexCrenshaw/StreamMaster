import { SMOverlay } from '@components/sm/SMOverlay';
import { OverlayPanel } from 'primereact/overlaypanel';
import { memo, useRef } from 'react';
import EPGFileCreateDialog from './EPGFileCreateDialog';
import EPGFilesDataSelector from './EPGFilesDataSelector';

const EPGFilesButton = () => {
  const op = useRef<OverlayPanel>(null);
  const closeOverlay = () => op.current?.hide();
  return (
    <>
      <SMOverlay
        title="EPG FILES"
        widthSize="5"
        icon="pi-upload"
        buttonClassName="icon-green-filled"
        buttonLabel="EPG"
        header={<EPGFileCreateDialog onUploadComplete={closeOverlay} />}
      >
        <EPGFilesDataSelector />
      </SMOverlay>

      {/* <UploadButton tooltip="" className="sm-upload-button" outlined={true} label="EPG" onClick={(e) => op.current?.toggle(e)} />
      <OverlayPanel className="sm-overlay col-6 p-0 default-border" ref={op} showCloseIcon={false}>
        <SMCard
          title="EPG Files"
          header={
            <div className="justify-content-end flex-row flex">
              <EPGFileCreateDialog onUploadComplete={closeOverlay} />
              <XButton iconFilled={false} onClick={(e) => op.current?.toggle(e)} />
            </div>
          }
        >
          <EPGFilesDataSelector />
        </SMCard>
      </OverlayPanel> */}
    </>
  );
};

EPGFilesButton.displayName = 'EPGFilesButton';

export interface M3UFilesEditorProperties {}

export default memo(EPGFilesButton);
