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
    <div className="flex flex-row w-6 justify-content-center align-items-center ">
      <div className="w-full ">
        <StreamGroupSelector
          onChange={(sg) => {
            setSelectedStreamGroup(sg);
            setSelectedItems([sg]);
          }}
          selectedStreamGroup={selectedStreamGroup}
        />
      </div>

      <div className="pr-1" />

      <SMOverlay
        title="STREAM GROUPS"
        widthSize="5"
        icon="pi-file-edit"
        iconFilled
        buttonClassName="w-4rem icon-sg"
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
