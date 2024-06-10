import { SMDialogRef } from '@components/sm/SMDialog';
import { CreateChannelGroupRequest } from '@lib/smAPI/smapiTypes';
import { memo, useCallback, useMemo, useRef, useState, type FC } from 'react';

import StringEditor from '@components/inputs/StringEditor';
import { SMPopUp } from '@components/sm/SMPopUp';
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
    <SMPopUp
      buttonClassName="icon-green"
      icon="pi-plus"
      iconFilled
      modal
      onOkClick={() => addGroup()}
      okButtonDisabled={!isSaveEnabled}
      placement="bottom-end"
      showRemember={false}
      title="Create Group"
      tooltip="Create Group"
    >
      <div className="flex grid justify-content-center align-items-center w-full">
        <div className="flex col-10 mt-1">
          <StringEditor
            autoFocus
            darkBackGround
            disableDebounce
            label="Name"
            labelInline
            onChange={(e) => e !== undefined && setNewGroupName(e)}
            onSave={(e) => addGroup()}
            value={newGroupName}
          />
        </div>
      </div>
    </SMPopUp>
  );
};

ChannelGroupAddDialog.displayName = 'Play List Editor';

export default memo(ChannelGroupAddDialog);
