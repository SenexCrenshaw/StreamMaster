import StandardHeader from '@components/StandardHeader';
import ChannelGroupDataSelector from '@components/channelGroups/ChannelGroupDataSelector';
import { PlayListEditorIcon } from '@lib/common/icons';
import { memo } from 'react';
import SMStreamDataSelector from './SMStreamDataSelector';

const StreamEditor = () => {
  const id = 'streameditor';

  return (
    <StandardHeader className="streamEditor" displayName="STREAMS" icon={<PlayListEditorIcon />}>
      <div className="col-4 m-0 p-0">
        <ChannelGroupDataSelector id={id} />
      </div>
      <div className="col-8 m-0 p-0">
        <SMStreamDataSelector id={id} />
      </div>
    </StandardHeader>
  );
};

StreamEditor.displayName = 'Streams';

export default memo(StreamEditor);
