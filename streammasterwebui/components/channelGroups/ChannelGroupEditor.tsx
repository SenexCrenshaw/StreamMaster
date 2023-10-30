import React from 'react';

import { useVideoStreamsUpdateVideoStreamMutation, type UpdateVideoStreamRequest, type VideoStreamDto } from '@lib/iptvApi';
import ChannelGroupSelector from './ChannelGroupSelector';

const ChannelGroupEditor = (props: ChannelGroupEditorProperties) => {
  const [videoStreamsUpdateVideoStreamMutation] = useVideoStreamsUpdateVideoStreamMutation();
  const onUpdateStream = React.useCallback(
    async (groupName: string) => {
      if (
        props.data === undefined
        || props.data.id === undefined
        || props.data.id === ''
        || !groupName
        || groupName === ''
        || props.data.user_Tvg_group === groupName
      ) {
        return;
      }

      const data = {} as UpdateVideoStreamRequest;

      data.id = props.data.id;
      data.tvg_group = groupName;

      await videoStreamsUpdateVideoStreamMutation(data)
        .then(() => {})
        .catch((error) => {
          console.error(error);
        });
    },
    [props.data, videoStreamsUpdateVideoStreamMutation]
  );

  return (
    <div className="flex w-full">
      <ChannelGroupSelector onChange={onUpdateStream} resetValue={props.data.tvg_group} value={props.data.user_Tvg_group} />
    </div>
  );
};

ChannelGroupEditor.displayName = 'Channel Group Dropdown';

export interface ChannelGroupEditorProperties {
  readonly data: VideoStreamDto;
}

export default React.memo(ChannelGroupEditor);
