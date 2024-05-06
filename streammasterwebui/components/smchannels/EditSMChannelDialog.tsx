import SMDialog, { SMDialogRef } from '@components/sm/SMDialog';

import { UpdateSMChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { CreateSMChannelRequest, SMChannelDto, UpdateSMChannelRequest } from '@lib/smAPI/smapiTypes';
import React, { useRef } from 'react';
import SMChannelDialog from './SMChannelDialog';

interface CopySMChannelProperties {
  readonly onHide?: () => void;
  smChannel: SMChannelDto;
}

const EditSMChannelDialog = ({ onHide, smChannel }: CopySMChannelProperties) => {
  const smDialogRef = useRef<SMDialogRef>(null);

  const ReturnToParent = React.useCallback(() => {
    onHide?.();
  }, [onHide]);

  const onSave = React.useCallback(
    (request: CreateSMChannelRequest) => {
      const requestToSend = { ...request } as UpdateSMChannelRequest;
      requestToSend.Id = smChannel.Id;

      UpdateSMChannel(requestToSend)
        .then(() => {})
        .catch((e) => {
          console.error(e);
        })
        .finally(() => {
          smDialogRef.current?.close();
        });
    },
    [smChannel.Id]
  );

  return (
    <SMDialog
      ref={smDialogRef}
      iconFilled={false}
      title={`EDIT CHANNEL : ${smChannel.Name}`}
      onHide={() => ReturnToParent()}
      buttonClassName="icon-yellow"
      icon="pi-pencil"
      widthSize={8}
      info="General"
      tooltip="Edit Channel"
    >
      <SMChannelDialog smChannel={smChannel} onSave={onSave} />;
    </SMDialog>
  );
};

EditSMChannelDialog.displayName = 'EditSMChannelDialog';

export default React.memo(EditSMChannelDialog);
