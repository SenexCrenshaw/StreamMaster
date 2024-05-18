import { type FC, memo, useCallback, useMemo, useState } from 'react';
import AddButton from '../buttons/AddButton';
import { CreateChannelGroupRequest } from '@lib/smAPI/smapiTypes';
import SMDialog from '@components/sm/SMDialog';
import { CreateChannelGroup } from '@lib/smAPI/ChannelGroups/ChannelGroupsCommands';
import StringEditor from '@components/inputs/StringEditor';

const ChannelGroupAddDialog: FC = () => {
  const [newGroupName, setNewGroupName] = useState<string>('');

  const ReturnToParent = useCallback(() => {
    setNewGroupName('');
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
      title="CREATE GROUP"
      iconFilled
      onHide={() => ReturnToParent()}
      buttonClassName="icon-green"
      icon="pi-plus"
      widthSize={2}
      info="General"
      tooltip="Create Group"
    >
      <div className="flex grid justify-content-center align-items-center w-full">
        <div className="flex col-10 mt-1">
          <StringEditor disableDebounce label="Name" onChange={(e) => e && setNewGroupName(e)} onSave={(e) => e && setNewGroupName(e)} value={newGroupName} />
        </div>
        <div className="flex col-12 justify-content-end">
          <AddButton disabled={!isSaveEnabled} label="Add Group" onClick={addGroup} tooltip="Add Group" />
        </div>
      </div>
    </SMDialog>
  );
};

ChannelGroupAddDialog.displayName = 'Play List Editor';

export default memo(ChannelGroupAddDialog);
