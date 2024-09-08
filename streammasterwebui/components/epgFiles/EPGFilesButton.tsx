import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import { useIsTrue } from '@lib/redux/hooks/isTrue';
import { memo, useRef } from 'react';
import EPGFileCreateDialog from './EPGFileCreateDialog';
import EPGFilesDataSelector from './EPGFilesDataSelector';

const EPGFilesButton = () => {
  const ref = useRef<SMPopUpRef>(null);
  const closeOverlay = () => ref.current?.hide();
  const { isTrue: smTableIsSimple } = useIsTrue('isSimple');
  return (
    <SMPopUp
      buttonClassName="sm-w-4rem icon-green"
      buttonLabel="EPG"
      contentWidthSize="5"
      modal
      header={<EPGFileCreateDialog onUploadComplete={closeOverlay} />}
      icon="pi-upload"
      iconFilled
      info=""
      placement={smTableIsSimple ? 'bottom-end' : 'bottom'}
      ref={ref}
      title="EPG FILES"
    >
      <EPGFilesDataSelector />
    </SMPopUp>
  );
};

EPGFilesButton.displayName = 'EPGFilesButton';

export interface M3UFilesEditorProperties {}

export default memo(EPGFilesButton);
