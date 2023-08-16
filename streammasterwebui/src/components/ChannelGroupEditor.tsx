import React from "react";
import type * as StreamMasterApi from '../store/iptvApi';
import { Toast } from 'primereact/toast';
import * as Hub from "../store/signlar_functions";
import ChannelGroupSelector from "./ChannelGroupSelector";

const ChannelGroupEditor = (props: ChannelGroupEditorProps) => {
  const toast = React.useRef<Toast>(null);

  const onUpdateStream = React.useCallback(async (groupName: string,) => {
    if (props.data === undefined || props.data.id === undefined || props.data.id === '' || !groupName || groupName === '' || props.data.user_Tvg_group === groupName) {
      return;
    }

    return;
    const data = {} as StreamMasterApi.UpdateVideoStreamRequest;
    data.id = props.data.id;
    data.tvg_group = groupName;

    await Hub.UpdateVideoStream(data)
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

  }, [props.data]);

  return (
    <>
      <Toast position="bottom-right" ref={toast} />

      <div className="iconSelector flex w-full justify-content-center align-items-center">
        <ChannelGroupSelector
          onChange={onUpdateStream}
          resetValue={props.data.tvg_group}
          value={props.data.user_Tvg_group}
        />
      </div>
    </ >
  )
};

ChannelGroupEditor.displayName = 'Channel Group Dropdown';
ChannelGroupEditor.defaultProps = {

};

export type ChannelGroupEditorProps = {
  data: StreamMasterApi.VideoStreamDto;

};

export default React.memo(ChannelGroupEditor);
