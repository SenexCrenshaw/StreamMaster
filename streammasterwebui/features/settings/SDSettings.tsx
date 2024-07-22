import { GetMessage } from '@lib/common/intl';
import { useSettingsContext } from '@lib/context/SettingsProvider';
import { Fieldset } from 'primereact/fieldset';
import { SelectItem } from 'primereact/selectitem';
import React from 'react';
import { BaseSettings } from './BaseSettings';
import { GetCheckBoxLine } from './components/GetCheckBoxLine';
import { GetDropDownLine } from './components/GetDropDownLine';
import { GetInputNumberLine } from './components/GetInputNumberLine';
import { GetInputTextLine } from './components/GetInputTextLine';
import { GetPasswordLine } from './components/GetPasswordLine';

export function SDSettings(): React.ReactElement {
  const { currentSetting } = useSettingsContext();

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

  if (currentSetting === null || currentSetting === undefined) {
    return (
      <Fieldset className="mt-4 pt-10" legend={GetMessage('SD')}>
        <div className="text-center">{GetMessage('loading')}</div>
      </Fieldset>
    );
  }

  return (
    <BaseSettings title="SCHEDULES DIRECT">
      <>
        {GetInputNumberLine({ field: 'SDSettings.MaxSubscribedLineups' })}
        {GetCheckBoxLine({ field: 'SDSettings.SDEnabled' })}
        {GetInputTextLine({ field: 'SDSettings.SDUserName' })}
        {GetPasswordLine({ field: 'SDSettings.SDPassword' })}
        {GetDropDownLine({ field: 'SDSettings.PreferredLogoStyle', options: getLogoStyleOptions() })}
        {GetDropDownLine({ field: 'SDSettings.AlternateLogoStyle', options: getLogoStyleOptions() })}
        {GetCheckBoxLine({ field: 'SDSettings.SeriesPosterArt' })}
        {GetCheckBoxLine({ field: 'SDSettings.SeriesWsArt' })}
        {GetDropDownLine({ field: 'SDSettings.SeriesPosterAspect', options: getArtworkAspectOptions() })}
        {GetDropDownLine({ field: 'SDSettings.ArtworkSize', options: getArtworkSizeOptions() })}
        {GetCheckBoxLine({ field: 'SDSettings.ExcludeCastAndCrew' })}
        {GetCheckBoxLine({ field: 'SDSettings.XmltvIncludeChannelNumbers' })}
        {GetCheckBoxLine({ field: 'SDSettings.AlternateSEFormat' })}
        {GetCheckBoxLine({ field: 'SDSettings.PrefixEpisodeDescription' })}
        {GetCheckBoxLine({ field: 'SDSettings.PrefixEpisodeTitle' })}
        {GetCheckBoxLine({ field: 'SDSettings.AppendEpisodeDesc' })}
        {GetInputNumberLine({ field: 'SDSettings.SDepgDays' })}
        {GetInputNumberLine({ field: 'SDSettings.XmltvFillerProgramLength' })}
        {GetCheckBoxLine({ field: 'SDSettings.SeasonEventImages' })}
        {GetCheckBoxLine({ field: 'SDSettings.XmltvAddFillerData' })}
        {GetCheckBoxLine({ field: 'SDSettings.XmltvExtendedInfoInTitleDescriptions' })}
        {GetCheckBoxLine({ field: 'SDSettings.XmltvSingleImage' })}
      </>
    </BaseSettings>
  );
}
