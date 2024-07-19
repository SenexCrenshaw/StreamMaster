import { GetMessage } from '@lib/common/intl';
import { Fieldset } from 'primereact/fieldset';
import { SelectItem } from 'primereact/selectitem';
import React, { useMemo } from 'react';
import { getInputNumberLine } from './components/getInputNumberLine';
import { getInputTextLine } from './components/getInputTextLine';
import { useSettingChangeHandler } from './hooks/useSettingChangeHandler';

import { BaseSettings } from './BaseSettings';
import { getCheckBoxLine } from './components/getCheckBoxLine';
import { GetDropDownLine } from './components/getDropDownLine';

export function StreamingSettings(): React.ReactElement {
  const { onChange, currentSettingRequest } = useSettingChangeHandler();
  // const { data: CommandProfiles } = useGetCommandProfiles();

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
        {GetDropDownLine({ currentSettingRequest, field: 'StreamingProxyType', onChange, options: getHandlersOptions })}
        {getInputNumberLine({ currentSettingRequest, field: 'GlobalStreamLimit', onChange })}
        {getInputTextLine({ currentSettingRequest, field: 'ClientUserAgent', onChange })}
        {getInputTextLine({ currentSettingRequest, field: 'StreamingClientUserAgent', onChange })}
        {/* {getInputTextLine({ currentSettingRequest, field: 'FFMpegOptions', onChange })} */}
        {getCheckBoxLine({ currentSettingRequest, field: 'ShowClientHostNames', onChange })}
      </>
    </BaseSettings>
  );
}
