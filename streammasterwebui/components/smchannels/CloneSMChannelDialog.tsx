import StringEditor from '@components/inputs/StringEditor';
import SMPopUp from '@components/sm/SMPopUp';
import { useIsTrue } from '@lib/redux/hooks/isTrue';

import { CopySMChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { CopySMChannelRequest, SMChannelDto } from '@lib/smAPI/smapiTypes';
import React from 'react';

interface CloneSMChannelDialogProperties {
  label: string;
  readonly onHide?: () => void;
  smChannel: SMChannelDto;
}

const CloneSMChannelDialog = ({ label, onHide, smChannel }: CloneSMChannelDialogProperties) => {
  const { isTrue: smTableIsSimple } = useIsTrue('isSimple');
  const [newName, setNewName] = React.useState<string>('');

  // useEffect(() => {
  //   if (smChannel) {
  //     GetSMChannelUniqueName({ SMChannelName: smChannel.Name } as GetSMChannelUniqueNameRequest)
  //       .then((response) => {
  //         if (response) {
  //           setNewName(response);
  //         }
  //       })
  //       .catch((error) => {
  //         console.error(error);
  //       });
  //   }
  // }, [smChannel]);

  const ReturnToParent = React.useCallback(() => {
    onHide?.();
  }, [onHide]);

  const onSave = React.useCallback(async () => {
    if (!smChannel) {
      return;
    }

    const request = {} as CopySMChannelRequest;
    request.SMChannelId = smChannel.Id;

    // const t = getUniqueName(newName);
    request.NewName = newName;

    CopySMChannel(request)
      .then(() => {})
      .catch((error) => {
        console.error(error);
      })
      .finally(() => {
        ReturnToParent();
      });
  }, [ReturnToParent, newName, smChannel]);

  return (
    <SMPopUp
      contentWidthSize="3"
      placement={smTableIsSimple ? 'bottom-end' : 'bottom'}
      buttonClassName="icon-orange"
      disabled={newName === undefined || newName === ''}
      icon="pi-clone"
      info=""
      onOkClick={async () => await onSave()}
      // rememberKey={'CloneSMChannelDialog'}
      title="Clone"
    >
      <div className="w-full">
        <StringEditor
          labelInline
          label="New Name"
          darkBackGround
          disableDebounce
          onChange={(e) => e !== undefined && setNewName(e)}
          onSave={(e) => {}}
          value={newName}
        />
      </div>
    </SMPopUp>
  );
};

CloneSMChannelDialog.displayName = 'COPYSMCHANNELDIALOG';

export default React.memo(CloneSMChannelDialog);
