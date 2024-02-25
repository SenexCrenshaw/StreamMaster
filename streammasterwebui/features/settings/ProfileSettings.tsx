import { GetMessage } from '@lib/common/common';
import React, { useMemo } from 'react';
// Import the getLine function
import { Fieldset } from 'primereact/fieldset';
import { getProfileLine } from './getProfileLine';
import { useSettingChangeHandler } from './useSettingChangeHandler';

export function ProfileSettings(): React.ReactElement {
  const { onChange, selectedCurrentSettingDto } = useSettingChangeHandler();

  const getLines = useMemo(() => {
    if (selectedCurrentSettingDto === null || selectedCurrentSettingDto === undefined) {
      return <div className="text-center">{GetMessage('loading')}</div>;
    }

    if (selectedCurrentSettingDto.ffmpegProfiles === null || selectedCurrentSettingDto.ffmpegProfiles === undefined) {
      return <div className="text-center">{GetMessage('loading')}</div>;
    }

    const toRet = Object.keys(selectedCurrentSettingDto.ffmpegProfiles).map((key) => {
      console.log('key', key);
      return getProfileLine({ field: 'ffmpegProfiles.' + key, selectedCurrentSettingDto, onChange });
    });

    return toRet;
  }, [onChange, selectedCurrentSettingDto]);

  if (selectedCurrentSettingDto === null || selectedCurrentSettingDto === undefined) {
    return (
      <Fieldset className="mt-4 pt-10" legend={GetMessage('SD')}>
        {getLines}
      </Fieldset>
    );
  }

  return (
    <Fieldset className="mt-4 pt-10" legend={GetMessage('profiles')} toggleable>
      {getLines}
    </Fieldset>
  );
}
