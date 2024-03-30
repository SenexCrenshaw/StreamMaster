import { Suspense, lazy, memo } from 'react';

const StreamEditor = () => {
  const id = 'streameditor';
  const SMChannelDataSelector = lazy(() => import('./SMChannelDataSelector'));
  const SMStreamDataSelector = lazy(() => import('./SMStreamDataSelector'));

  return (
    // <StandardHeader className="streamEditor" displayName="STREAMS" icon={<PlayListEditorIcon />}>
    <Suspense fallback={<div>Loading...</div>}>
      <div className="flex justify-content-between align-items-center">
        <div className="col-7 m-0 p-0">
          <SMChannelDataSelector id={id} />
        </div>
        {/* <div className="col-5 m-0 p-0 border-left-1 border-50">{<SMStreamDataSelector id={id} />}</div> */}
      </div>
    </Suspense>
    // </StandardHeader>
  );
};

StreamEditor.displayName = 'Streams';

export default memo(StreamEditor);
