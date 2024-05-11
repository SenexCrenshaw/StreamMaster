import SMDialog, { SMDialogRef } from '@components/sm/SMDialog';

import { UpdateSMChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import useGetSMChannel from '@lib/smAPI/SMChannels/useGetSMChannel';
import { CreateSMChannelRequest, GetSMChannelRequest, SMChannelDto, UpdateSMChannelRequest } from '@lib/smAPI/smapiTypes';
import React, { useRef } from 'react';
import SMChannelDialog from './SMChannelDialog';

interface CopySMChannelProperties {
  readonly onHide?: () => void;
  smChannel: SMChannelDto;
}

const EditSMChannelDialog = ({ onHide, smChannel: toGet }: CopySMChannelProperties) => {
  const smDialogRef = useRef<SMDialogRef>(null);
  const query = useGetSMChannel({ SMChannelId: toGet.Id } as GetSMChannelRequest);

  const ReturnToParent = React.useCallback(() => {
    onHide?.();
  }, [onHide]);

  const onSave = React.useCallback(
    (request: CreateSMChannelRequest) => {
      const requestToSend = { ...request } as UpdateSMChannelRequest;
      requestToSend.Id = toGet.Id;

      UpdateSMChannel(requestToSend)
        .then(() => {})
        .catch((e) => {
          console.error(e);
        })
        .finally(() => {
          smDialogRef.current?.close();
        });
    },
    [toGet.Id]
  );

  if (!query.data) return null;

  return (
    <SMDialog
      darkBackGround
      ref={smDialogRef}
      iconFilled={false}
      title={`EDIT CHANNEL : ${toGet.Name}`}
      onHide={() => ReturnToParent()}
      buttonClassName="icon-yellow"
      icon="pi-pencil"
      widthSize={5}
      info="General"
      tooltip="Edit Channel"
    >
      <SMChannelDialog smChannel={query.data} onSave={onSave} />
    </SMDialog>
  );
};

EditSMChannelDialog.displayName = 'EditSMChannelDialog';

export default React.memo(EditSMChannelDialog);
