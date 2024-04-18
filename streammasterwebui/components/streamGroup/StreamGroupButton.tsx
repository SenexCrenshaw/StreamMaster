import { SMCard } from '@components/sm/SMCard';
import XButton from '@components/buttons/XButton';
import { OverlayPanel } from 'primereact/overlaypanel';
import { memo, useRef } from 'react';
import AddButton from '@components/buttons/AddButton';
import StreamGroupDataSelector from './StreamGroupDataSelector';
import { StreamGroupCreateDialog } from './StreamGroupCreateDialog';

const StreamGroupButton = () => {
  const op = useRef<OverlayPanel>(null);
  // const closeOverlay = () => op.current?.hide();
  return (
    <>
      <AddButton tooltip="Stream Groups" outlined={true} label="SG" onClick={(e) => op.current?.toggle(e)} />
      <OverlayPanel className="sm-overlay col-6 p-0 default-border" ref={op} showCloseIcon={false}>
        <SMCard
          title="Stream Groups"
          header={
            <div className="justify-content-end flex-row flex">
              {/* <EPGFileCreateDialog onUploadComplete={closeOverlay} /> */}
              <StreamGroupCreateDialog onHide={() => {}} />
              <XButton iconFilled={false} onClick={(e) => op.current?.toggle(e)} />
            </div>
          }
        >
          <StreamGroupDataSelector id={'StreamGroup'} />
        </SMCard>
      </OverlayPanel>
    </>
  );
};

StreamGroupButton.displayName = 'StreamGroupButton';

export interface M3UFilesEditorProperties {}

export default memo(StreamGroupButton);
