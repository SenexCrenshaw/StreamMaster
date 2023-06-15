/* eslint-disable @typescript-eslint/no-unused-vars */

import './SettingsEditor.css';
import { Button } from 'primereact/button';

import React from 'react';
import { Fieldset } from 'primereact/fieldset';
import { useSettingsGetSettingQuery, type SettingDto } from '../../store/iptvApi';
import { UpdateSetting } from '../../store/signlar_functions';
import { SettingsEditorIcon } from '../../common/icons';
import { Checkbox } from 'primereact/checkbox';
import { InputText } from 'primereact/inputtext';

import { type MenuItem } from 'primereact/menuitem';
import { Dock } from 'primereact/dock';
import SaveIcon from '@mui/icons-material/Save';

import HistoryIcon from '@mui/icons-material/History';
import { Toast } from 'primereact/toast';
import { Dropdown } from 'primereact/dropdown';
import { StreamingProxyTypes } from '../../store/streammaster_enums';
import { type SelectItem } from 'primereact/selectitem';
import { InputNumber } from 'primereact/inputnumber';
import { Password } from 'primereact/password';
import { type UserInformation } from '../../common/common';
import { GetMessage, GetMessageDiv, getTopToolOptions } from '../../common/common';

import { baseHostURL, requiresAuth } from '../../settings';

import { ScrollPanel } from 'primereact/scrollpanel';
import { useLocalStorage } from 'primereact/hooks';
// import { useAppSelector, useAppDispatch } from '../../app/hooks';
// import { selectUserInformation, setUserInformation } from '../../store/userSlice';

export const SettingsEditor = (props: SettingsEditorProps) => {
  const toast = React.useRef<Toast>(null);
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  // const [userInformation, setUserInformation] = useLocalStorage<UserInformation>({ IsAuthenticated: false, TokenAge: new Date() } as UserInformation, 'userInformation');



  const [newData, setNewData] = React.useState<SettingDto>({} as SettingDto);
  const [originalData, setOriginalData] = React.useState<SettingDto>({} as SettingDto);

  const settingsQuery = useSettingsGetSettingQuery();


  React.useMemo(() => {
    if (settingsQuery.isLoading || !settingsQuery.data)
      return;

    setNewData({ ...settingsQuery.data });
    setOriginalData({ ...settingsQuery.data });

  }, [settingsQuery]);

  const isSaveEnabled = React.useMemo((): boolean => {
    if (JSON.stringify(newData) === JSON.stringify(originalData))
      return false;

    return true;
  }, [newData, originalData]);

  const getLine = React.useCallback((label: string, value: React.ReactElement) => {
    return (
      <div className='flex col-12'>
        <div className='flex col-2 col-offset-1'>
          <span>{label}</span>
        </div>
        <div className='flex col-3 m-0 p-0 debug'>
          {value}
        </div>
      </div>
    );
  }, []);

  const getRecord = React.useCallback((fieldName: string) => {
    type ObjectKey = keyof typeof newData;
    const record = newData[fieldName as ObjectKey];
    if (record === undefined || record === null || record === '') {
      return undefined;
    }

    return record;
  }, [newData]);

  const getRecordString = React.useCallback((fieldName: string): string => {
    const record = getRecord(fieldName);
    let toDisplay = JSON.stringify(record);

    if (!toDisplay || toDisplay === undefined || toDisplay === '') {
      return '';
    }

    if (toDisplay.startsWith('"') && toDisplay.endsWith('"')) {
      toDisplay = toDisplay.substring(1, toDisplay.length - 1);
    }

    return toDisplay;
  }, [getRecord]);

  const getInputNumberLine = React.useCallback((field: string, max?: number | null) => {
    const label = GetMessage(field);
    return (
      getLine(label + ':',
        <InputNumber
          className="withpadding w-full text-left"
          max={max === null ? 64 : max}
          min={0}
          onValueChange={(e) => setNewData({ ...newData, [field]: e.target.value })}
          placeholder={label}
          showButtons
          size={3}
          value={getRecord(field) as number}
        />)
    );
  }, [getLine, getRecord, newData]);


  const getPasswordLine = React.useCallback((field: string) => {
    const label = GetMessage(field);
    return (
      getLine(label + ':',
        <Password
          className="withpadding"
          feedback={false}
          onChange={(e) => setNewData({ ...newData, [field]: e.target.value })}
          placeholder={label}
          toggleMask
          value={getRecordString(field)}
        />)
    );
  }, [getLine, getRecordString, newData]);

  const getInputTextLine = React.useCallback((field: string) => {
    const label = GetMessage(field);
    return (
      getLine(label + ':',
        <InputText
          className="withpadding w-full text-left"
          onChange={(e) => setNewData({ ...newData, [field]: e.target.value })}
          placeholder={label}
          value={getRecordString(field)}
        />)
    );
  }, [getLine, getRecordString, newData]);

  const getCheckBoxLine = React.useCallback((field: string) => {
    const label = GetMessage(field);
    return (
      getLine(label + ':',
        <Checkbox
          checked={getRecord(field) as boolean}
          className="w-full text-left"
          onChange={(e) => setNewData({ ...newData, [field]: !e.target.value })}
          placeholder={label}
          value={getRecord(field) as boolean}
        />)
    );
  }, [getLine, getRecord, newData]);



  const getHandlersOptions = (): SelectItem[] => {
    const test = Object.entries(StreamingProxyTypes)
      .splice(0, Object.keys(StreamingProxyTypes).length / 2)
      .map(([number, word]) => {
        return {
          label: word,
          value: number,
        } as SelectItem;
      });

    return test;
  };

  const getDropDownLine = React.useCallback((field: string, options: SelectItem[]) => {
    const label = GetMessage(field);
    return (
      <>
        {
          getLine(label + ':',
            <Dropdown
              className="withpadding w-full text-left"
              onChange={(e) => setNewData({ ...newData, [field]: parseInt(e.target.value) })}
              options={options}
              placeholder={label}
              value={getRecordString(field)}
            />)
        }
      </>
    );
  }, [getLine, getRecordString, newData]);

  const onSave = React.useCallback(() => {
    if (!isSaveEnabled) {
      return;
    }

    UpdateSetting(newData)
      .then((returnData) => {
        if (toast.current) {
          if (returnData) {
            toast.current.show({
              detail: `Update Settings Successful`,
              life: 3000,
              severity: 'success',
              summary: 'Successful',
            });

            if (
              (newData.adminPassword !== undefined && newData.adminPassword !== '') ||
              (newData.adminUserName !== undefined && newData.adminUserName !== '')
            ) {
              props.logOut();
            }

          } else {
            toast.current.show({
              detail: `Update Settings Failed`,
              life: 3000,
              severity: 'error',
              summary: 'Error',
            });
          }
        }
      }).catch((e) => {
        if (toast.current) {
          toast.current.show({
            detail: `Update Settings Failed`,
            life: 3000,
            severity: 'error',
            summary: 'Error ' + e.message,
          });
        }
      });
  }, [isSaveEnabled, newData, props]);

  const items: MenuItem[] = [
    {
      command: () => {
        onSave();
      },
      disabled: !isSaveEnabled,
      icon: <SaveIcon sx={{ fontSize: 40 }} />,
      label: 'Save',
    },
    {
      command: () => {
        setNewData({ ...originalData });
        toast.current?.show({ detail: 'Undo', life: 3000, severity: 'info', summary: 'Info' });
      },
      disabled: !isSaveEnabled,
      icon: <HistoryIcon sx={{ fontSize: 40 }} />,
      label: 'Undo',
    },
  ];


  return (

    <div className="settingsEditor">
      <ScrollPanel style={{ height: 'calc(100vh - 18px)', width: '100%' }}>
        <Toast position="bottom-right" ref={toast} />
        <Dock model={items} position='right' />
        <div className="justify-content-between align-items-center">
          <div className="flex justify-content-start align-items-center w-full text-left font-bold text-white-500 surface-overlay justify-content-start align-items-center">
            <SettingsEditorIcon className='p-0 mr-1' />
            {GetMessageDiv('settings', true)}
          </div >

          <Fieldset className="mt-4 pt-10" legend={GetMessage('general')}>
            {getInputTextLine('deviceID')}
            {getCheckBoxLine('cleanURLs')}
            {getInputTextLine('ffmPegExecutable')}
            {getCheckBoxLine('overWriteM3UChannels')}
          </Fieldset>

          <Fieldset className="mt-4 pt-10" legend={GetMessage('auth')}>
            {getInputTextLine('adminUserName')}
            {getPasswordLine('admninPassword')}
            {getInputTextLine('apiUserName')}
            {getPasswordLine('apiPassword')}
            <div className='flex col-12'>
              <div className='flex col-2 col-offset-1'>
                <span>{GetMessage('signout')}</span>
              </div>
              <div className='flex col-3 m-0 p-0 debug'>
                <Button
                  disabled={!props.isAuthenticated || requiresAuth !== true}
                  icon="pi pi-check"
                  label={GetMessage('signout')}
                  onClick={() =>
                    props.logOut()
                  }
                  rounded
                  severity="success"
                  size="small"
                />
              </div>
            </div>

          </Fieldset>

          <Fieldset className="mt-4 pt-10" legend={GetMessage('streaming')}>
            {getDropDownLine('streamingProxyType', getHandlersOptions())}
            {getInputNumberLine('ringBufferSizeMB')}
            {getInputNumberLine('maxConnectRetry', 999)}
            {getInputNumberLine('maxConnectRetryTimeMS', 999)}
          </Fieldset>

          <Fieldset className="mt-4 pt-10" legend={GetMessage('filesEPG')} >
            {getCheckBoxLine('cacheIcons')}
            {getInputTextLine('sdUserName')}
            {getPasswordLine('sdPassword')}
          </Fieldset>

          {/* <Fieldset className="mt-4 pt-10" legend={GetMessage('backup')} /> */}

          <Fieldset className="mt-4 pt-10" legend={GetMessage('development')} >
            <Button
              icon='pi pi-bookmark-fill'
              label='Swagger'
              onClick={() => {
                const link = `${baseHostURL}swagger`;
                window.open(link);
              }
              }
              tooltip="Swagger Link"
              tooltipOptions={getTopToolOptions}
            />
          </Fieldset>
        </div >
      </ScrollPanel>
    </div >

  );
};

SettingsEditor.displayName = 'Settings';

type SettingsEditorProps = {
  isAuthenticated: boolean;
  logOut: () => void;
}

export default React.memo(SettingsEditor);
