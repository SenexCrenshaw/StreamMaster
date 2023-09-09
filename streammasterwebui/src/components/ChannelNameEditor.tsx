import React from "react";
import { useVideoStreamsUpdateVideoStreamMutation, type UpdateVideoStreamRequest, type VideoStreamDto } from "../store/iptvApi";
import StringEditorBodyTemplate from "./StringEditorBodyTemplate";

const ChannelNameEditor = (props: ChannelNameEditorProps) => {
  const [videoStreamsUpdateVideoStreamMutation, { isLoading }] = useVideoStreamsUpdateVideoStreamMutation();

  const onUpdateM3UStream = React.useCallback(async (name: string,) => {
    if (props.data.id === '' || !name || name === '' || props.data.user_Tvg_name === name) {
      return;
    }

    const data = {} as UpdateVideoStreamRequest;

    data.id = props.data.id;
    data.tvg_name = name;

    await videoStreamsUpdateVideoStreamMutation(data)
      .then(() => {
      }).catch((error) => {
        console.error(error);
      });

  }, [props.data.id, props.data.user_Tvg_name, videoStreamsUpdateVideoStreamMutation]);

  if (props.data.user_Tvg_name === undefined) {
    return <span className='sm-inputtext' />
  }

  return (
    <StringEditorBodyTemplate
      isLoading={isLoading}
      onChange={async (e) => {
        await onUpdateM3UStream(e);
      }}
      resetValue={props.data.isUserCreated ? undefined : props.data.tvg_name}
      value={props.data.user_Tvg_name}
    />
  )
}

ChannelNameEditor.displayName = 'Channel Number Editor';
ChannelNameEditor.defaultProps = {

};

export type ChannelNameEditorProps = {
  readonly data: VideoStreamDto;
};

export default React.memo(ChannelNameEditor);
