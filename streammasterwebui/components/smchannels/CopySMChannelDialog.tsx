import OKButton from '@components/buttons/OKButton';
import StringEditor from '@components/inputs/StringEditor';
import SMDialog, { SMDialogRef } from '@components/sm/SMDialog';

import { CopySMChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import useGetSMChannelNames from '@lib/smAPI/SMChannels/useGetSMChannelNames';
import { CopySMChannelRequest, SMChannelDto } from '@lib/smAPI/smapiTypes';
import React, { useCallback, useEffect, useRef } from 'react';

interface CopySMChannelProperties {
  label: string;
  readonly onHide?: () => void;
  smChannel: SMChannelDto;
}

const CopySMChannelDialog = ({ label, onHide, smChannel }: CopySMChannelProperties) => {
  const smDialogRef = useRef<SMDialogRef>(null);
  const smQuery = useGetSMChannelNames();

  const [newName, setNewName] = React.useState<string>('');

  const getUniqueName = useCallback(
    (name: string): string => {
      if (!smQuery.data) {
        return name;
      }

      let uniqueName = name;
      let counter = 1;

      while (smQuery.data.some((x) => x === uniqueName)) {
        uniqueName = `${name}.${counter}`;
        counter++;
      }

      return uniqueName;
    },
    [smQuery.data]
  );

  useEffect(() => {
    if (smChannel) {
      const t = getUniqueName(smChannel.Name);
      setNewName(t);
    }
  }, [getUniqueName, smChannel]);

  const ReturnToParent = React.useCallback(() => {
    onHide?.();
  }, [onHide]);

  const onSave = React.useCallback(async () => {
    if (!smChannel) {
      return;
    }

    const request = {} as CopySMChannelRequest;
    request.SMChannelId = smChannel.Id;

    const t = getUniqueName(newName);
    request.NewName = t;

    CopySMChannel(request)
      .then(() => {})
      .catch((error) => {
        console.error(error);
      })
      .finally(() => {
        smDialogRef.current?.close();
      });
  }, [getUniqueName, newName, smChannel]);

  return (
    <SMDialog
      ref={smDialogRef}
      iconFilled={false}
      title="COPY CHANNEL"
      onHide={() => ReturnToParent()}
      buttonClassName="icon-orange"
      icon="pi-clone"
      widthSize={2}
      info="General"
    >
      <div className="w-12">
        <div className="surface-border flex grid flex-wrap justify-content-center p-0 m-0">
          <div className="flex col-12 pl-1 justify-content-start align-items-center p-0 m-0 w-full">
            <StringEditor label="New Name" darkBackGround disableDebounce onChange={(e) => e && setNewName(e)} onSave={(e) => {}} value={newName} />
          </div>
          <div className="flex col-12 gap-1 mt-4 justify-content-center ">
            <OKButton onClick={async () => await onSave()} />
          </div>
        </div>
        <div className="layout-padding-bottom-lg" />
      </div>
    </SMDialog>
  );
};

CopySMChannelDialog.displayName = 'COPYSMCHANNELDIALOG';

export default React.memo(CopySMChannelDialog);
