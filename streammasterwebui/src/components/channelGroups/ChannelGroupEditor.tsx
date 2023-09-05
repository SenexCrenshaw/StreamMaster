import React from "react";

import ChannelGroupSelector from "./ChannelGroupSelector";
import { type UpdateVideoStreamRequest, type VideoStreamDto } from "../../store/iptvApi";
import { useVideoStreamsUpdateVideoStreamMutation } from "../../store/iptvApi";

const ChannelGroupEditor = (props: ChannelGroupEditorProps) => {

  const [videoStreamsUpdateVideoStreamMutation] = useVideoStreamsUpdateVideoStreamMutation();
  const onUpdateStream = React.useCallback(async (groupName: string,) => {
    if (props.data === undefined || props.data.id === undefined || props.data.id === '' || !groupName || groupName === '' || props.data.user_Tvg_group === groupName) {
      return;
    }

    const data = {} as UpdateVideoStreamRequest;

    data.id = props.data.id;
    data.tvg_group = groupName;

    await videoStreamsUpdateVideoStreamMutation(data)
      .then(() => {

      }).catch((e) => {
        console.error(e);
      });

  }, [props.data, videoStreamsUpdateVideoStreamMutation]);

  return (
    <div className="iconSelector flex w-full justify-content-center align-items-center">
      <ChannelGroupSelector
        onChange={onUpdateStream}
        resetValue={props.data.tvg_group}
        value={props.data.user_Tvg_group}
      />
    </div>
  )
};

ChannelGroupEditor.displayName = 'Channel Group Dropdown';
ChannelGroupEditor.defaultProps = {

};

export type ChannelGroupEditorProps = {
  data: VideoStreamDto;

};

export default React.memo(ChannelGroupEditor);
