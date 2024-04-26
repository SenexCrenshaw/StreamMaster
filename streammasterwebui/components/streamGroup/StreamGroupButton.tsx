import AddButton from '@components/buttons/AddButton';
import XButton from '@components/buttons/XButton';
import { SMCard } from '@components/sm/SMCard';
import { useSelectedItems } from '@lib/redux/slices/useSelectedItemsSlice';
import { useSelectedStreamGroup } from '@lib/redux/slices/useSelectedStreamGroup';
import { OverlayPanel } from 'primereact/overlaypanel';
import { memo, useRef } from 'react';
import { StreamGroupCreateDialog } from './StreamGroupCreateDialog';
import StreamGroupDataSelector from './StreamGroupDataSelector';
import { StreamGroupSelector } from './StreamGroupSelector';

const StreamGroupButton = () => {
  const op = useRef<OverlayPanel>(null);
  const { setSelectSelectedItems } = useSelectedItems('selectedStreamGroup');
  const { selectedStreamGroup, setSelectedStreamGroup } = useSelectedStreamGroup('StreamGroup');

  return (
    <div className="flex flex-row">
      <StreamGroupSelector
        onChange={(sg) => {
          console.log('OnChange', sg);
          setSelectedStreamGroup(sg);
          setSelectSelectedItems([sg]);
        }}
        selectedStreamGroup={selectedStreamGroup}
      />
      <div className="pr-1" />
      <AddButton tooltip="Stream Groups" outlined={true} label="SG" onClick={(e) => op.current?.toggle(e)} />
      <OverlayPanel className="sm-overlay col-6 p-0 default-border" ref={op} showCloseIcon={false}>
        <SMCard
          title="Stream Groups"
          header={
            <div className="justify-content-end flex-row flex">
              <StreamGroupCreateDialog onHide={() => {}} />
              <XButton iconFilled={false} onClick={(e) => op.current?.toggle(e)} />
            </div>
          }
        >
          <StreamGroupDataSelector id={'StreamGroup'} />
        </SMCard>
      </OverlayPanel>
    </div>
  );
};

StreamGroupButton.displayName = 'StreamGroupButton';

export interface M3UFilesEditorProperties {}

export default memo(StreamGroupButton);
