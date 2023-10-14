import { memo, useCallback, type CSSProperties } from 'react';

import { getTopToolOptions } from '@lib/common/common';
import { UpdateVideoStreamRequest, VideoStreamDto } from '@lib/iptvApi';
import { isDev } from '@lib/settings';
import { UpdateVideoStream } from '@lib/smAPI/VideoStreams/VideoStreamsMutateAPI';
import NumberEditorBodyTemplate from './NumberEditorBodyTemplate';

const ChannelNumberEditor = (props: ChannelNumberEditorProps) => {
  const onUpdateVideoStream = useCallback(
    async (channelNumber: number) => {
      if (props.data.id === '' || props.data.user_Tvg_chno === channelNumber) {
        return;
      }

      const data = {} as UpdateVideoStreamRequest;

      data.id = props.data.id;
      data.tvg_chno = channelNumber;

      await UpdateVideoStream(data)
        .then(() => {})
        .catch((e) => {
          console.log(e);
        });
    },
    [props.data.id, props.data.user_Tvg_chno],
  );

  return (
    <NumberEditorBodyTemplate
      onChange={async (e) => {
        await onUpdateVideoStream(e);
      }}
      resetValue={props.data.tvg_chno}
      style={props.style}
      tooltip={isDev ? 'id: ' + props.data.id : undefined}
      tooltipOptions={getTopToolOptions}
      value={props.data.user_Tvg_chno}
    />
  );
};

ChannelNumberEditor.displayName = 'Channel Number Editor';

export type ChannelNumberEditorProps = {
  readonly data: VideoStreamDto;
  readonly style?: CSSProperties;
};

export default memo(ChannelNumberEditor);
