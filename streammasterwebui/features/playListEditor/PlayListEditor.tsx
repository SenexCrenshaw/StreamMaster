import StandardHeader from '@components/StandardHeader';
import { PlayListEditorIcon } from '@lib/common/icons';
import { memo } from 'react';
import ChannelGroupVideoStreamDataSelector from './ChannelGroupVideoStreamDataSelector';

// const StandardHeader = React.lazy(() => import('@components/StandardHeader'));

// const ChannelGroupVideoStreamDataSelector = React.lazy(() => import('./ChannelGroupVideoStreamDataSelector'));
// const PlayListDataSelector = React.lazy(() => import('./PlayListDataSelector'));

const PlayListEditor = () => {
  const id = 'playlisteditor';

  return (
    <StandardHeader className="playListEditor" displayName="PLAYLIST" icon={<PlayListEditorIcon />}>
      {/* <div className="col-3 m-0 p-0 pr-1">
        <PlayListDataSelectorDropDown id={id} />
      </div> */}
      <div className="col-12 m-0 p-0">
        <ChannelGroupVideoStreamDataSelector id={id} />
      </div>
    </StandardHeader>
  );
};

PlayListEditor.displayName = 'Playlist Editor';

export default memo(PlayListEditor);
