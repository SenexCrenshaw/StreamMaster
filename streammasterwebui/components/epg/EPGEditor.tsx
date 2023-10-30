import { type UpdateVideoStreamRequest, type VideoStreamDto } from '@lib/iptvApi';
import { UpdateVideoStream } from '@lib/smAPI/VideoStreams/VideoStreamsMutateAPI';
import { memo } from 'react';
import EPGSelector from '../selectors/EPGSelector';

interface EPGEditorProperties {
  readonly data: VideoStreamDto;
  readonly enableEditMode?: boolean;
}

const EPGEditor = ({ data, enableEditMode }: EPGEditorProperties) => {
  const onUpdateVideoStream = async (epg: string) => {
    if (data.id === '') {
      return;
    }

    const toSend = {} as UpdateVideoStreamRequest;

    toSend.id = data.id;

    if (epg && epg !== '' && data.user_Tvg_ID !== epg) {
      toSend.tvg_ID = epg;
    }

    await UpdateVideoStream(toSend)
      .then(() => {})
      .catch((error: unknown) => {
        console.error(error);
      });
  };

  return (
    <div className="flex w-full">
      <EPGSelector
        enableEditMode={enableEditMode}
        onChange={async (e: string) => {
          await onUpdateVideoStream(e);
        }}
        value={data.user_Tvg_ID}
      />
    </div>
  );
};

export default memo(EPGEditor);
