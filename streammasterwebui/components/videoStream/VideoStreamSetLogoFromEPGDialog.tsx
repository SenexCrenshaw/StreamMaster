import { getTopToolOptions } from '@lib/common/common';

import { Button } from 'primereact/button';
import { memo, useCallback } from 'react';

const VideoStreamSetLogoFromEPGDialog = (props: VideoStreamSetLogoFromEPGDialogProperties) => {
  const ReturnToParent = useCallback(() => {
    props.onClose?.();
  }, [props]);

  const onChangeLogo = useCallback(async () => {
    if (props.value === undefined || props.value.id === undefined) {
      ReturnToParent();

      return;
    }

    const toSend = {} as VideoStreamsSetVideoStreamsLogoFromEpgApiArg;

    toSend.ids = [props.value.id];

    await SetVideoStreamsLogoFromEpg(toSend)
      .then(() => {})
      .catch((error) => {
        console.log(error);
      });
  }, [ReturnToParent, props.value]);

  return (
    <Button
      disabled={props.value === undefined || props.value.id === undefined}
      icon="pi pi-image"
      onClick={async () => await onChangeLogo()}
      rounded
      size="small"
      text={props.iconFilled !== true}
      tooltip="Set Logo From EPG"
      tooltipOptions={getTopToolOptions}
    />
  );
};

VideoStreamSetLogoFromEPGDialog.displayName = 'VideoStreamSetLogoFromEPGDialog';

interface VideoStreamSetLogoFromEPGDialogProperties {
  readonly iconFilled?: boolean | undefined;
  readonly onClose?: () => void;
  readonly value?: VideoStreamDto | undefined;
}

export default memo(VideoStreamSetLogoFromEPGDialog);
