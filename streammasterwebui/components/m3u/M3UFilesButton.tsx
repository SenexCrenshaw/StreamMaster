import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import { memo, useRef } from 'react';
import M3UFileCreateDialog from './M3UFileCreateDialog';
import M3UFilesDataSelector from './M3UFilesDataSelector';

const M3UFilesButton = () => {
  const op = useRef<SMPopUpRef>(null);
  const closeOverlay = () => op.current?.hide();

  return (
    <SMPopUp
      buttonClassName="sm-w-4rem icon-green"
      buttonLabel="M3U"
      contentWidthSize="4"
      header={<M3UFileCreateDialog onUploadComplete={closeOverlay} />}
      icon="pi-upload"
      iconFilled
      info=""
      modal
      onOkClick={undefined}
      placement="bottom-end"
      ref={op}
      title="M3U FILES"
    >
      <M3UFilesDataSelector />
    </SMPopUp>
  );
};

M3UFilesButton.displayName = 'M3UFilesButton';

export interface M3UFilesEditorProperties {}

export default memo(M3UFilesButton);
