import StandardHeader from '@components/StandardHeader';
import { StreamGroupEditorIcon } from '@lib/common/icons';
import { useSelectedStreamGroup } from '@lib/redux/slices/useSelectedStreamGroup';
import { BlockUI } from 'primereact/blockui';
import { memo } from 'react';
import StreamGroupDataSelector from './StreamGroupDataSelector';
import StreamGroupSelectedVideoStreamDataSelector from './StreamGroupSelectedVideoStreamDataSelector';
import StreamGroupVideoStreamDataSelector from './StreamGroupVideoStreamDataSelector';

// const StandardHeader = React.lazy(() => import('@components/StandardHeader'));

// const StreamGroupDataSelector = React.lazy(() => import('./StreamGroupDataSelector'));
// const StreamGroupSelectedVideoStreamDataSelector = React.lazy(() => import('./StreamGroupSelectedVideoStreamDataSelector'));
// const StreamGroupVideoStreamDataSelector = React.lazy(() => import('./StreamGroupVideoStreamDataSelector'));

const StreamGroupEditor = (): JSX.Element => {
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
