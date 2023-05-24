import React from "react";
import type * as StreamMasterApi from '../store/iptvApi';
import { Button } from "primereact/button";
import { getTopToolOptions } from "../common/common";
import { ResetLogoIcon } from "../common/icons";
import { Toast } from 'primereact/toast';
import * as Hub from "../store/signlar_functions";
import ChannelGroupSelector from "./ChannelGroupSelector";
import ChannelGroupAddDialog from "./ChannelGroupAddDialog";

const ChannelGroupEditor = (props: ChannelGroupEditorProps) => {
  const toast = React.useRef<Toast>(null);

  const [channelGroup, setChannelGroup] = React.useState<string>('');


  React.useMemo(() => {

    if (props.data.user_Tvg_group !== undefined) {
      setChannelGroup(props.data.user_Tvg_group);
    }

  }, [props.data.user_Tvg_group]);

  const onUpdateStream = React.useCallback(async (groupName: string,) => {
    if (props.data === undefined || props.data.id === undefined || props.data.id < 0 || !groupName || groupName === '' || props.data.user_Tvg_group === groupName) {
      return;
    }

    const data = {} as StreamMasterApi.UpdateVideoStreamRequest;
    data.id = props.data.id;
    data.tvg_group = groupName;

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

  }, [props.data]);

  // eslint-disable-next-line @typescript-eslint/no-unused-vars, @typescript-eslint/no-explicit-any
  const filterTemplate = React.useCallback((filterOptions: any) => {
    return (
      <div className="flex gap-2 align-items-center">
        <ChannelGroupAddDialog />
        {filterOptions.element}
        {(props.data !== undefined && props.data.tvg_group !== channelGroup) &&
          < Button

            icon={<ResetLogoIcon sx={{ fontSize: 18 }} />}
            onClick={() => {
              if (props.data.tvg_group !== undefined) {
                setChannelGroup(props.data.tvg_group);
              }
            }
            }
            rounded
            severity="warning"
            size="small"
            tooltip="Reset Group"
            tooltipOptions={getTopToolOptions}
          />
        }
      </div>
    );
  }, [channelGroup, props.data]);

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
