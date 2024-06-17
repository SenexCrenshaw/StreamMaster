import StringEditor from '@components/inputs/StringEditor';
import SMPopUp from '@components/sm/SMPopUp';

import { CopySMChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import useGetSMChannelNames from '@lib/smAPI/SMChannels/useGetSMChannelNames';
import { CopySMChannelRequest, SMChannelDto } from '@lib/smAPI/smapiTypes';
import React, { useCallback, useEffect } from 'react';

interface CloneSMChannelDialogProperties {
  label: string;
  readonly onHide?: () => void;
  smChannel: SMChannelDto;
}

const CloneSMChannelDialog = ({ label, onHide, smChannel }: CloneSMChannelDialogProperties) => {
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
        ReturnToParent();
      });
  }, [ReturnToParent, getUniqueName, newName, smChannel]);

  return (
    <SMPopUp
      buttonClassName="icon-orange"
      disabled={newName === undefined || newName === ''}
      rememberKey={'CloneSMChannelDialog'}
      title="Clone"
      onOkClick={async () => await onSave()}
      icon="pi-clone"
    >
      <div className="flex col-12 justify-content-start align-items-center p-0 m-0 w-full">
        <StringEditor label="New Name" darkBackGround disableDebounce onChange={(e) => e !== undefined && setNewName(e)} onSave={(e) => {}} value={newName} />
      </div>
    </SMPopUp>
  );
};

CloneSMChannelDialog.displayName = 'COPYSMCHANNELDIALOG';

export default React.memo(CloneSMChannelDialog);
