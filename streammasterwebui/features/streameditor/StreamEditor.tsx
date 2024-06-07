import StandardHeader from '@components/StandardHeader';
import { PlayListEditorIcon } from '@lib/common/icons';
import { useIsTrue } from '@lib/redux/hooks/isTrue';
import { memo } from 'react';
import SMChannelDataSelector from './SMChannelDataSelector';
import SMStreamDataSelector from './SMStreamDataSelector';

const StreamEditor = () => {
  const id = 'streameditor';
  const { isTrue: smTableIsSimple } = useIsTrue('streameditor-SMStreamDataSelector');

  if (smTableIsSimple) {
    return (
      <StandardHeader displayName="PLAYLIST" icon={<PlayListEditorIcon />}>
        <div className="w-10">
          <SMChannelDataSelector id={id} />
        </div>

        <div className="w-2 layout-padding-left">
          <SMStreamDataSelector id={id} />
        </div>
      </StandardHeader>
    );
  }

  return (
    <StandardHeader displayName="PLAYLIST" icon={<PlayListEditorIcon />}>
      <div className="sm-w-6">
        <SMChannelDataSelector id={id} />
      </div>

      <div className="sm-w-6 layout-padding-left">
        <SMStreamDataSelector id={id} />
      </div>
    </StandardHeader>
  );
};

StreamEditor.displayName = 'Streams';

export default memo(StreamEditor);
