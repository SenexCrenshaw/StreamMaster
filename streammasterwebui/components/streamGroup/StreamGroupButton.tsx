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
    <div className="flex flex-row sm-w-6 justify-content-center align-items-center ">
      <div className="sm-w-8">
        <StreamGroupSelector
          onChange={(sg) => {
            setSelectedStreamGroup(sg);
            setSelectedItems([sg]);
          }}
          selectedStreamGroup={selectedStreamGroup}
        />
      </div>
      <div className="pr-1" />
      <div className="sm-w-4">
        <SMOverlay
          title="STREAM GROUPS"
          contentWidthSize="4"
          icon="pi-file-edit"
          iconFilled
          buttonClassName="w-4rem icon-sg"
          buttonLabel="SG"
          header={<StreamGroupCreateDialog />}
        >
          <StreamGroupDataSelector id={'StreamGroup'} />
        </SMOverlay>
      </div>
    </div>
  );
};

StreamGroupButton.displayName = 'StreamGroupButton';

export interface M3UFilesEditorProperties {}

export default memo(StreamGroupButton);
