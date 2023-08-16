import React from "react";

import { Button } from "primereact/button";
import { Toast } from 'primereact/toast';
import { getTopToolOptions } from "../common/common";
import { type UpdateVideoStreamRequest, type VideoStreamDto } from "../store/iptvApi";
import { useVideoStreamsUpdateVideoStreamMutation, useProgrammesGetProgrammeNamesQuery } from "../store/iptvApi";


const VideoStreamSetEPGFromNameDialog = (props: VideoStreamSetEPGFromNameDialogProps) => {
  const toast = React.useRef<Toast>(null);
  const [videoStreamsUpdateVideoStreamMutation] = useVideoStreamsUpdateVideoStreamMutation();

  const programmeNamesQuery = useProgrammesGetProgrammeNamesQuery();

  const [canSet, setCanSet] = React.useState<string>('');

  const ReturnToParent = React.useCallback(() => {
    setCanSet('');
    props.onClose?.();
  }, [props]);

  React.useEffect(() => {

    if (props.value === null || props.value === undefined || props.value.user_Tvg_ID === undefined || props.value.user_Tvg_ID === ''
      || (programmeNamesQuery === null || programmeNamesQuery === undefined || programmeNamesQuery.data === null || programmeNamesQuery.data === undefined)
    ) {
      return;
    }

    if (programmeNamesQuery.data.find((x) => x.displayName === props.value?.user_Tvg_name)) {
      setCanSet(props.value?.user_Tvg_name);
      return;
    }

    if (programmeNamesQuery.data.find((x) => x.channelName === props.value?.user_Tvg_name)) {
      setCanSet(props.value?.user_Tvg_name);
      return;
    }

  }, [programmeNamesQuery, props.value]);

  const onChangeEPG = React.useCallback(async () => {

    if (props.value === undefined || props.value.id === undefined || canSet === '') {
      ReturnToParent();
      return;
    }

    const toSend = {} as UpdateVideoStreamRequest;
    toSend.id = props.value?.id;
    toSend.tvg_ID = canSet;

    await videoStreamsUpdateVideoStreamMutation(toSend)
      .then(() => {
        if (toast.current) {

          toast.current.show({
            detail: `Updated Stream`,
            life: 3000,
            severity: 'success',
            summary: 'Successful',
          });

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

  }, [ReturnToParent, canSet, props.value, videoStreamsUpdateVideoStreamMutation]);

  return (
    <>
      <Toast position="bottom-right" ref={toast} />
      <Button
        disabled={canSet === '' || props.value?.user_Tvg_ID === canSet}
        icon='pi pi-book'
        onClick={async () =>
          await onChangeEPG()
        }
        rounded
        size="small"
        text={props.iconFilled !== true}
        tooltip="Match EPG to Name"
        tooltipOptions={getTopToolOptions}
      />
    </>
  );
}

VideoStreamSetEPGFromNameDialog.displayName = 'VideoStreamSetEPGFromNameDialog';
VideoStreamSetEPGFromNameDialog.defaultProps = {
}

type VideoStreamSetEPGFromNameDialogProps = {
  iconFilled?: boolean | undefined;
  onClose?: (() => void);
  value?: VideoStreamDto | undefined;
};

export default React.memo(VideoStreamSetEPGFromNameDialog);
