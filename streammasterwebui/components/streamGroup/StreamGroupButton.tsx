import { SMOverlay } from '@components/sm/SMOverlay';
import { useSelectedItems } from '@lib/redux/slices/useSelectedItemsSlice';
import { useSelectedStreamGroup } from '@lib/redux/slices/useSelectedStreamGroup';
import { memo } from 'react';
import StreamGroupCreateDialog from './StreamGroupCreateDialog';
import StreamGroupDataSelector from './StreamGroupDataSelector';
import { StreamGroupSelector } from './StreamGroupSelector';

const StreamGroupButton = () => {
  const { setSelectSelectedItems } = useSelectedItems('selectedStreamGroup');
  const { selectedStreamGroup, setSelectedStreamGroup } = useSelectedStreamGroup('StreamGroup');

  return (
    <div className="flex flex-row">
      <StreamGroupSelector
        onChange={(sg) => {
          setSelectedStreamGroup(sg);
          setSelectSelectedItems([sg]);
        }}
        selectedStreamGroup={selectedStreamGroup}
      />
      <div className="pr-1" />
      <SMOverlay
        title="STREAM GROUPS"
        widthSize="5"
        icon="pi-file-edit"
        buttonClassName="icon-orange-filled"
        buttonLabel="SG"
        header={<StreamGroupCreateDialog />}
      >
        <StreamGroupDataSelector id={'StreamGroup'} />
      </SMOverlay>
    </div>
  );
};

StreamGroupButton.displayName = 'StreamGroupButton';

export interface M3UFilesEditorProperties {}

export default memo(StreamGroupButton);
