import { GetMessage } from '@lib/common/intl';
import { Fieldset } from 'primereact/fieldset';
import { SelectItem } from 'primereact/selectitem';
import React, { useMemo } from 'react';
import { GetDropDownLine } from './components/GetDropDownLine';
import { getCheckBoxLine } from './components/getCheckBoxLine';
import { getInputNumberLine } from './components/getInputNumberLine';
import { getInputTextLine } from './components/getInputTextLine';
import { useSettingChangeHandler } from './hooks/useSettingChangeHandler';

import { SMCard } from '@components/sm/SMCard';
import useGetVideoProfiles from '@lib/smAPI/Profiles/useGetVideoProfiles';

export function StreamingSettings(): React.ReactElement {
  const { onChange, currentSettingRequest } = useSettingChangeHandler();
  const { data: videoProfiles } = useGetVideoProfiles();

  const getHandlersOptions = useMemo((): SelectItem[] => {
    const DefaultStreamingProxyTypes = ['SystemDefault', 'None', 'StreamMaster'];

    const options = DefaultStreamingProxyTypes.map((type) => ({
      label: type,
      value: type
    }));

    if (videoProfiles) {
      videoProfiles.forEach((profile) => {
        options.push({
          label: profile.ProfileName,
          value: profile.ProfileName
        });
      });
    }
    return options;
  }, [videoProfiles]);

  if (!currentSettingRequest) {
    return (
      <Fieldset className="mt-4 pt-10" legend={GetMessage('SD')}>
        <div className="text-center">{GetMessage('loading')}</div>
      </Fieldset>
    );
  }

  return (
    <SMCard hasCloseButton darkBackGround={false} title="STREAMING" header={<div className="justify-content-end align-items-center flex-row flex gap-1"></div>}>
      <div className="sm-card-children">
        <div className="sm-card-children-content">
          <div className="layout-padding-bottom" />
          <div className="settings-lines">
            {GetDropDownLine({ currentSettingRequest, field: 'StreamingProxyType', onChange, options: getHandlersOptions })}
            {getInputNumberLine({ currentSettingRequest, field: 'GlobalStreamLimit', onChange })}
            {getInputTextLine({ currentSettingRequest, field: 'ClientUserAgent', onChange })}
            {getInputTextLine({ currentSettingRequest, field: 'StreamingClientUserAgent', onChange })}
            {/* {getInputTextLine({ currentSettingRequest, field: 'FFMpegOptions', onChange })} */}
            {getCheckBoxLine({ currentSettingRequest, field: 'ShowClientHostNames', onChange })}
          </div>
        </div>
        <div className="layout-padding-bottom" />
      </div>
    </SMCard>
  );
}
