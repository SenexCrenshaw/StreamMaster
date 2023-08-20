import { type CSSProperties } from "react";
import React from "react";
import NumberEditorBodyTemplate from "./NumberEditorBodyTemplate";

import { getTopToolOptions } from "../common/common";
import { isDebug } from "../settings";
import { type UpdateVideoStreamRequest } from "../store/iptvApi";
import { type VideoStreamDto } from "../store/iptvApi";
import { useVideoStreamsUpdateVideoStreamMutation } from "../store/iptvApi";

const ChannelNumberEditor = (props: ChannelNumberEditorProps) => {
  const [videoStreamsUpdateVideoStreamMutation] = useVideoStreamsUpdateVideoStreamMutation();

  const onUpdateVideoStream = React.useCallback(async (channelNumber: number,) => {
    if (props.data.id === '' || props.data.user_Tvg_chno === channelNumber) {
      return;
    }

    const data = {} as UpdateVideoStreamRequest;
    data.id = props.data.id;
    data.tvg_chno = channelNumber;

    await videoStreamsUpdateVideoStreamMutation(data)
      .then(() => {

      }).catch((e) => {
        console.log(e);
      });

  }, [props.data.id, props.data.user_Tvg_chno, videoStreamsUpdateVideoStreamMutation]);


  return (
    <NumberEditorBodyTemplate
      onChange={async (e) => {
        await onUpdateVideoStream(e);
      }}
      resetValue={props.data.tvg_chno}
      style={props.style}
      tooltip={isDebug ? 'id: ' + props.data.id : undefined}
      tooltipOptions={getTopToolOptions}
      value={props.data.user_Tvg_chno}
    />
  )
}

ChannelNumberEditor.displayName = 'Channel Number Editor';
ChannelNumberEditor.defaultProps = {
};

export type ChannelNumberEditorProps = {
  data: VideoStreamDto;
  style?: CSSProperties;
};

export default React.memo(ChannelNumberEditor);


