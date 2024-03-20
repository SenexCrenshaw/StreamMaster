import { memo } from 'react';
import SMChannelDataSelector from './SMChannelDataSelector';
import SMStreamDataSelector from './SMStreamDataSelector';

const StreamEditor = () => {
  const id = 'streameditor';

  return (
    // <StandardHeader className="streamEditor" displayName="STREAMS" icon={<PlayListEditorIcon />}>
    <div className="flex justify-content-between align-items-center">
      <div className="col-7 m-0 p-0">
        <SMChannelDataSelector id={id} />
      </div>
      <div className="col-5 m-0 p-0">{<SMStreamDataSelector id={id} />}</div>
    </div>
    // </StandardHeader>
  );
};

StreamEditor.displayName = 'Streams';

export default memo(StreamEditor);
