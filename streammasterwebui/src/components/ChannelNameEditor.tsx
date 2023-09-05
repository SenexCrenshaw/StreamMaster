import React from "react";

import { Toast } from 'primereact/toast';
import StringEditorBodyTemplate from "./StringEditorBodyTemplate";
import { type UpdateVideoStreamRequest, type VideoStreamDto } from "../store/iptvApi";
import { useVideoStreamsUpdateVideoStreamMutation } from "../store/iptvApi";

const ChannelNameEditor = (props: ChannelNameEditorProps) => {
  const toast = React.useRef<Toast>(null);
  const [videoStreamsUpdateVideoStreamMutation] = useVideoStreamsUpdateVideoStreamMutation();

  const onUpdateM3UStream = React.useCallback(async (name: string,) => {
    if (props.data.id === '' || !name || name === '' || props.data.user_Tvg_name === name) {
      return;
    }

    const data = {} as UpdateVideoStreamRequest;

    data.id = props.data.id;
    data.tvg_name = name;

    await videoStreamsUpdateVideoStreamMutation(data)
      .then(() => {
        if (toast.current) {

          toast.current.show({
            detail: `Updated Stream`,
            life: 3000,
            severity: 'success',
            summary: 'Successful',
          });

        }
      }).catch((e) => {
        if (toast.current) {
          toast.current.show({
            detail: `Update Stream Failed`,
            life: 3000,
            severity: 'error',
            summary: 'Error ' + e.message,
          });
        }
      });

  }, [props.data.id, props.data.user_Tvg_name, videoStreamsUpdateVideoStreamMutation]);

  if (props.data.user_Tvg_name === undefined) {
    return <span className='sm-inputtext' />
  }

  return (
    <div className="p-inputtext p-0">
      <Toast position="bottom-right" ref={toast} />

      <StringEditorBodyTemplate
        onChange={async (e) => {
          await onUpdateM3UStream(e);
        }}
        resetValue={props.data.tvg_name}
        value={props.data.user_Tvg_name}
      />

    </div>
  )
}

ChannelNameEditor.displayName = 'Channel Number Editor';
ChannelNameEditor.defaultProps = {

};

export type ChannelNameEditorProps = {
  data: VideoStreamDto;
};

export default React.memo(ChannelNameEditor);
