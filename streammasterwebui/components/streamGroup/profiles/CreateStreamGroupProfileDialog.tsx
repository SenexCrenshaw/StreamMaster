import StringEditor from '@components/inputs/StringEditor';
import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import { Logger } from '@lib/common/logger';
import { AddProfileToStreamGroup } from '@lib/smAPI/StreamGroups/StreamGroupsCommands';
import { AddProfileToStreamGroupRequest, StreamGroupDto } from '@lib/smAPI/smapiTypes';
import { useCallback, useRef, useState } from 'react';
import FileProfileDropDown from './FileProfileDropDown';
import VideoProfileDropDown from './VideoProfileDropDown';

interface CreateStreamGroupProfileDialogProps {
  readonly streamGroupDto: StreamGroupDto;
}

const CreateStreamGroupProfileDialog = ({ streamGroupDto }: CreateStreamGroupProfileDialogProps) => {
  const defaultValues = { FileProfileName: 'Default', VideoProfileName: 'Default' } as AddProfileToStreamGroupRequest;

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
        Logger.error('CreateStreamGroupProfileDialog', { error });
      })
      .finally(() => {
        setName('');
        smPopUpRef.current?.hide();
      });
  }, [fileRequest, name, streamGroupDto]);

  return (
    <SMPopUp
      contentWidthSize="3"
      buttonClassName="icon-green"
      icon="pi-plus"
      title="Create Profile"
      ref={smPopUpRef}
      modal
      modalCentered
      onOkClick={() => {
        save();
      }}
      okButtonDisabled={!name}
      tooltip="Create File Profile"
      zIndex={10}
    >
      <>
        <div className="sm-headerBg dialog-padding border-sides">
          <div className="w-12 ">
            <div className="flex gap-1">
              <div className="sm-w-3">
                <StringEditor
                  autoFocus
                  label="Name"
                  placeholder="Name"
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
                <FileProfileDropDown
                  buttonDarkBackground
                  value={fileRequest.FileProfileName}
                  onChange={(e) => {
                    updateM3UOutputProfileStateAndRequest({ FileProfileName: e.Name });
                  }}
                />
              </div>
              <div className={dropdownClass}>
                <VideoProfileDropDown
                  buttonDarkBackground
                  value={fileRequest.FileProfileName}
                  onChange={(e) => {
                    updateM3UOutputProfileStateAndRequest({ FileProfileName: e.Name });
                  }}
                />
              </div>
            </div>
          </div>
        </div>
        <div className="layout-padding-bottom-lg sm-headerBg border-radius-bottom" />
      </>
    </SMPopUp>
  );
};

export default CreateStreamGroupProfileDialog;
