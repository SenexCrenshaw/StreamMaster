import BooleanEditor from '@components/inputs/BooleanEditor';
import NumberEditor from '@components/inputs/NumberEditor';
import StringEditor from '@components/inputs/StringEditor';
import { SMDialogRef } from '@components/sm/SMDialog';
import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import { Logger } from '@lib/common/logger';
import { CreateStreamGroup } from '@lib/smAPI/StreamGroups/StreamGroupsCommands';
import { CreateStreamGroupRequest } from '@lib/smAPI/smapiTypes';
import React, { useCallback, useMemo, useRef, useState } from 'react';

export interface StreamGroupCreateDialogProperties {
  readonly showButton?: boolean | null;
}

export const StreamGroupCreateDialog = ({ showButton }: StreamGroupCreateDialogProperties) => {
  const [saving, setSaving] = useState<boolean>(false);
  const smPopUpRef = useRef<SMPopUpRef>(null);
  const defaultValues = useMemo(
    () =>
      ({
        AutoSetChannelNumbers: true,
        IgnoreExistingChannelNumbers: true,
        Name: '',
        StartChannelNumber: 1
      } as CreateStreamGroupRequest),
    []
  );
  const [createStreamGroupRequest, setCreateStreamGroupRequest] = useState<CreateStreamGroupRequest>(defaultValues);
  const smDialogRef = useRef<SMDialogRef>(null);

  const ReturnToParent = useCallback(() => {
    smDialogRef.current?.hide();
    setCreateStreamGroupRequest({} as CreateStreamGroupRequest);
    setSaving(false);
  }, []);

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

  Logger.debug('StreamGroupCreateDialog', 'render', { createStreamGroupRequest });

  return (
    <SMPopUp
      ref={smPopUpRef}
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
      zIndex={10}
    >
      <div className="w-full">
        <StringEditor
          disableDebounce
          darkBackGround
          autoFocus
          value={createStreamGroupRequest.Name}
          label="Stream Group Name"
          onSave={() => create()}
          onChange={(e) => e !== undefined && updateStateAndRequest({ Name: e })}
        />
        <div className="layout-padding-bottom-lg" />
        <div className="flex w-12 justify-content-end align-content-center">
          <div className="sm-w-6  sm-center-stuff">
            <NumberEditor darkBackGround disableDebounce showButtons label="START CH #" onChange={(e) => {}} value={1} />
          </div>
          <div className="sm-w-6 flex flex-column">
            <BooleanEditor checked={true} label="Fill Channel #" onChange={(e) => {}} />
            <BooleanEditor checked={true} label="Skip Existing #" onChange={(e) => {}} />
          </div>
        </div>
        <div className="layout-padding-bottom-lg" />
      </div>
    </SMPopUp>
  );
};
StreamGroupCreateDialog.displayName = 'StreamGroupCreateDialog';

export default React.memo(StreamGroupCreateDialog);
