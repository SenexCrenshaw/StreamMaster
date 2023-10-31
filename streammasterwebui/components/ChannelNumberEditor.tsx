import { CSSProperties, memo, useCallback } from 'react';

import { getTopToolOptions } from '@lib/common/common';
import { UpdateVideoStreamRequest, VideoStreamDto } from '@lib/iptvApi';
import { isDev } from '@lib/settings';
import { UpdateVideoStream } from '@lib/smAPI/VideoStreams/VideoStreamsMutateAPI';
import NumberEditorBodyTemplate from './NumberEditorBodyTemplate';

interface ChannelNumberEditorProperties {
  readonly data: VideoStreamDto;
  readonly style?: CSSProperties;
}

const ChannelNumberEditor = ({ data, style }: ChannelNumberEditorProperties) => {
  const onUpdateVideoStream = useCallback(
    async (channelNumber: number) => {
      if (data.id === '' || data.user_Tvg_chno === channelNumber) {
        return;
      }

      const toSend = {} as UpdateVideoStreamRequest;

      toSend.id = data.id;
      toSend.tvg_chno = channelNumber;

      await UpdateVideoStream(toSend)
        .then(() => {})
        .catch((error) => {
          console.log(error);
        });
    },
    [data.id, data.user_Tvg_chno]
  );

  return (
    <NumberEditorBodyTemplate
      onChange={async (e) => {
        await onUpdateVideoStream(e);
      }}
      resetValue={data.tvg_chno}
      style={style}
      tooltip={isDev ? `id: ${data.id}` : undefined}
      tooltipOptions={getTopToolOptions}
      value={data.user_Tvg_chno}
    />
  );
};

ChannelNumberEditor.displayName = 'Channel Number Editor';

export default memo(ChannelNumberEditor);
