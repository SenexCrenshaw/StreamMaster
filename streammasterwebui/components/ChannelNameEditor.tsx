import { UpdateVideoStreamRequest, VideoStreamDto } from '@lib/iptvApi';
import { UpdateVideoStream } from '@lib/smAPI/VideoStreams/VideoStreamsMutateAPI';
import React from 'react';
import StringEditorBodyTemplate from './inputs/StringEditorBodyTemplate';

const ChannelNameEditor = (props: ChannelNameEditorProperties) => {
  const onUpdateM3UStream = React.useCallback(
    async (name: string) => {
      if (props.data.id === '' || !name || name === '' || props.data.user_Tvg_name === name) {
        return;
      }

      const data = {} as UpdateVideoStreamRequest;

      data.id = props.data.id;
      data.tvg_name = name;

      await UpdateVideoStream(data)
        .then(() => {})
        .catch((error) => {
          console.error(error);
        });
    },
    [props.data.id, props.data.user_Tvg_name]
  );

  if (props.data.user_Tvg_name === undefined) {
    return <span className="sm-inputtext" />;
  }

  return (
    <StringEditorBodyTemplate
      onChange={async (e) => {
        await onUpdateM3UStream(e);
      }}
      resetValue={props.data.isUserCreated ? undefined : props.data.tvg_name}
      value={props.data.user_Tvg_name}
    />
  );
};

ChannelNameEditor.displayName = 'Channel Number Editor';

export interface ChannelNameEditorProperties {
  readonly data: VideoStreamDto;
}

export default React.memo(ChannelNameEditor);
