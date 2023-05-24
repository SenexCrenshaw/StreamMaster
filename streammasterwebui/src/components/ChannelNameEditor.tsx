import React from "react";

import type * as StreamMasterApi from '../store/iptvApi';
import * as Hub from "../store/signlar_functions";
import { Toast } from 'primereact/toast';
import StringEditorBodyTemplate from "./StringEditorBodyTemplate";

const ChannelNameEditor = (props: ChannelNameEditorProps) => {
  const toast = React.useRef<Toast>(null);

  const onUpdateM3UStream = React.useCallback(async (name: string,) => {
    if (props.data.id < 0 || !name || name === '' || props.data.user_Tvg_name === name) {
      return;
    }

    const data = {} as StreamMasterApi.UpdateVideoStreamRequest;
    data.id = props.data.id;
    data.tvg_name = name;

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

  }, [props.data.id, props.data.user_Tvg_name]);

  if (props.data.user_Tvg_name == undefined) {
    return <span className='sm-inputtext' />
  }

  if (!props.enableEditMode) {
    return <div
      className='sm-inputtext smallshiftleft'
      style={{
        ...{
          backgroundColor: 'var(--mask-bg)',
          overflow: 'hidden',
          textOverflow: 'ellipsis',
          whiteSpace: 'nowrap',
          width: `${props.width}`
        },
      }}
    >{props.data.user_Tvg_name}</div>
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
  width: '22rem',
};

export type ChannelNameEditorProps = {
  data: StreamMasterApi.VideoStreamDto;
  enableEditMode: boolean;
  width?: string;
};

export default React.memo(ChannelNameEditor);
