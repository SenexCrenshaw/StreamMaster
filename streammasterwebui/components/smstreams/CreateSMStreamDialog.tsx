import { SMDialogRef } from '@components/sm/SMDialog';
import SMPopUp from '@components/sm/SMPopUp';
import { SMChannelDialogRef } from '@components/smchannels/SMChannelDialog';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';

import { CreateSMStream } from '@lib/smAPI/SMStreams/SMStreamsCommands';
import { SMStreamDto } from '@lib/smAPI/smapiTypes';
import React, { useRef, useState } from 'react';
import SMStreamDialog from './SMStreamDialog';

interface CreateSMStreamDialogProperties {
  readonly label?: string;
}

const CreateSMStreamDialog = ({ label }: CreateSMStreamDialogProperties) => {
  const dataKey = 'SMChannelSMStreamDialog-SMStreamDataForSMChannelSelector';
  const { setSelectedItems } = useSelectedItems<SMStreamDto>(dataKey);
  const [saveEnabled, setSaveEnabled] = useState<boolean>(false);
  const smChannelDialogRef = useRef<SMChannelDialogRef>(null);
  const smDialogRef = useRef<SMDialogRef>(null);

  const ReturnToParent = React.useCallback(() => {
    setSelectedItems([]);
  }, [setSelectedItems]);

  const onSave = React.useCallback((request: any) => {
    CreateSMStream(request)
      .then(() => {})
      .catch((e: any) => {
        console.error(e);
      })
      .finally(() => {
        smDialogRef.current?.hide();
      });
  }, []);

  return (
    <SMPopUp
      buttonClassName="icon-green"
      contentWidthSize="5"
      menu
      icon="pi-plus"
      iconFilled
      buttonLabel={label}
      modal
      okButtonDisabled={!saveEnabled}
      onOkClick={() => smChannelDialogRef.current?.save()}
      placement="bottom-end"
      showRemember={false}
      title="Create Stream"
    >
      <SMStreamDialog ref={smChannelDialogRef} onSave={onSave} onSaveEnabled={setSaveEnabled} />
    </SMPopUp>
  );
};

CreateSMStreamDialog.displayName = 'CreateSMStreamDialog';

export default React.memo(CreateSMStreamDialog);
