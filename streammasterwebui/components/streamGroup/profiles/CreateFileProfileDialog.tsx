import StringEditor from '@components/inputs/StringEditor';
import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import { getEnumKeyByValue } from '@lib/common/enumTools';
import { Logger } from '@lib/common/logger';

import { AddOutputProfile } from '@lib/smAPI/Profiles/ProfilesCommands';
import { AddOutputProfileRequest, OutputProfileDto, ValidM3USetting } from '@lib/smAPI/smapiTypes';
import { useCallback, useRef, useState } from 'react';
import OutputProfileValueDropDown from './columns/OutputProfileValueDropDown';
import BooleanEditor from '@components/inputs/BooleanEditor';

const CreateFileProfileDialog = () => {
  const defaultValues = {
    EnableChannelNumber: true,
    EnableGroupTitle: true,
    EnableIcon: true,
    EnableId: true,
    EPGId: getEnumKeyByValue(ValidM3USetting, ValidM3USetting.EPGId),
    Group: getEnumKeyByValue(ValidM3USetting, ValidM3USetting.Group),
    IsReadOnly: false,
    Name: getEnumKeyByValue(ValidM3USetting, ValidM3USetting.Name)
  } as OutputProfileDto;

  const [fileProfile, setFileProfile] = useState<OutputProfileDto>(defaultValues);
  const smPopUpRef = useRef<SMPopUpRef>(null);
  const [name, setName] = useState<string>();

  const updateOutputProfileStateAndRequest = useCallback(
    (updatedFields: Partial<OutputProfileDto>) => {
      const updatedProfile = { ...fileProfile, ...updatedFields };
      setFileProfile(updatedProfile);
    },
    [fileProfile]
  );

  const dropdownClass = 'sm-w-6rem';
  const boolClass = 'sm-w-5rem';

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
      icon="pi-plus-circle"
      title="Create Profile"
      ref={smPopUpRef}
      modal
      placement="bottom-end"
      onOkClick={() => {
        save();
      }}
      okButtonDisabled={!name}
      tooltip="Create Profile"
      zIndex={10}
    >
      <>
        <div className="sm-headerBg dialog-padding border-sides">
          <div className="w-12 ">
            <div className="flex gap-1">
              <div className="sm-w-3">
                <StringEditor
                  autoFocus
                  label="Profile Name"
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
                <OutputProfileValueDropDown
                  darkBackGround
                  header="Name Map"
                  label="Name Map"
                  name={name}
                  value={fileProfile.Name}
                  onChange={(e) => {
                    updateOutputProfileStateAndRequest({ Name: e.label });
                  }}
                />
              </div>
              <div className={dropdownClass}>
                <OutputProfileValueDropDown
                  darkBackGround
                  header="EPGId"
                  label="EPGId"
                  name={name}
                  value={fileProfile.EPGId}
                  onChange={(e) => {
                    updateOutputProfileStateAndRequest({ EPGId: e.label });
                  }}
                />
              </div>
              <div className={dropdownClass}>
                <OutputProfileValueDropDown
                  darkBackGround
                  header="Group"
                  label="Group"
                  name={name}
                  value={fileProfile.Group}
                  onChange={(e) => {
                    updateOutputProfileStateAndRequest({ Group: e.label });
                  }}
                />
              </div>
              <div className={boolClass}>
                <BooleanEditor
                  label="Id"
                  checked={fileProfile.EnableId}
                  onChange={(e) => {
                    updateOutputProfileStateAndRequest({ EnableId: e });
                  }}
                />
              </div>
              <div className={boolClass}>
                <BooleanEditor
                  label="Channel #"
                  checked={fileProfile.EnableChannelNumber}
                  onChange={(e) => {
                    updateOutputProfileStateAndRequest({ EnableChannelNumber: e });
                  }}
                />
              </div>
              <div className={boolClass}>
                <BooleanEditor
                  label="Group Title"
                  checked={fileProfile.EnableGroupTitle}
                  onChange={(e) => {
                    updateOutputProfileStateAndRequest({ EnableGroupTitle: e });
                  }}
                />
              </div>
              <div className={boolClass}>
                <BooleanEditor
                  label="Icon"
                  checked={fileProfile.EnableIcon}
                  onChange={(e) => {
                    updateOutputProfileStateAndRequest({ EnableIcon: e });
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