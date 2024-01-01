import { GetMessage } from '@lib/common/common';
import { Fieldset } from 'primereact/fieldset';
import { SelectItem } from 'primereact/selectitem';
import React from 'react';
import { getCheckBoxLine } from './getCheckBoxLine';
import { getDropDownLine } from './getDropDownLine';
import { getInputNumberLine } from './getInputNumberLine';
import { getInputTextLine } from './getInputTextLine';
import { getPasswordLine } from './getPasswordLine';
import { useSettingChangeHandler } from './useSettingChangeHandler';

export function SDSettings(): React.ReactElement {
  const { onChange, selectedCurrentSettingDto } = useSettingChangeHandler();

  const getLogoStyleOptions = (): SelectItem[] => {
    var options = ['Dark', 'Gray', 'Light', 'White'];

    const test = options.map(
      (word) =>
        ({
          label: word,
          value: word.toLocaleLowerCase()
        } as SelectItem)
    );

    return test;
  };

  const getArtworkSizeOptions = (): SelectItem[] => {
    var options = ['Sm', 'Md', 'Lg'];

    const test = options.map(
      (word) =>
        ({
          label: word,
          value: word.toLocaleLowerCase()
        } as SelectItem)
    );

    return test;
  };

  const getArtworkAspectOptions = (): SelectItem[] => {
    var options = ['2x3', '4x3', '16x9'];

    const test = options.map(
      (word) =>
        ({
          label: word,
          value: word.toLocaleLowerCase()
        } as SelectItem)
    );

    return test;
  };

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
      {getDropDownLine({ field: 'sdSettings.preferredLogoStyle', options: getLogoStyleOptions(), selectedCurrentSettingDto, onChange })}
      {getDropDownLine({ field: 'sdSettings.alternateLogoStyle', options: getLogoStyleOptions(), selectedCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.seriesPosterArt', selectedCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.seriesWsArt', selectedCurrentSettingDto, onChange })}
      {getDropDownLine({ field: 'sdSettings.seriesPosterAspect', options: getArtworkAspectOptions(), selectedCurrentSettingDto, onChange })}
      {getDropDownLine({ field: 'sdSettings.artworkSize', options: getArtworkSizeOptions(), selectedCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.excludeCastAndCrew', selectedCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.xmltvIncludeChannelNumbers', selectedCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.alternateSEFormat', selectedCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.prefixEpisodeDescription', selectedCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.prefixEpisodeTitle', selectedCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.appendEpisodeDesc', selectedCurrentSettingDto, onChange })}
      {getInputNumberLine({ field: 'sdSettings.sdepgDays', selectedCurrentSettingDto, onChange })}
      {getInputNumberLine({ field: 'sdSettings.xmltvFillerProgramLength', selectedCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.seasonEventImages', selectedCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.xmltvAddFillerData', selectedCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.xmltvExtendedInfoInTitleDescriptions', selectedCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.xmltvSingleImage', selectedCurrentSettingDto, onChange })}
    </Fieldset>
  );
}
