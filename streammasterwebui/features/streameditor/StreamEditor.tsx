import StandardHeader from '@components/StandardHeader';
import { PlayListEditorIcon } from '@lib/common/icons';
import { memo } from 'react';
import SMChannelDataSelector from './SMChannelDataSelector';
import SMStreamDataSelector from './SMStreamDataSelector';

const StreamEditor = () => {
  const id = 'streameditor';

  return (
    <StandardHeader displayName="PLAYLIST" icon={<PlayListEditorIcon />}>
      <div className="w-6">
        <SMChannelDataSelector id={id} />
      </div>
      <div className="w-6 layout-padding-left">
        <SMStreamDataSelector id={id} />
      </div>
    </StandardHeader>
  );
};

StreamEditor.displayName = 'Streams';

export default memo(StreamEditor);
