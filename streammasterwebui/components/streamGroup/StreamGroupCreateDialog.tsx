import StringEditor from '@components/inputs/StringEditor';
import { SMDialogRef } from '@components/sm/SMDialog';
import SMPopUp from '@components/sm/SMPopUp';
import { CreateStreamGroup } from '@lib/smAPI/StreamGroups/StreamGroupsCommands';
import { CreateStreamGroupRequest } from '@lib/smAPI/smapiTypes';
import React, { useCallback, useMemo, useRef, useState } from 'react';
// import useScrollAndKeyEvents from '@lib/hooks/useScrollAndKeyEvents';

export interface StreamGroupCreateDialogProperties {
  readonly onHide?: (didUpload: boolean) => void;
  readonly showButton?: boolean | null;
}

export const StreamGroupCreateDialog = ({ onHide, showButton }: StreamGroupCreateDialogProperties) => {
  const [name, setName] = useState<string>('');
  const [saving, setSaving] = useState<boolean>(false);
  const smDialogRef = useRef<SMDialogRef>(null);

  const ReturnToParent = useCallback(
    (didUpload?: boolean) => {
      smDialogRef.current?.hide();
      setName('');
      setSaving(false);
      onHide?.(didUpload ?? false);
    },
    [onHide]
  );

  const create = useCallback(() => {
    if (saving) {
      return;
    }

    setSaving(true);

    const request = {} as CreateStreamGroupRequest;
    request.Name = name;

    CreateStreamGroup(request)
      .then(() => {})
      .catch((error) => {
        console.error('Error Adding SG', error);
      })
      .finally(() => {
        smDialogRef.current?.hide();
        // ReturnToParent(true);
      });
  }, [name, saving]);

  const isSaveEnabled = useMemo(() => {
    return name !== undefined && name !== '';
  }, [name]);

  return (
    <SMPopUp
      icon="pi-plus"
      iconFilled
      contentWidthSize="2"
      title="ADD SG"
      onOkClick={() => {
        create();
      }}
      onCloseClick={() => ReturnToParent()}
      okButtonDisabled={!isSaveEnabled}
      buttonClassName="icon-green"
      tooltip="Add SG"
    >
      <div className="sm-w-10rem">
        <StringEditor
          disableDebounce
          darkBackGround
          autoFocus
          value={name}
          label="Stream Group Name"
          onSave={() => create()}
          onChange={(e) => e !== undefined && setName(e)}
        />
        {/* <div className="layout-padding-bottom-lg" />
        <div className="flex w-12 justify-content-end align-content-center">
          <SaveButton
            buttonDisabled={!isSaveEnabled}
            label="Add Stream Group"
            onClick={() => {
              create();
            }}
          />
        </div> */}
        <div className="layout-padding-bottom-lg" />
      </div>
    </SMPopUp>
  );
};
StreamGroupCreateDialog.displayName = 'StreamGroupCreateDialog';

export default React.memo(StreamGroupCreateDialog);
