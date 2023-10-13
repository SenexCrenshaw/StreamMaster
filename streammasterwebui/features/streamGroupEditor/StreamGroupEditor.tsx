import { BlockUI } from 'primereact/blockui';
import { memo } from 'react';

import StandardHeader from '@/components/StandardHeader';
import { StreamGroupEditorIcon } from '@/lib/common/icons';
import { useSelectedStreamGroup } from '@/lib/redux/slices/useSelectedStreamGroup';
import StreamGroupDataSelector from './StreamGroupDataSelector';
import StreamGroupSelectedVideoStreamDataSelector from './StreamGroupSelectedVideoStreamDataSelector';
import StreamGroupVideoStreamDataSelector from './StreamGroupVideoStreamDataSelector';

const StreamGroupEditor = () => {
  const id = 'streamgroupeditor';
  const { selectedStreamGroup } = useSelectedStreamGroup(id);

  return (
    <StandardHeader displayName="Stream Group" icon={<StreamGroupEditorIcon />}>
      <div className="col-3 m-0 p-0 pr-1">
        <StreamGroupDataSelector id={id} />
      </div>

      <div className="col-9 m-0 p-0 pl-1">
        <BlockUI
          blocked={selectedStreamGroup === undefined || selectedStreamGroup.id === undefined || selectedStreamGroup.id <= 1 || selectedStreamGroup.isReadOnly}
        >
          <div className="grid grid-nogutter flex flex-wrap justify-content-between h-full col-12 p-0">
            <div className="col-6">
              <StreamGroupVideoStreamDataSelector id={id} />
            </div>
            <div className="col-6">
              <StreamGroupSelectedVideoStreamDataSelector id={id} />
            </div>
          </div>
        </BlockUI>
      </div>
    </StandardHeader>
  );
};

export default memo(StreamGroupEditor);
