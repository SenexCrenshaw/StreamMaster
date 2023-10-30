import { Button } from 'primereact/button';

import { SMTextColor } from '@components/SMTextColor';
import TextInput from '@components/inputs/TextInput';
import SettingsNameRegexDataSelector from '@components/settings/SettingsNameRegexDataSelector';
import { SettingsEditorIcon } from '@lib/common/Icons';
import { GetMessage, getTopToolOptions } from '@lib/common/common';
import { AuthenticationType, StreamingProxyTypes } from '@lib/common/streammaster_enums';
import { SettingDto, useSettingsGetSettingQuery } from '@lib/iptvApi';
import { getDefaultSetting } from '@lib/locales/default_setting';
import { getHelp } from '@lib/locales/help_en';
import { baseHostURL } from '@lib/settings';
import { UpdateSetting } from '@lib/smAPI/Settings/SettingsMutateAPI';
import useSettings from '@lib/useSettings';
import HistoryIcon from '@mui/icons-material/History';
import SaveIcon from '@mui/icons-material/Save';
import { Checkbox } from 'primereact/checkbox';
import { Dock } from 'primereact/dock';
import { Dropdown } from 'primereact/dropdown';
import { Fieldset } from 'primereact/fieldset';
import { InputNumber } from 'primereact/inputnumber';
import { type MenuItem } from 'primereact/menuitem';
import { Password } from 'primereact/password';
import { ScrollPanel } from 'primereact/scrollpanel';
import { type SelectItem } from 'primereact/selectitem';
import React, { ReactElement, memo, useCallback, useEffect, useMemo, useState } from 'react';

const StandardHeader = React.lazy(() => import('@components/StandardHeader'));

export const SettingsEditor = () => {
  // const toast = useRef<Toast>(null)
  const setting = useSettings();
  const [newData, setNewData] = useState<SettingDto>({} as SettingDto);
  const [originalData, setOriginalData] = useState<SettingDto>({} as SettingDto);

  const settingsQuery = useSettingsGetSettingQuery();

  useEffect(() => {
    if (settingsQuery.isLoading || !settingsQuery.data) return;

    setNewData({ ...settingsQuery.data });
    setOriginalData({ ...settingsQuery.data });
  }, [settingsQuery]);

  const adminUserNameError = useMemo((): string | undefined => {
    if (newData.authenticationMethod === AuthenticationType.Forms && newData.adminUserName === '') return GetMessage('formsAuthRequiresAdminUserName');

    return undefined;
  }, [newData.adminUserName, newData.authenticationMethod]);

  const adminPasswordError = useMemo((): string | undefined => {
    if (newData.authenticationMethod === AuthenticationType.Forms && newData.adminPassword === '') return GetMessage('formsAuthRequiresAdminPassword');

    return undefined;
  }, [newData.adminPassword, newData.authenticationMethod]);

  const isSaveEnabled = useMemo((): boolean => {
    if (JSON.stringify(newData) === JSON.stringify(originalData)) return false;

    if (adminUserNameError !== undefined || adminPasswordError !== undefined) {
      return false;
    }

    if (newData.enableSSL === true && newData.sslCertPath === '') {
      return false;
    }

    return true;
  }, [adminPasswordError, adminUserNameError, newData, originalData]);

  const getLine = useCallback(
    (label: string, value: ReactElement, help?: string | null, defaultSetting?: string | null) => (
      <div className="flex col-12 align-content-center">
        <div className="flex col-2 col-offset-1">{label}</div>
        <div className="flex col-3 m-0 p-0 debug">{value}</div>
        {help !== null && help !== undefined && (
          <div className="flex flex-column col-3 text-sm align-content-center col-offset-1 debug">
            {help}
            {defaultSetting && <SMTextColor italicized text={defaultSetting} />}
          </div>
        )}
      </div>
    ),
    []
  );

  const getRecord = useCallback(
    (fieldName: string) => {
      type ObjectKey = keyof typeof newData;
      const record = newData[fieldName as ObjectKey];

      if (record === undefined || record === null || record === '') {
        return;
      }

      return record;
    },
    [newData]
  );

  const getRecordString = useCallback(
    (fieldName: string): string => {
      const record = getRecord(fieldName);
      let toDisplay = JSON.stringify(record);

      if (!toDisplay || toDisplay === undefined || toDisplay === '') {
        return '';
      }

      if (toDisplay.startsWith('"') && toDisplay.endsWith('"')) {
        toDisplay = toDisplay.substring(1, toDisplay.length - 1);
      }

      return toDisplay;
    },
    [getRecord]
  );

  const getInputNumberLine = useCallback(
    (field: string, max?: number | null) => {
      const label = GetMessage(field);
      const help = getHelp(field);

      return getLine(
        `${label}:`,
        <InputNumber
          className="withpadding w-full text-left"
          max={max === null ? 64 : max}
          min={0}
          onValueChange={(e) => setNewData({ ...newData, [field]: e.target.value })}
          placeholder={label}
          showButtons
          size={3}
          value={getRecord(field) as number}
        />,
        help
      );
    },
    [getLine, getRecord, newData]
  );

  const getPasswordLine = useCallback(
    (field: string, warning?: string | null) => {
      const label = GetMessage(field);
      const help = getHelp(field);

      return getLine(
        `${label}:`,
        <span className="w-full">
          <Password
            className="password withpadding w-full text-left"
            feedback={false}
            onChange={(e) => setNewData({ ...newData, [field]: e.target.value })}
            placeholder={label}
            toggleMask
            value={getRecordString(field)}
          />
          <br />
          {warning !== null && warning !== undefined && <span className="text-xs text-orange-500">{warning}</span>}
        </span>,
        help
      );
    },
    [getLine, getRecordString, newData]
  );

  const getInputTextLine = useCallback(
    (field: string, warning?: string | null) => {
      const label = GetMessage(field);
      const help = getHelp(field);
      const defaultSetting = getDefaultSetting(field);

      return getLine(
        `${label}:`,
        <span className="w-full">
          <TextInput dontValidate onChange={(e) => setNewData({ ...newData, [field]: e })} placeHolder={label} showCopy value={getRecordString(field)} />
          <br />
          {warning !== null && warning !== undefined && <span className="text-xs text-orange-500">{warning}</span>}
        </span>,
        help,
        defaultSetting
      );
    },
    [getLine, getRecordString, newData]
  );

  const getCheckBoxLine = useCallback(
    (field: string) => {
      const label = GetMessage(field);
      const help = getHelp(field);

      return getLine(
        `${label}:`,
        <Checkbox
          checked={getRecord(field) as boolean}
          className="w-full text-left"
          onChange={(e) => setNewData({ ...newData, [field]: !e.target.value })}
          placeholder={label}
          value={getRecord(field) as boolean}
        />,
        help
      );
    },
    [getLine, getRecord, newData]
  );

  const getHandlersOptions = (): SelectItem[] => {
    const test = Object.entries(StreamingProxyTypes)
      .splice(0, Object.keys(StreamingProxyTypes).length / 2)
      .map(
        ([number, word]) => ({
          label: word,
          value: number
        } as SelectItem)
      );

    return test;
  };

  const getAuthTypeOptions = (): SelectItem[] => {
    const test = Object.entries(AuthenticationType)
      .splice(0, Object.keys(AuthenticationType).length / 2)
      .map(
        ([number, word]) => ({
          label: word,
          value: number
        } as SelectItem)
      );

    return test;
  };

  const getDropDownLine = useCallback(
    (field: string, options: SelectItem[]) => {
      const label = GetMessage(field);
      const help = getHelp(field);

      return (
        <>
          {getLine(
            `${label}:`,
            <Dropdown
              className="withpadding w-full text-left"
              onChange={(e) => setNewData({ ...newData, [field]: Number.parseInt(e.target.value) })}
              options={options}
              placeholder={label}
              value={getRecordString(field)}
            />,
            help
          )}
        </>
      );
    },
    [getLine, getRecordString, newData]
  );

  const onSave = useCallback(() => {
    if (!isSaveEnabled) {
      return;
    }

    UpdateSetting(newData)
      .then(() => {})
      .catch(() => {});
  }, [isSaveEnabled, newData]);

  const items: MenuItem[] = [
    {
      command: () => {
        onSave();
      },
      disabled: !isSaveEnabled,
      icon: <SaveIcon sx={{ fontSize: 40 }} />,
      label: 'Save'
    },
    {
      command: () => {
        setNewData({ ...originalData });
      },
      disabled: !isSaveEnabled,
      icon: <HistoryIcon sx={{ fontSize: 40 }} />,
      label: 'Undo'
    }
  ];

  return (
    <StandardHeader displayName={GetMessage('settings')} icon={<SettingsEditorIcon />}>
      <ScrollPanel style={{ height: 'calc(100vh - 58px)', width: '100%' }}>
        <Dock model={items} position="right" />
        <Fieldset className="mt-4 pt-10" legend={GetMessage('general')}>
          {getInputTextLine('deviceID')}
          {getCheckBoxLine('cleanURLs')}
          {getInputTextLine('ffmPegExecutable')}
          {getCheckBoxLine('enableSSL')}
          {newData.enableSSL === true && (
            <>
              {getInputTextLine('sslCertPath', GetMessage('changesServiceRestart'))}
              {getPasswordLine('sslCertPassword', GetMessage('changesServiceRestart'))}
            </>
          )}
          {getCheckBoxLine('overWriteM3UChannels')}
          {/* {getCheckBoxLine('logPerformance')} */}
        </Fieldset>

        <Fieldset className="mt-4 pt-10" legend={GetMessage('authentication')}>
          {getInputTextLine('apiKey')}
          {getDropDownLine('authenticationMethod', getAuthTypeOptions())}
          {getInputTextLine('adminUserName', adminUserNameError)}
          {getPasswordLine('adminPassword', adminPasswordError)}
          <div className="flex col-12">
            <div className="flex col-2 col-offset-1">
              <span>{GetMessage('signout')}</span>
            </div>
            <div className="flex col-3 m-0 p-0 debug">
              <Button
                disabled={!setting.authenticationType || (setting.authenticationType as number) === 0}
                icon="pi pi-check"
                label={GetMessage('signout')}
                onClick={() => (window.location.href = '/logout')}
                rounded
                severity="success"
                size="small"
              />
            </div>
          </div>
        </Fieldset>

        <Fieldset className="mt-4 pt-10" legend={GetMessage('streaming')}>
          {getDropDownLine('streamingProxyType', getHandlersOptions())}
          {getInputNumberLine('globalStreamLimit')}
          {getInputNumberLine('ringBufferSizeMB')}
          {getInputNumberLine('preloadPercentage', 999)}
          {/* {getInputNumberLine('maxConnectRetry', 999)}
            {getInputNumberLine('maxConnectRetryTimeMS', 999)} */}
          {getInputTextLine('clientUserAgent')}
          {getInputTextLine('streamingClientUserAgent')}
          {getInputTextLine('ffMpegOptions')}
          {getCheckBoxLine('showClientHostNames')}
        </Fieldset>

        <Fieldset className="mt-4 pt-10" legend={GetMessage('filesEPGM3U')}>
          {getCheckBoxLine('cacheIcons')}
          {getCheckBoxLine('videoStreamAlwaysUseEPGLogo')}
          {getInputTextLine('dummyRegex')}
          {getCheckBoxLine('sdEnabled')}
          {getInputTextLine('sdUserName')}
          {getPasswordLine('sdPassword')}
          <Fieldset className="mt-4 pt-10" collapsed legend={GetMessage('nameregexSettings')} toggleable>
            <SettingsNameRegexDataSelector data={settingsQuery.data?.nameRegex} />
          </Fieldset>
          <Fieldset className="mt-4 pt-10" collapsed legend={GetMessage('m3uSettings')} toggleable>
            {getCheckBoxLine('m3UIgnoreEmptyEPGID')}
            {getCheckBoxLine('m3UFieldCUID')}
            {getCheckBoxLine('m3UFieldChannelId')}
            {getCheckBoxLine('m3UFieldChannelNumber')}
            {getCheckBoxLine('m3UFieldTvgName')}
            {getCheckBoxLine('m3UFieldTvgChno')}
            {getCheckBoxLine('m3UFieldTvgId')}
            {getCheckBoxLine('m3UFieldTvgLogo')}
            {getCheckBoxLine('m3UFieldGroupTitle')}
          </Fieldset>
        </Fieldset>

        <Fieldset className="mt-4 pt-10" legend={GetMessage('development')}>
          <Button
            icon="pi pi-bookmark-fill"
            label="Swagger"
            onClick={() => {
              const link = `${baseHostURL}/swagger`;
              window.open(link);
            }}
            tooltip="Swagger Link"
            tooltipOptions={getTopToolOptions}
          />
        </Fieldset>
      </ScrollPanel>
    </StandardHeader>
  );
};

export default memo(SettingsEditor);
