import { SMCard } from '@components/sm/SMCard';
import { GetMessage } from '@lib/common/intl';
import { Fieldset } from 'primereact/fieldset';
import { SelectItem } from 'primereact/selectitem';
import React from 'react';
import { GetDropDownLine } from './components/getDropDownLine';

import { getInputNumberLine } from './components/getInputNumberLine';
import { getInputTextLine } from './components/getInputTextLine';
import { getPasswordLine } from './components/getPasswordLine';
import { useSettingChangeHandler } from './hooks/useSettingChangeHandler';
import { getCheckBoxLine } from './components/getCheckBoxLine';

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
    <SMCard info="" hasCloseButton title="SCHEDULES DIRECT">
      <div className="sm-card-children">
        <div className="sm-card-children-content">
          <div className="settings-lines">
            {getInputNumberLine({ currentSettingRequest, field: 'SDSettings.MaxSubscribedLineups', onChange })}
            {getCheckBoxLine({ currentSettingRequest, field: 'SDSettings.SDEnabled', onChange })}
            {getInputTextLine({ currentSettingRequest, field: 'SDSettings.SDUserName', onChange })}
            {getPasswordLine({ currentSettingRequest, field: 'SDSettings.SDPassword', onChange })}
            {GetDropDownLine({ currentSettingRequest, field: 'SDSettings.PreferredLogoStyle', onChange, options: getLogoStyleOptions() })}
            {GetDropDownLine({ currentSettingRequest, field: 'SDSettings.AlternateLogoStyle', onChange, options: getLogoStyleOptions() })}
            {getCheckBoxLine({ currentSettingRequest, field: 'SDSettings.SeriesPosterArt', onChange })}
            {getCheckBoxLine({ currentSettingRequest, field: 'SDSettings.SeriesWsArt', onChange })}
            {GetDropDownLine({ currentSettingRequest, field: 'SDSettings.SeriesPosterAspect', onChange, options: getArtworkAspectOptions() })}
            {GetDropDownLine({ currentSettingRequest, field: 'SDSettings.ArtworkSize', onChange, options: getArtworkSizeOptions() })}
            {getCheckBoxLine({ currentSettingRequest, field: 'SDSettings.ExcludeCastAndCrew', onChange })}
            {getCheckBoxLine({ currentSettingRequest, field: 'SDSettings.XmltvIncludeChannelNumbers', onChange })}
            {getCheckBoxLine({ currentSettingRequest, field: 'SDSettings.AlternateSEFormat', onChange })}
            {getCheckBoxLine({ currentSettingRequest, field: 'SDSettings.PrefixEpisodeDescription', onChange })}
            {getCheckBoxLine({ currentSettingRequest, field: 'SDSettings.PrefixEpisodeTitle', onChange })}
            {getCheckBoxLine({ currentSettingRequest, field: 'SDSettings.AppendEpisodeDesc', onChange })}
            {getInputNumberLine({ currentSettingRequest, field: 'SDSettings.SDepgDays', onChange })}
            {getInputNumberLine({ currentSettingRequest, field: 'SDSettings.XmltvFillerProgramLength', onChange })}
            {getCheckBoxLine({ currentSettingRequest, field: 'SDSettings.SeasonEventImages', onChange })}
            {getCheckBoxLine({ currentSettingRequest, field: 'SDSettings.XmltvAddFillerData', onChange })}
            {getCheckBoxLine({ currentSettingRequest, field: 'SDSettings.XmltvExtendedInfoInTitleDescriptions', onChange })}
            {getCheckBoxLine({ currentSettingRequest, field: 'SDSettings.XmltvSingleImage', onChange })}
          </div>
        </div>
        <div className="layout-padding-bottom" />
      </div>
    </SMCard>
  );
}
