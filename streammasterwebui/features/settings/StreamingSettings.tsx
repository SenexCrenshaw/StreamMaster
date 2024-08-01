import { GetMessage } from '@lib/common/intl';
import { Fieldset } from 'primereact/fieldset';
import React, { useMemo } from 'react';
import { GetInputNumberLine } from './components/GetInputNumberLine';

import { useSettingsContext } from '@lib/context/SettingsProvider';
import { BaseSettings } from './BaseSettings';
import { GetCheckBoxLine } from './components/GetCheckBoxLine';
import { GetInputTextLine } from './components/GetInputTextLine';
import { GetDropDownLine } from './components/GetDropDownLine';
import { SelectItem } from 'primereact/selectitem';
import useGetCommandProfiles from '@lib/smAPI/Profiles/useGetCommandProfiles';
import useGetOutputProfiles from '@lib/smAPI/Profiles/useGetOutputProfiles';

export function StreamingSettings(): React.ReactElement {
  const { currentSetting } = useSettingsContext();
  const { data: commandProfiles } = useGetCommandProfiles();
  const { data: outputProfiles } = useGetOutputProfiles();

  const getIntroOptions = (): SelectItem[] => {
    var options = [
      { label: 'None', value: 0 },
      { label: 'Once', value: 1 },
      { label: 'Always', value: 2 }
    ] as SelectItem[];
    return options;
  };

  const DefaultCommandProfileNameOptions = useMemo((): SelectItem[] => {
    if (!commandProfiles) {
      return [];
    }

    const ret = commandProfiles.map(
      (x) =>
        ({
          label: x.ProfileName,
          value: x.ProfileName
        } as SelectItem)
    );
    return ret;
  }, [commandProfiles]);

  const DefaultOutputProfileNameOptions = useMemo((): SelectItem[] => {
    if (!outputProfiles) {
      return [];
    }

    const ret = outputProfiles.map(
      (x) =>
        ({
          label: x.ProfileName,
          value: x.ProfileName
        } as SelectItem)
    );
    return ret;
  }, [outputProfiles]);

  if (!currentSetting) {
    return (
      <Fieldset className="mt-4 pt-10" legend={GetMessage('SD')}>
        <div className="text-center">{GetMessage('loading')}</div>
      </Fieldset>
    );
  }

  return (
    <BaseSettings title="STREAMING">
      {GetInputNumberLine({ field: 'GlobalStreamLimit' })}
      {GetInputTextLine({ field: 'StreamingClientUserAgent' })}
      {GetCheckBoxLine({ field: 'ShowClientHostNames' })}
      {GetDropDownLine({ field: 'ShowIntros', options: getIntroOptions() })}
      {GetDropDownLine({ field: 'DefaultCommandProfileName', options: DefaultCommandProfileNameOptions })}
      {GetDropDownLine({ field: 'DefaultOutputProfileName', options: DefaultOutputProfileNameOptions })}
    </BaseSettings>
  );
}
