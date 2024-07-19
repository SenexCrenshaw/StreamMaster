import { useSelectedItems } from '@lib/redux/hooks/selectedItems';

import SMPopUp from '@components/sm/SMPopUp';
import { useSelectedStreamGroup } from '@lib/redux/hooks/selectedStreamGroup';
import { memo, useMemo } from 'react';
import StreamGroupCreateDialog from './StreamGroupCreateDialog';
import StreamGroupDataSelector from './StreamGroupDataSelector';
import { StreamGroupSelector } from './StreamGroupSelector';

interface StreamGroupButtonProperties {
  className?: string;
}

const StreamGroupButton = ({ className = 'sm-w-10rem sm-input-dark' }: StreamGroupButtonProperties) => {
  const { setSelectedItems } = useSelectedItems('selectedStreamGroup');
  const { selectedStreamGroup, setSelectedStreamGroup } = useSelectedStreamGroup('StreamGroup');

  const headerTemplate = useMemo(() => {
    return <StreamGroupCreateDialog />;
  }, []);

  return (
    <div className="flex justify-content-center align-items-center">
      <div className={className}>
        <StreamGroupSelector
          onChange={(sg) => {
            setSelectedStreamGroup(sg);
            setSelectedItems([sg]);
          }}
          selectedStreamGroup={selectedStreamGroup}
          zIndex={11}
        />
      </div>
      <div className="pr-1" />
      <div className="sm-w-4rem">
        <SMPopUp
          buttonClassName="sm-w-4rem icon-sg"
          buttonLabel="SG"
          contentWidthSize="5"
          header={headerTemplate}
          icon="pi-list-check"
          modal
          iconFilled
          showRemember={false}
          title="Stream Groups"
        >
          <StreamGroupDataSelector id={'StreamGroup'} />
        </SMPopUp>
      </div>
    </div>
  );
};

StreamGroupButton.displayName = 'StreamGroupButton';

export interface M3UFilesEditorProperties {}

export default memo(StreamGroupButton);
