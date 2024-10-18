import StringEditor from '@components/inputs/StringEditor';
import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import { getEnumKeyByValue } from '@lib/common/enumTools';
import { Logger } from '@lib/common/logger';

import BooleanEditor from '@components/inputs/BooleanEditor';
import { AddOutputProfile } from '@lib/smAPI/Profiles/ProfilesCommands';
import { AddOutputProfileRequest, OutputProfileDto, ValidM3USetting } from '@lib/smAPI/smapiTypes';
import { useCallback, useRef, useState } from 'react';
import OutputProfileValueDropDown from './columns/OutputProfileValueDropDown';

const CreateOutputProfileDialog = () => {
  const defaultValues = {
    EnableChannelNumber: true,
    EnableGroupTitle: true,
    EnableIcon: true,
    Group: getEnumKeyByValue(ValidM3USetting, ValidM3USetting.Group),
    Id: getEnumKeyByValue(ValidM3USetting, ValidM3USetting.ChannelNumber),
    IsReadOnly: false,
    Name: getEnumKeyByValue(ValidM3USetting, ValidM3USetting.Name)
    // AppendChannelNumberToId: false
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
  const boolClass = ''; //sm-w-5rem';

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
      contentWidthSize="7"
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
      zIndex={11}
    >
      <>
        <div className="flex flex-column gap-1">
          <div className="layout-padding-bottom-lg" />
          <div className="sm-between-stuff w-full">
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
                header="Id"
                label="Id"
                name={name}
                value={fileProfile.Id}
                onChange={(e) => {
                  updateOutputProfileStateAndRequest({ Id: e.label });
                }}
              />
            </div>
            <div className={dropdownClass}>
              <OutputProfileValueDropDown
                darkBackGround
                header="Name"
                label="Name"
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
                header="Group"
                label="Group"
                name={name}
                value={fileProfile.Group}
                onChange={(e) => {
                  updateOutputProfileStateAndRequest({ Group: e.label });
                }}
              />
            </div>

            {/* <div className="sm-w-2rem pl-2">
              <BooleanEditor
                label="Id"
                checked={fileProfile.EnableId}
                onChange={(e) => {
                  updateOutputProfileStateAndRequest({ EnableId: e });
                }}
              />
            </div> */}
            <div className={boolClass}>
              <BooleanEditor
                label="Enable Channel #"
                checked={fileProfile.EnableChannelNumber}
                onChange={(e) => {
                  updateOutputProfileStateAndRequest({ EnableChannelNumber: e });
                }}
              />
            </div>
            {/* <div className={boolClass}>
              <BooleanEditor
                label="Append Channel #"
                checked={fileProfile.AppendChannelNumberToId}
                onChange={(e) => {
                  updateOutputProfileStateAndRequest({ AppendChannelNumberToId: e });
                }}
              />
            </div> */}
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
        <div className="layout-padding-bottom-lg" />
        <div className="layout-padding-bottom-lg sm-headerBg border-radius-bottom" />
      </>
    </SMPopUp>
  );
};

export default CreateOutputProfileDialog;
