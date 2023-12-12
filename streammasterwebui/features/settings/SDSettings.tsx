import { GetMessage } from '@lib/common/common';
import { Fieldset } from 'primereact/fieldset';
import React from 'react';
import { getCheckBoxLine } from './getCheckBoxLine';
import { getInputNumberLine } from './getInputNumberLine';
import { getInputTextLine } from './getInputTextLine';
import { getPasswordLine } from './getPasswordLine';
import { useSettingChangeHandler } from './useSettingChangeHandler';

export function SDSettings(): React.ReactElement {
  const { onChange, selectedCurrentSettingDto } = useSettingChangeHandler();

  if (selectedCurrentSettingDto === null || selectedCurrentSettingDto === undefined) {
    return (
      <Fieldset className="mt-4 pt-10" legend={GetMessage('SD')}>
        <div className="text-center">{GetMessage('loading')}</div>
      </Fieldset>
    );
  }

  return (
    <Fieldset className="mt-4 pt-10" legend={GetMessage('SD')} toggleable>
      {getCheckBoxLine({ field: 'sdSettings.sdEnabled', selectedCurrentSettingDto, onChange })}
      {getInputTextLine({ field: 'sdSettings.sdUserName', selectedCurrentSettingDto, onChange })}
      {getPasswordLine({ field: 'sdSettings.sdPassword', selectedCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.seriesPosterArt', selectedCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.seriesWsArt', selectedCurrentSettingDto, onChange })}
      {getInputTextLine({ field: 'sdSettings.seriesPosterAspect', selectedCurrentSettingDto, onChange })}
      {getInputTextLine({ field: 'sdSettings.artworkSize', selectedCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.excludeCastAndCrew', selectedCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.alternateSEFormat', selectedCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.prefixEpisodeDescription', selectedCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.prefixEpisodeTitle', selectedCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.appendEpisodeDesc', selectedCurrentSettingDto, onChange })}
      {getInputNumberLine({ field: 'sdSettings.sdepgDays', selectedCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.seasonEventImages', selectedCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.xmltvAddFillerData', selectedCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.xmltvExtendedInfoInTitleDescriptions', selectedCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.xmltvSingleImage', selectedCurrentSettingDto, onChange })}
    </Fieldset>
  );
}
