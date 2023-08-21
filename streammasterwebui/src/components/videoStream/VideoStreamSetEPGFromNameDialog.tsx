
import { useState, useCallback, memo } from "react";
import { getTopToolOptions } from "../../common/common";
import { type SetVideoStreamSetEpGsFromNameRequest, type VideoStreamDto } from "../../store/iptvApi";
import { useVideoStreamsSetVideoStreamSetEpGsFromNameMutation } from "../../store/iptvApi";
import { Button } from "primereact/button";


const VideoStreamSetEPGFromNameDialog = (props: VideoStreamSetEPGFromNameDialogProps) => {

  const [vdeoStreamsSetVideoStreamSetEpGsFromNameMutation] = useVideoStreamsSetVideoStreamSetEpGsFromNameMutation();


  const [canSet, setCanSet] = useState<string>('');

  const ReturnToParent = useCallback(() => {
    setCanSet('');
    props.onClose?.();
  }, [props]);



  const onChangeEPG = useCallback(async () => {

    if (props.value === undefined || props.value.id === undefined || canSet === '') {
      ReturnToParent();
      return;
    }

    const toSend = {} as SetVideoStreamSetEpGsFromNameRequest;
    toSend.ids = [props.value?.id];


    await vdeoStreamsSetVideoStreamSetEpGsFromNameMutation(toSend)
      .then(() => {

      }
      ).catch((error) => {
        console.error(error);
      });
    ReturnToParent();

  }, [ReturnToParent, canSet, props.value, vdeoStreamsSetVideoStreamSetEpGsFromNameMutation]);

  return (
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

export default memo(VideoStreamSetEPGFromNameDialog);
