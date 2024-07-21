import { GetMessage } from '@lib/common/intl';
import { Fieldset } from 'primereact/fieldset';
import { SelectItem } from 'primereact/selectitem';
import React, { useMemo } from 'react';
import { GetInputNumberLine } from './components/GetInputNumberLine';

import { useSettingsContext } from '@lib/context/SettingsProvider';
import { BaseSettings } from './BaseSettings';
import { GetCheckBoxLine } from './components/GetCheckBoxLine';
import { GetDropDownLine } from './components/GetDropDownLine';
import { GetInputTextLine } from './components/GetInputTextLine';

export function StreamingSettings(): React.ReactElement {
  const { currentSettingRequest } = useSettingsContext();

  const getHandlersOptions = useMemo((): SelectItem[] => {
    const DefaultStreamingProxyTypes = ['SystemDefault', 'None', 'StreamMaster'];

    const options = DefaultStreamingProxyTypes.map((type) => ({
      label: type,
      value: type
    }));

    // if (CommandProfiles) {
    //   CommandProfiles.forEach((profile) => {
    //     options.push({
    //       label: profile.ProfileName,
    //       value: profile.ProfileName
    //     });
    //   });
    // }
    return options;
  }, []);

  if (!currentSettingRequest) {
    return (
      <Fieldset className="mt-4 pt-10" legend={GetMessage('SD')}>
        <div className="text-center">{GetMessage('loading')}</div>
      </Fieldset>
    );
  }

  return (
    <BaseSettings title="STREAMING">
      <>
        {GetDropDownLine({ field: 'StreamingProxyType', options: getHandlersOptions })}
        {GetInputNumberLine({ field: 'GlobalStreamLimit' })}
        {GetInputTextLine({ field: 'ClientUserAgent' })}
        {GetInputTextLine({ field: 'StreamingClientUserAgent' })}
        {/* {getInputTextLine({  field: 'FFMpegOptions' })} */}
        {GetCheckBoxLine({ field: 'ShowClientHostNames' })}
      </>
    </BaseSettings>
  );
}
