import React from "react";

import { Toast } from 'primereact/toast';
import IconSelector from "./IconSelector";
import { type VideoStreamDto } from "../store/iptvApi";
import { type UpdateVideoStreamRequest } from "../store/iptvApi";
import { useChannelGroupsUpdateChannelGroupMutation } from "../store/iptvApi";


const ChannelLogoEditor = (props: StreamDataSelectorProps) => {
  const toast = React.useRef<Toast>(null);
  const [channelGroupsUpdateChannelGroupMutation] = useChannelGroupsUpdateChannelGroupMutation();

  const onUpdateVideoStream = React.useCallback(async (Logo: string) => {
    if (props.data.id === '') {
      return;
    }


    const data = {} as UpdateVideoStreamRequest;

    data.id = props.data.id;

    if (Logo && Logo !== '' && props.data.user_Tvg_logo !== Logo) {
      data.tvg_logo = Logo;
    }


    await channelGroupsUpdateChannelGroupMutation(data)
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

  }, [channelGroupsUpdateChannelGroupMutation, props.data.id, props.data.user_Tvg_logo]);

  return (
    <>
      <Toast position="bottom-right" ref={toast} />
      <IconSelector
        className="p-inputtext-sm"
        enableEditMode={props.enableEditMode}
        onChange={
          async (e: string) => {
            await onUpdateVideoStream(e);
          }
        }
        value={props.data.user_Tvg_logo}
      />
    </>
  );
};

ChannelLogoEditor.displayName = 'Logo Editor';
ChannelLogoEditor.defaultProps = {
  enableEditMode: true
};

export type StreamDataSelectorProps = {
  data: VideoStreamDto;
  enableEditMode?: boolean;
};

export default React.memo(ChannelLogoEditor);
