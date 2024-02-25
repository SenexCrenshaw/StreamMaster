import { CSSProperties, memo, useCallback } from 'react';

import NumberEditorBodyTemplate from '@components/NumberEditorBodyTemplate';
import { getTopToolOptions } from '@lib/common/common';
import { UpdateVideoStreamRequest, VideoStreamDto } from '@lib/iptvApi';
import { isDev } from '@lib/settings';
import { UpdateVideoStream } from '@lib/smAPI/VideoStreams/VideoStreamsMutateAPI';

interface ProfileTimeOutNumberEditorProperties {
  readonly data: VideoStreamDto;
  readonly style?: CSSProperties;
}

const ProfileTimeOutNumberEditor = ({ data, style }: ProfileTimeOutNumberEditorProperties) => {
  const onUpdateVideoStream = useCallback(
    async (profileTimeOutNumber: number) => {
      if (data.id === '' || data.user_Tvg_chno === profileTimeOutNumber) {
        return;
      }

      const toSend = {} as UpdateVideoStreamRequest;

      toSend.id = data.id;
      toSend.tvg_chno = profileTimeOutNumber;

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

ProfileTimeOutNumberEditor.displayName = 'Channel Number Editor';

export default memo(ProfileTimeOutNumberEditor);
