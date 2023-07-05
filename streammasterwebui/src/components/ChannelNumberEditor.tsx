import { type CSSProperties } from "react";
import React from "react";
import NumberEditorBodyTemplate from "./NumberEditorBodyTemplate";
import type * as StreamMasterApi from '../store/iptvApi';
import * as Hub from "../store/signlar_functions";
import { Toast } from 'primereact/toast';
import { getTopToolOptions } from "../common/common";
import { isDebug } from "../settings";

const ChannelNumberEditor = (props: ChannelNumberEditorProps) => {
  const toast = React.useRef<Toast>(null);

  const onUpdateVideoStream = React.useCallback(async (channelNumber: number,) => {
    if (props.data.id < 0 || props.data.user_Tvg_chno === channelNumber) {
      return;
    }

    const data = {} as StreamMasterApi.UpdateVideoStreamRequest;
    data.id = props.data.id;
    data.tvg_chno = channelNumber;

    await Hub.UpdateVideoStream(data)
      .then((result) => {
        if (toast.current) {
          if (result) {
            toast.current.show({
              detail: `Updated Stream`,
              life: 3000,
              severity: 'success',
              summary: 'Successful',
            });
          } else {
            toast.current.show({
              detail: `Update Stream Failed`,
              life: 3000,
              severity: 'error',
              summary: 'Error',
            });
          }

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

  }, [props.data.id, props.data.user_Tvg_chno]);

  if (!props.enableEditMode) {
    return <span className='smallshiftleft'>{props.data.user_Tvg_chno}</span>
  }

  return (
    <>
      <Toast position="bottom-right" ref={toast} />
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
    </>
  )
}

ChannelNumberEditor.displayName = 'Channel Number Editor';
ChannelNumberEditor.defaultProps = {
};

export type ChannelNumberEditorProps = {
  data: StreamMasterApi.VideoStreamDto;
  enableEditMode: boolean;
  style?: CSSProperties;
};

export default React.memo(ChannelNumberEditor);


