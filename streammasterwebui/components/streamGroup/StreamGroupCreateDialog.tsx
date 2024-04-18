import React, { useCallback, useMemo, useState } from 'react';
import AddButton from '@components/buttons/AddButton';
import { SMCard } from '@components/sm/SMCard';
import XButton from '@components/buttons/XButton';
import { CreateStreamGroupRequest } from '@lib/smAPI/smapiTypes';
import { Dialog } from 'primereact/dialog';
import TextInput from '@components/inputs/TextInput';
import SaveButton from '@components/buttons/SaveButton';
import { CreateStreamGroup } from '@lib/smAPI/StreamGroups/StreamGroupsCommands';
// import useScrollAndKeyEvents from '@lib/hooks/useScrollAndKeyEvents';

export interface StreamGroupCreateDialogProperties {
  readonly onHide?: (didUpload: boolean) => void;
  readonly showButton?: boolean | null;
}

export const StreamGroupCreateDialog = ({ onHide, showButton }: StreamGroupCreateDialogProperties) => {
  const [visible, setVisible] = useState<boolean>(false);
  const [name, setName] = useState<string>('');
  const [saving, setSaving] = useState<boolean>(false);

  let localSave = false;

  const ReturnToParent = useCallback(
    (didUpload?: boolean) => {
      setVisible(false);
      setName('');
      setSaving(false);
      onHide?.(didUpload ?? false);
    },
    [onHide]
  );

  const create = useCallback(() => {
    if (saving || localSave) {
      return;
    }
    localSave = true;
    setSaving(true);

    const request = {} as CreateStreamGroupRequest;
    request.Name = name;

    CreateStreamGroup(request)
      .then(() => {})
      .catch((error) => {
        console.error('Error Adding SG', error);
      })
      .finally(() => {
        ReturnToParent(true);
      });
  }, [ReturnToParent, name, saving]);

  const isSaveEnabled = useMemo(() => {
    return name !== undefined && name !== '';
  }, [name]);

  return (
    <>
      <Dialog
        visible={visible}
        style={{ width: '40vw' }}
        onHide={() => ReturnToParent()}
        content={({ hide }) => (
          <SMCard title="ADD STREAM GROUP" header={<XButton iconFilled={false} onClick={(e) => hide(e)} tooltip="Close" />}>
            <div className="sm-fileupload w-12 p-0 m-0 ">
              <div className="flex">
                <div className="w-6 pt-4">
                  <div className="w-12 p-2">
                    <TextInput autoFocus value={name} label="Stream Group Name" onEnter={() => create()} onChange={(e) => setName(e)} />
                  </div>
                </div>
              </div>
              <div className="flex w-12 gap-2 justify-content-end align-content-center pr-3">
                {/* <ResetButton disabled={!isSaveEnabled} onClick={() => {}} /> */}
                <SaveButton
                  disabled={!isSaveEnabled}
                  label="Add Stream Group"
                  onClick={() => {
                    create();
                  }}
                />
              </div>
            </div>
          </SMCard>
        )}
      />
      <div hidden={showButton === false} className="justify-content-center">
        <AddButton
          onClick={(e) => {
            setVisible(true);
          }}
          tooltip="Add EPG File"
          iconFilled={false}
        />
      </div>
    </>
  );
};
StreamGroupCreateDialog.displayName = 'StreamGroupCreateDialog';

export default React.memo(StreamGroupCreateDialog);
