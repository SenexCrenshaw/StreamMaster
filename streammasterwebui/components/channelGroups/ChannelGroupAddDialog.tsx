import SMDialog, { SMDialogRef } from '@components/sm/SMDialog';
import { CreateChannelGroupRequest } from '@lib/smAPI/smapiTypes';
import { memo, useCallback, useMemo, useRef, useState, type FC } from 'react';

import OKButton from '@components/buttons/OKButton';
import StringEditor from '@components/inputs/StringEditor';
import { CreateChannelGroup } from '@lib/smAPI/ChannelGroups/ChannelGroupsCommands';

const ChannelGroupAddDialog: FC = () => {
  const [newGroupName, setNewGroupName] = useState<string>('');
  const dialogRef = useRef<SMDialogRef>(null);
  const ReturnToParent = useCallback(() => {
    setNewGroupName('');
    dialogRef.current?.hide();
  }, []);

  const addGroup = useCallback(() => {
    if (!newGroupName) {
      ReturnToParent();

      return;
    }

    const requestData: CreateChannelGroupRequest = {
      GroupName: newGroupName,
      IsReadOnly: false
    };

    CreateChannelGroup(requestData)
      .then(() => {})
      .catch((error: unknown) => {
        console.error(error);
      })
      .finally(() => {
        ReturnToParent();
      });
  }, [ReturnToParent, newGroupName]);

  const isSaveEnabled = useMemo((): boolean => {
    if (newGroupName && newGroupName !== '') {
      return true;
    }

    return false;
  }, [newGroupName]);

  return (
    <SMDialog
      buttonClassName="icon-green"
      header={<OKButton disabled={!isSaveEnabled} onClick={addGroup} tooltip="Add Group" />}
      icon="pi-plus"
      iconFilled
      info="General"
      onHide={() => ReturnToParent()}
      ref={dialogRef}
      title="CREATE GROUP"
      tooltip="Create Group"
      widthSize={2}
    >
      <div className="flex grid justify-content-center align-items-center w-full">
        <div className="flex col-10 mt-1">
          <StringEditor
            autoFocus
            darkBackGround
            disableDebounce
            label="Name"
            labelInline
            onChange={(e) => e && setNewGroupName(e)}
            onSave={(e) => addGroup()}
            value={newGroupName}
          />
        </div>
      </div>
    </SMDialog>
  );
};

ChannelGroupAddDialog.displayName = 'Play List Editor';

export default memo(ChannelGroupAddDialog);
