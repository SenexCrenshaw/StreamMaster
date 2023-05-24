/* eslint-disable @typescript-eslint/no-unused-vars */
import React from "react";
import type * as StreamMasterApi from '../store/iptvApi';
import { Toast } from 'primereact/toast';
import * as Hub from "../store/signlar_functions";
import IconSelector from "./IconSelector";


const ChannelLogoEditor = (props: StreamDataSelectorProps) => {
  const toast = React.useRef<Toast>(null);

  const onUpdateVideoStream = React.useCallback(async (Logo: string) => {
    if (props.data.id < 0) {
      return;
    }


    const data = {} as StreamMasterApi.UpdateVideoStreamRequest;

    data.id = props.data.id;

    if (Logo && Logo !== '' && props.data.user_Tvg_logo !== Logo) {
      data.tvg_logo = Logo;
    }


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

  }, [props.data.id, props.data.user_Tvg_logo]);

  return (
    <>
      <Toast position="bottom-right" ref={toast} />
      <IconSelector
        className="p-inputtext-sm"
        onChange={
          async (e: StreamMasterApi.IconFileDto) => {

            // const newiconSource = e.originalSource.includes('://')
            //   ? e.originalSource
            //   : e.name;

            await onUpdateVideoStream(e.originalSource);
          }
        }
        onReset={
          async (e: StreamMasterApi.IconFileDto) => {

            // const newiconSource = e.originalSource.includes('://')
            //   ? e.originalSource
            //   : e.name;

            await onUpdateVideoStream(e.originalSource);
          }
        }
        resetValue={props.data.tvg_logo}
        value={props.data.user_Tvg_logo}
      />
    </>
  );
};

ChannelLogoEditor.displayName = 'Logo Editor';
ChannelLogoEditor.defaultProps = {
};

export type StreamDataSelectorProps = {
  data: StreamMasterApi.VideoStreamDto;
};

export default React.memo(ChannelLogoEditor);
