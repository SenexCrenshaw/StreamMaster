import { SMOverlayRef } from '@components/sm/SMOverlay';
import SMPopUp from '@components/sm/SMPopUp';
import { useIsTrue } from '@lib/redux/hooks/isTrue';
import { memo, useRef } from 'react';
import EPGFileCreateDialog from './EPGFileCreateDialog';
import EPGFilesDataSelector from './EPGFilesDataSelector';

const EPGFilesButton = () => {
  const op = useRef<SMOverlayRef>(null);
  const closeOverlay = () => op.current?.hide();
  const { isTrue: smTableIsSimple } = useIsTrue('isSimple');
  return (
    <SMPopUp
      buttonClassName="sm-w-4rem icon-green"
      buttonLabel="EPG"
      contentWidthSize="5"
      header={<EPGFileCreateDialog onUploadComplete={closeOverlay} />}
      icon="pi-upload"
      iconFilled
      info=""
      placement={smTableIsSimple ? 'bottom-end' : 'bottom'}
      title="EPG FILES"
    >
      <EPGFilesDataSelector />
    </SMPopUp>
  );
};

EPGFilesButton.displayName = 'EPGFilesButton';

export interface M3UFilesEditorProperties {}

export default memo(EPGFilesButton);
