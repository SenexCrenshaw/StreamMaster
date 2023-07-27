import React from "react";
import * as StreamMasterApi from '../store/iptvApi';
import * as Hub from '../store/signlar_functions';
import { Button } from "primereact/button";
import { Toast } from 'primereact/toast';
import { getTopToolOptions } from "../common/common";


const VideoStreamSetLogoFromEPGDialog = (props: VideoStreamSetLogoFromEPGDialogProps) => {
  const toast = React.useRef<Toast>(null);
  const channelLogos = StreamMasterApi.useVideoStreamsGetChannelLogoDtosQuery();
  const programmeNamesQuery = StreamMasterApi.useProgrammesGetProgrammeNamesQuery();

  const [epgLogoUrl, setEpgLogoUrl] = React.useState<string | undefined>(undefined);

  const ReturnToParent = React.useCallback(() => {
    setEpgLogoUrl(undefined);
    props.onClose?.();
  }, [props]);

  React.useEffect(() => {

    if (props.value === null || props.value === undefined || props.value.user_Tvg_ID === undefined || props.value.user_Tvg_ID === ''
      || (channelLogos === null || channelLogos === undefined || channelLogos.data === null || channelLogos.data === undefined)
      || (programmeNamesQuery === null || programmeNamesQuery === undefined || programmeNamesQuery.data === null || programmeNamesQuery.data === undefined)
      || channelLogos?.data?.length === 0) {
      return;
    }

    const epg = programmeNamesQuery.data.find((x) => x.displayName === props.value?.user_Tvg_ID);
    if (epg) {
      const found = channelLogos.data.find((x) => x.epgId === epg.channel);
      if (found?.logoUrl !== undefined && found.logoUrl !== '') {
        setEpgLogoUrl(found.logoUrl);
      }
    }

  }, [channelLogos, programmeNamesQuery, props.value]);

  const onChangeLogo = React.useCallback(async () => {

    if (props.value === undefined || props.value.id === undefined || epgLogoUrl === undefined || epgLogoUrl === undefined) {
      ReturnToParent();
      return;
    }

    const toSend = {} as StreamMasterApi.UpdateVideoStreamRequest;
    toSend.id = props.value?.id;
    toSend.tvg_logo = epgLogoUrl;

    await Hub.UpdateVideoStream(toSend)
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
      }
      ).catch((error) => {
        if (toast.current) {
          toast.current.show({
            detail: `Update Stream Failed`,
            life: 3000,
            severity: 'error',
            summary: 'Error ' + error.message,
          });
        }
      });

  }, [ReturnToParent, epgLogoUrl, props.value]);

  return (
    <>
      <Toast position="bottom-right" ref={toast} />
      <Button
        disabled={!epgLogoUrl || props.value?.user_Tvg_logo === epgLogoUrl}
        icon='pi pi-image'
        onClick={async () =>
          await onChangeLogo()
        }
        rounded
        size="small"
        text={props.iconFilled !== true}
        tooltip="Change Logo to EPG"
        tooltipOptions={getTopToolOptions}
      />
    </>
  );
}

VideoStreamSetLogoFromEPGDialog.displayName = 'VideoStreamSetLogoFromEPGDialog';
VideoStreamSetLogoFromEPGDialog.defaultProps = {
}

type VideoStreamSetLogoFromEPGDialogProps = {
  iconFilled?: boolean | undefined;
  onClose?: (() => void);
  value?: StreamMasterApi.VideoStreamDto | undefined;
};

export default React.memo(VideoStreamSetLogoFromEPGDialog);
