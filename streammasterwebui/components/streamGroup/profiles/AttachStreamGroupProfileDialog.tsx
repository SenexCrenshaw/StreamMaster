import StringEditor from '@components/inputs/StringEditor';
import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import { Logger } from '@lib/common/logger';
import { AddProfileToStreamGroup } from '@lib/smAPI/StreamGroups/StreamGroupsCommands';
import { AddProfileToStreamGroupRequest, StreamGroupDto } from '@lib/smAPI/smapiTypes';
import { useCallback, useRef, useState } from 'react';
import OutputProfileDropDown from './OutputProfileDropDown';

interface AttachStreamGroupProfileDialogProps {
  readonly streamGroupDto: StreamGroupDto;
}

const AttachStreamGroupProfileDialog = ({ streamGroupDto }: AttachStreamGroupProfileDialogProps) => {
  const defaultValues = { OutputProfileName: 'Default', VideoProfileName: 'StreamMaster' } as AddProfileToStreamGroupRequest;
  const [fileRequest, setFileRequest] = useState<AddProfileToStreamGroupRequest>(defaultValues);
  const smPopUpRef = useRef<SMPopUpRef>(null);
  const [name, setName] = useState<string>();

  const updateM3UOutputProfileStateAndRequest = useCallback(
    (updatedFields: Partial<AddProfileToStreamGroupRequest>) => {
      const updatedProfile = { ...fileRequest, ...updatedFields };
      setFileRequest(updatedProfile);
    },
    [fileRequest]
  );

  const dropdownClass = 'sm-w-9rem';

  const save = useCallback(() => {
    if (!name || !streamGroupDto) return;
    fileRequest.Name = name;
    fileRequest.StreamGroupId = streamGroupDto.Id;
    AddProfileToStreamGroup(fileRequest)
      .then((response) => {})
      .catch((error) => {
        Logger.error('AttachStreamGroupProfileDialog', { error });
      })
      .finally(() => {
        setName('');
        smPopUpRef.current?.hide();
      });
  }, [fileRequest, name, streamGroupDto]);

  return (
    <SMPopUp
      buttonClassName="icon-green"
      contentWidthSize="3"
      icon="pi-plus-circle"
      modal
      ref={smPopUpRef}
      title="Attach Profile"
      onOkClick={() => {
        save();
      }}
      okButtonDisabled={!name}
      tooltip="Attach Profile"
      zIndex={12}
    >
      <>
        <div className="sm-headerBg dialog-padding border-sides">
          <div className="layout-padding-bottom-lg" />
          <div className="sm-center-stuff gap-1">
            <div className="sm-w-6">
              <StringEditor
                autoFocus
                label="Profile Name"
                placeholder="Profile Name"
                darkBackGround
                disableDebounce
                onChange={(e) => {
                  setName(e);
                }}
                onSave={(e) => {
                  save();
                }}
                value={name}
              />
            </div>
            <div className={dropdownClass}>
              <OutputProfileDropDown
                buttonDarkBackground
                value={fileRequest.OutputProfileName}
                onChange={(e) => {
                  updateM3UOutputProfileStateAndRequest({ OutputProfileName: e.ProfileName });
                }}
              />
            </div>
            {/* <div className={dropdownClass}>
              <VideoProfileDropDown
                buttonDarkBackground
                value={fileRequest.VideoProfileName}
                onChange={(e) => {
                  updateM3UOutputProfileStateAndRequest({ VideoProfileName: e.ProfileName });
                }}
              />
            </div> */}
          </div>

          <div className="layout-padding-bottom-lg" />
        </div>
        <div className="layout-padding-bottom-lg sm-headerBg border-radius-bottom" />
      </>
    </SMPopUp>
  );
};

export default AttachStreamGroupProfileDialog;
