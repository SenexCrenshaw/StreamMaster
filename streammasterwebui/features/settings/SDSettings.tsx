import { GetMessage } from '@lib/common/common';
import { SettingDto } from '@lib/iptvApi';
import { useSelectCurrentSettingDto } from '@lib/redux/slices/selectedCurrentSettingDto';
import { Fieldset } from 'primereact/fieldset';
import React from 'react';
import { getCheckBoxLine } from './getCheckBoxLine';
import { getInputNumberLine } from './getInputNumberLine';
import { getInputTextLine } from './getInputTextLine';
import { getPasswordLine } from './getPasswordLine';

export function SDSettings(): React.ReactElement {
  const { selectCurrentSettingDto, setSelectedCurrentSettingDto } = useSelectCurrentSettingDto('CurrentSettingDto');

  const onChange = (newValue: SettingDto) => {
    if (selectCurrentSettingDto === undefined || setSelectedCurrentSettingDto === undefined || newValue === null || newValue === undefined) return;
    setSelectedCurrentSettingDto(newValue);
  };

  return (
    <Fieldset className="mt-4 pt-10" legend={GetMessage('SD')}>
      {getCheckBoxLine({ field: 'sdSettings.sdEnabled', selectCurrentSettingDto, onChange })}
      {getInputTextLine({ field: 'sdSettings.sdUserName', selectCurrentSettingDto, onChange })}
      {getPasswordLine({ field: 'sdSettings.sdPassword', selectCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.seriesPosterArt', selectCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.seriesWsArt', selectCurrentSettingDto, onChange })}
      {getInputTextLine({ field: 'sdSettings.seriesPosterAspect', selectCurrentSettingDto, onChange })}
      {getInputTextLine({ field: 'sdSettings.artworkSize', selectCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.excludeCastAndCrew', selectCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.alternateSEFormat', selectCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.prefixEpisodeDescription', selectCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.prefixEpisodeTitle', selectCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.appendEpisodeDesc', selectCurrentSettingDto, onChange })}
      {getInputNumberLine({ field: 'sdSettings.sdepgDays', selectCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.seasonEventImages', selectCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.xmltvAddFillerData', selectCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.xmltvExtendedInfoInTitleDescriptions', selectCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.xmltvSingleImage', selectCurrentSettingDto, onChange })}
    </Fieldset>
  );
}
