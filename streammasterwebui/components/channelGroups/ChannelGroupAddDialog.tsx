import { type FC, memo, useCallback, useMemo, useState } from 'react';
import { CreateChannelGroupRequest } from '@lib/smAPI/smapiTypes';
import SMDialog from '@components/sm/SMDialog';
import { CreateChannelGroup } from '@lib/smAPI/ChannelGroups/ChannelGroupsCommands';
import StringEditor from '@components/inputs/StringEditor';
import OKButton from '@components/buttons/OKButton';

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
      header={<OKButton disabled={!isSaveEnabled} onClick={addGroup} tooltip="Add Group" />}
    >
      <div className="flex grid justify-content-center align-items-center w-full">
        <div className="flex col-10 mt-1">
          <StringEditor autoFocus disableDebounce label="Name" onChange={(e) => e && setNewGroupName(e)} onSave={(e) => addGroup()} value={newGroupName} />
        </div>
      </div>
    </SMDialog>
  );
};

ChannelGroupAddDialog.displayName = 'Play List Editor';

export default memo(ChannelGroupAddDialog);
