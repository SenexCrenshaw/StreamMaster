import StringEditor from '@components/inputs/StringEditor';
import { SMDialogRef } from '@components/sm/SMDialog';
import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import { generateKey } from '@lib/common/crypto';
import { CreateStreamGroup } from '@lib/smAPI/StreamGroups/StreamGroupsCommands';
import { CreateStreamGroupRequest } from '@lib/smAPI/smapiTypes';
import React, { useCallback, useMemo, useRef, useState } from 'react';
import CommandProfileDropDown from '../profiles/CommandProfileDropDown';
import OutputProfileDropDown from '../profiles/OutputProfileDropDown';

export interface StreamGroupCreateDialogProperties {
  readonly showButton?: boolean | null;
  readonly zIndex?: number | undefined;
  readonly modal?: boolean | null;
}

export const StreamGroupCreateDialog = ({ modal, zIndex }: StreamGroupCreateDialogProperties) => {
  const [saving, setSaving] = useState<boolean>(false);
  const smPopUpRef = useRef<SMPopUpRef>(null);
  const defaultValues = useMemo(
    () =>
      ({
        CommandProfileName: 'Default',
        GroupKey: generateKey(),
        Name: '',
        OutputProfileName: 'Default'
      } as CreateStreamGroupRequest),
    []
  );
  const [createStreamGroupRequest, setCreateStreamGroupRequest] = useState<CreateStreamGroupRequest>(defaultValues);
  const smDialogRef = useRef<SMDialogRef>(null);

  const ReturnToParent = useCallback(() => {
    smDialogRef.current?.hide();
    setCreateStreamGroupRequest(defaultValues);
    setSaving(false);
  }, [defaultValues]);

  const create = useCallback(() => {
    if (saving || createStreamGroupRequest === undefined || createStreamGroupRequest.Name === undefined || createStreamGroupRequest.Name === '') {
      return;
    }

    setSaving(true);

    CreateStreamGroup(createStreamGroupRequest)
      .then(() => {})
      .catch((error) => {
        console.error('Error Adding SG', error);
      })
      .finally(() => {
        smDialogRef.current?.hide();
        smPopUpRef.current?.hide();
        ReturnToParent();
      });
  }, [ReturnToParent, createStreamGroupRequest, saving]);

  const updateStateAndRequest = useCallback(
    (updatedFields: Partial<CreateStreamGroupRequest>) => {
      const updatedRequest = { ...createStreamGroupRequest, ...updatedFields };
      setCreateStreamGroupRequest(updatedRequest);
    },
    [createStreamGroupRequest]
  );

  const isSaveEnabled = useMemo(() => {
    if (createStreamGroupRequest === undefined) {
      return false;
    }
    if (createStreamGroupRequest.Name === '') {
      return false;
    }
    return true;
  }, [createStreamGroupRequest]);

  return (
    <SMPopUp
      contentWidthSize="4"
      icon="pi-plus"
      iconFilled
      ref={smPopUpRef}
      title="ADD SG"
      modal
      onOkClick={() => {
        create();
      }}
      buttonClassName="icon-green"
      okButtonDisabled={!isSaveEnabled}
      onCloseClick={() => ReturnToParent()}
      tooltip="Add SG"
      placement="bottom-end"
      zIndex={12}
    >
      <div className="sm-center-stuff p-2">
        <div className="sm-w-3">
          <StringEditor
            autoFocus
            darkBackGround
            disableDebounce
            label="Name"
            onChange={(e) => e !== undefined && updateStateAndRequest({ Name: e })}
            onSave={() => create()}
            value={createStreamGroupRequest.Name}
          />
        </div>
        <div className="sm-w-3">
          <StringEditor
            darkBackGround
            disableDebounce
            label="Group Key"
            onChange={(e) => e !== undefined && updateStateAndRequest({ GroupKey: e })}
            onSave={() => create()}
            value={createStreamGroupRequest.GroupKey}
          />
        </div>
        <div className="sm-w-3">
          <OutputProfileDropDown
            buttonDarkBackground
            value={createStreamGroupRequest.OutputProfileName ?? ''}
            onChange={(e) => e !== undefined && updateStateAndRequest({ OutputProfileName: e.ProfileName })}
          />
        </div>
        <div className="sm-w-3">
          <CommandProfileDropDown
            buttonDarkBackground
            value={createStreamGroupRequest.CommandProfileName ?? ''}
            onChange={(e) => e !== undefined && updateStateAndRequest({ CommandProfileName: e.ProfileName })}
          />
        </div>

        <div className="layout-padding-bottom-lg" />
        <div className="flex w-12 justify-content-end align-content-center"></div>
        <div className="layout-padding-bottom-lg" />
      </div>
    </SMPopUp>
  );
};
StreamGroupCreateDialog.displayName = 'StreamGroupCreateDialog';

export default React.memo(StreamGroupCreateDialog);
