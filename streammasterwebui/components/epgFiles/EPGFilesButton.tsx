import SMOverlay, { SMOverlayRef } from '@components/sm/SMOverlay';
import { useIsTrue } from '@lib/redux/hooks/isTrue';
import { memo, useRef } from 'react';
import EPGFileCreateDialog from './EPGFileCreateDialog';
import EPGFilesDataSelector from './EPGFilesDataSelector';

const EPGFilesButton = () => {
  const op = useRef<SMOverlayRef>(null);
  const closeOverlay = () => op.current?.hide();
  const { isTrue: smTableIsSimple } = useIsTrue('isSimple');
  return (
    <SMOverlay
      buttonClassName="sm-w-4rem icon-green"
      buttonLabel="EPG"
      contentWidthSize="5"
      header={<EPGFileCreateDialog onUploadComplete={closeOverlay} />}
      icon="pi-upload"
      iconFilled
      info=""
      placement={smTableIsSimple ? 'bottom-end' : 'bottom'}
      ref={op}
      title="EPG FILES"
    >
      <EPGFilesDataSelector />
    </SMOverlay>
  );
};

EPGFilesButton.displayName = 'EPGFilesButton';

export interface M3UFilesEditorProperties {}

export default memo(EPGFilesButton);
