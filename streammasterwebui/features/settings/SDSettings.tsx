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
  const { onChange, currentSettingRequest } = useSettingChangeHandler();

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

  if (currentSettingRequest === null || currentSettingRequest === undefined) {
    return (
      <Fieldset className="mt-4 pt-10" legend={GetMessage('SD')}>
        <div className="text-center">{GetMessage('loading')}</div>
      </Fieldset>
    );
  }

  return (
    <Fieldset className="mt-4 pt-10" legend={GetMessage('SD')} toggleable>
      {getCheckBoxLine({ field: 'sdSettings.sdEnabled', currentSettingRequest, onChange })}
      {getInputTextLine({ field: 'sdSettings.sdUserName', currentSettingRequest, onChange })}
      {getPasswordLine({ field: 'sdSettings.sdPassword', currentSettingRequest, onChange })}
      {getDropDownLine({ field: 'sdSettings.preferredLogoStyle', options: getLogoStyleOptions(), currentSettingRequest, onChange })}
      {getDropDownLine({ field: 'sdSettings.alternateLogoStyle', options: getLogoStyleOptions(), currentSettingRequest, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.seriesPosterArt', currentSettingRequest, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.seriesWsArt', currentSettingRequest, onChange })}
      {getDropDownLine({ field: 'sdSettings.seriesPosterAspect', options: getArtworkAspectOptions(), currentSettingRequest, onChange })}
      {getDropDownLine({ field: 'sdSettings.artworkSize', options: getArtworkSizeOptions(), currentSettingRequest, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.excludeCastAndCrew', currentSettingRequest, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.xmltvIncludeChannelNumbers', currentSettingRequest, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.alternateSEFormat', currentSettingRequest, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.prefixEpisodeDescription', currentSettingRequest, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.prefixEpisodeTitle', currentSettingRequest, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.appendEpisodeDesc', currentSettingRequest, onChange })}
      {getInputNumberLine({ field: 'sdSettings.sdepgDays', currentSettingRequest, onChange })}
      {getInputNumberLine({ field: 'sdSettings.xmltvFillerProgramLength', currentSettingRequest, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.seasonEventImages', currentSettingRequest, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.xmltvAddFillerData', currentSettingRequest, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.xmltvExtendedInfoInTitleDescriptions', currentSettingRequest, onChange })}
      {getCheckBoxLine({ field: 'sdSettings.xmltvSingleImage', currentSettingRequest, onChange })}
    </Fieldset>
  );
}
