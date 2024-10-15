import SettingsNameRegexDataSelector from '@components/settings/SettingsNameRegexDataSelector';
import { GetMessage } from '@lib/common/intl';
import { useSettingsContext } from '@lib/context/SettingsProvider';
import useGetSettings from '@lib/smAPI/Settings/useGetSettings';
import { Fieldset } from 'primereact/fieldset';
import React from 'react';
import { BaseSettings } from './BaseSettings';
import { GetCheckBoxLine } from './components/GetCheckBoxLine';
import { GetInputTextLine } from './components/GetInputTextLine';
import { GetDropDownLine } from './components/GetDropDownLine';
import { SelectItem } from 'primereact/selectitem';

export function MiscSettings(): React.ReactElement {
  const settingsQuery = useGetSettings();
  const { currentSetting } = useSettingsContext();
  if (currentSetting === null || currentSetting === undefined) {
    return (
      <Fieldset className="mt-4 pt-10" legend={GetMessage('SD')}>
        <div className="text-center">{GetMessage('loading')}</div>
      </Fieldset>
    );
  }

  // const getLogoStyleOptions = (): SelectItem[] => {
  //   var options = ['Dark', 'Gray', 'Light', 'White'];

  //   const test = options.map(
  //     (word) =>
  //       ({
  //         label: word,
  //         value: word.toLocaleLowerCase()
  //       } as SelectItem)
  //   );

  //   return test;
  // };

  const getDefaultCompressionOptions = (): SelectItem[] => {
    var options = ['None', 'GZ', 'ZIP'];

    const test = options.map(
      (word) =>
        ({
          label: word,
          value: word.toLocaleLowerCase()
        } as SelectItem)
    );

    return test;
  };

  const getLogoCacheOptions = (): SelectItem[] => {
    var options = ['None', 'Cache'];

    const test = options.map(
      (word) =>
        ({
          label: word,
          value: word.toLocaleLowerCase()
        } as SelectItem)
    );

    return test;
  };

  return (
    <BaseSettings title="MISC">
      <>
        {GetCheckBoxLine({ field: 'PrettyEPG' })}
        {GetCheckBoxLine({ field: 'AutoSetEPG' })}
        {GetDropDownLine({ field: 'LogoCache', options: getLogoCacheOptions() })}
        {/* {getCheckBoxLine({  field: 'VideoStreamAlwaysUseEPGLogo' })} */}
        {GetInputTextLine({ field: 'DummyRegex' })}
        {/* {GetCheckBoxLine({ field: 'M3UIgnoreEmptyEPGID' })} */}
        {GetDropDownLine({ field: 'DefaultCompression', options: getDefaultCompressionOptions() })}
        {/* {getCheckBoxLine({  field: 'm3UFieldGroupTitle' })} */}
        {/* {getCheckBoxLine({  field: 'm3UUseChnoForId' })} */}
        {/* {getCheckBoxLine({  field: 'm3UStationId' })} */}
        {/* {getCheckBoxLine({  field: 'm3UUseCUIDForChannelID' })} */}
        <Fieldset className="mt-4 pt-10" collapsed legend={GetMessage('nameregexSettings')} toggleable>
          <SettingsNameRegexDataSelector data={settingsQuery.data?.NameRegex} />
        </Fieldset>
      </>
    </BaseSettings>
  );
}
