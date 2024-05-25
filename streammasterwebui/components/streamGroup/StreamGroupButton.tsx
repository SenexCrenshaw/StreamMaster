import SMOverlay from '@components/sm/SMOverlay';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';

import { useSelectedStreamGroup } from '@lib/redux/hooks/selectedStreamGroup';
import { memo } from 'react';
import StreamGroupCreateDialog from './StreamGroupCreateDialog';
import StreamGroupDataSelector from './StreamGroupDataSelector';
import { StreamGroupSelector } from './StreamGroupSelector';

const StreamGroupButton = () => {
  const { setSelectedItems } = useSelectedItems('selectedStreamGroup');
  const { selectedStreamGroup, setSelectedStreamGroup } = useSelectedStreamGroup('StreamGroup');

  return (
    <>
      <StreamGroupSelector
        onChange={(sg) => {
          setSelectedStreamGroup(sg);
          setSelectedItems([sg]);
        }}
        selectedStreamGroup={selectedStreamGroup}
      />

      <SMOverlay
        title="STREAM GROUPS"
        widthSize="5"
        icon="pi-file-edit"
        iconFilled
        buttonClassName="w-3rem icon-sg"
        buttonLabel="SG"
        header={<StreamGroupCreateDialog />}
      >
        <StreamGroupDataSelector id={'StreamGroup'} />
      </SMOverlay>
      <div className="pr-1" />
    </>
  );
};

StreamGroupButton.displayName = 'StreamGroupButton';

export interface M3UFilesEditorProperties {}

export default memo(StreamGroupButton);
