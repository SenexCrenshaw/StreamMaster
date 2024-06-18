import StringEditor from '@components/inputs/StringEditor';
import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import { getEnumKeyByValue } from '@lib/common/enumTools';
import { Logger } from '@lib/common/logger';

import { AddOutputProfile } from '@lib/smAPI/Profiles/ProfilesCommands';
import { AddOutputProfileRequest, OutputProfileDto, ValidM3USetting } from '@lib/smAPI/smapiTypes';
import { useCallback, useRef, useState } from 'react';
import FileProfileValueDropDown from './columns/OutputProfileValueDropDown';

const CreateFileProfileDialog = () => {
  const defaultValues = {
    ChannelId: getEnumKeyByValue(ValidM3USetting, ValidM3USetting.Id),
    ChannelNumber: getEnumKeyByValue(ValidM3USetting, ValidM3USetting.ChannelNumber),
    EnableIcon: true,
    GroupTitle: getEnumKeyByValue(ValidM3USetting, ValidM3USetting.Group),
    TVGGroup: getEnumKeyByValue(ValidM3USetting, ValidM3USetting.Group),
    TVGId: getEnumKeyByValue(ValidM3USetting, ValidM3USetting.EPGID),
    TVGName: getEnumKeyByValue(ValidM3USetting, ValidM3USetting.Name)
  } as OutputProfileDto;

  const [fileProfile, setFileProfile] = useState<OutputProfileDto>(defaultValues);
  const smPopUpRef = useRef<SMPopUpRef>(null);
  const [name, setName] = useState<string>();

  const updateM3UOutputProfileStateAndRequest = useCallback(
    (updatedFields: Partial<OutputProfileDto>) => {
      const updatedProfile = { ...fileProfile, ...updatedFields };
      setFileProfile(updatedProfile);
    },
    [fileProfile]
  );

  const dropdownClass = 'sm-w-9rem';

  const save = useCallback(() => {
    const outputProfileDto = {
      ...fileProfile,
      Name: name
    } as OutputProfileDto;

    const request = {
      OutputProfileDto: outputProfileDto
    } as AddOutputProfileRequest;

    AddOutputProfile(request)
      .then((response) => {})
      .catch((error) => {
        Logger.error('CreateFileProfileDialog', { error });
      })
      .finally(() => {
        setName('');
        smPopUpRef.current?.hide();
      });
  }, [fileProfile, name]);

  return (
    <SMPopUp
      contentWidthSize="6"
      buttonClassName="icon-green"
      iconFilled
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
                <FileProfileValueDropDown
                  darkBackGround
                  header="TVG Name"
                  label="TVG Name"
                  value={fileProfile.TVGName}
                  onChange={(e) => {
                    updateM3UOutputProfileStateAndRequest({ TVGName: e.label });
                  }}
                />
              </div>
              <div className={dropdownClass}>
                <FileProfileValueDropDown
                  darkBackGround
                  header="Channel Id"
                  label="Channel Id"
                  value={fileProfile.ChannelId}
                  onChange={(e) => {
                    updateM3UOutputProfileStateAndRequest({ ChannelId: e.label });
                  }}
                />
              </div>
              <div className={dropdownClass}>
                <FileProfileValueDropDown
                  darkBackGround
                  header="Channel #"
                  label="Channel #"
                  value={fileProfile.ChannelNumber}
                  onChange={(e) => {
                    updateM3UOutputProfileStateAndRequest({ ChannelNumber: e.label });
                  }}
                />
              </div>
              <div className={dropdownClass}>
                <FileProfileValueDropDown
                  darkBackGround
                  header="TVG Id"
                  label="TVG Id"
                  value={fileProfile.TVGId}
                  onChange={(e) => {
                    updateM3UOutputProfileStateAndRequest({ TVGId: e.label });
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

export default CreateFileProfileDialog;
