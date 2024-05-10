import { SMOverlay } from '@components/sm/SMOverlay';
import { OverlayPanel } from 'primereact/overlaypanel';
import { memo, useRef } from 'react';
import EPGFileCreateDialog from './EPGFileCreateDialog';
import EPGFilesDataSelector from './EPGFilesDataSelector';

const EPGFilesButton = () => {
  const op = useRef<OverlayPanel>(null);
  const closeOverlay = () => op.current?.hide();

  return (
    <SMOverlay
      title="EPG FILES"
      widthSize="5"
      icon="pi-upload"
      iconFilled
      buttonClassName="w-4rem icon-green"
      buttonLabel="EPG"
      header={<EPGFileCreateDialog onUploadComplete={closeOverlay} />}
    >
      <EPGFilesDataSelector />
    </SMOverlay>
  );
};

EPGFilesButton.displayName = 'EPGFilesButton';

export interface M3UFilesEditorProperties {}

export default memo(EPGFilesButton);
