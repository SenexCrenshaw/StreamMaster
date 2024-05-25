import { GetMessage } from '@lib/common/intl';
import { Fieldset } from 'primereact/fieldset';
import { SelectItem } from 'primereact/selectitem';
import React from 'react';
import { getCheckBoxLine } from './components/getCheckBoxLine';
import { getDropDownLine } from './components/getDropDownLine';
import { getInputNumberLine } from './components/getInputNumberLine';
import { getInputTextLine } from './components/getInputTextLine';
import { getPasswordLine } from './components/getPasswordLine';
import { useSettingChangeHandler } from './useSettingChangeHandler';
import { SMCard } from '@components/sm/SMCard';

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
    <SMCard
      darkBackGround={false}
      title="SCHEDULES DIRECT"
      header={<div className="justify-content-end align-items-center flex-row flex gap-1">{/* {header}                */}</div>}
    >
      <div className="sm-card-children">
        <div className="sm-card-children-content">
          <div className="layout-padding-bottom" />
          <div className="settings-lines ">
            {getCheckBoxLine({ currentSettingRequest, field: 'sdSettings.sdEnabled', onChange })}
            {getInputTextLine({ currentSettingRequest, field: 'sdSettings.sdUserName', onChange })}
            {getPasswordLine({ currentSettingRequest, field: 'sdSettings.sdPassword', onChange })}
            {getDropDownLine({ currentSettingRequest, field: 'sdSettings.preferredLogoStyle', onChange, options: getLogoStyleOptions() })}
            {getDropDownLine({ currentSettingRequest, field: 'sdSettings.alternateLogoStyle', onChange, options: getLogoStyleOptions() })}
            {getCheckBoxLine({ currentSettingRequest, field: 'sdSettings.seriesPosterArt', onChange })}
            {getCheckBoxLine({ currentSettingRequest, field: 'sdSettings.seriesWsArt', onChange })}
            {getDropDownLine({ currentSettingRequest, field: 'sdSettings.seriesPosterAspect', onChange, options: getArtworkAspectOptions() })}
            {getDropDownLine({ currentSettingRequest, field: 'sdSettings.artworkSize', onChange, options: getArtworkSizeOptions() })}
            {getCheckBoxLine({ currentSettingRequest, field: 'sdSettings.excludeCastAndCrew', onChange })}
            {getCheckBoxLine({ currentSettingRequest, field: 'sdSettings.xmltvIncludeChannelNumbers', onChange })}
            {getCheckBoxLine({ currentSettingRequest, field: 'sdSettings.alternateSEFormat', onChange })}
            {getCheckBoxLine({ currentSettingRequest, field: 'sdSettings.prefixEpisodeDescription', onChange })}
            {getCheckBoxLine({ currentSettingRequest, field: 'sdSettings.prefixEpisodeTitle', onChange })}
            {getCheckBoxLine({ currentSettingRequest, field: 'sdSettings.appendEpisodeDesc', onChange })}
            {getInputNumberLine({ currentSettingRequest, field: 'sdSettings.sdepgDays', onChange })}
            {getInputNumberLine({ currentSettingRequest, field: 'sdSettings.xmltvFillerProgramLength', onChange })}
            {getCheckBoxLine({ currentSettingRequest, field: 'sdSettings.seasonEventImages', onChange })}
            {getCheckBoxLine({ currentSettingRequest, field: 'sdSettings.xmltvAddFillerData', onChange })}
            {getCheckBoxLine({ currentSettingRequest, field: 'sdSettings.xmltvExtendedInfoInTitleDescriptions', onChange })}
            {getCheckBoxLine({ currentSettingRequest, field: 'sdSettings.xmltvSingleImage', onChange })}
          </div>
        </div>
        <div className="layout-padding-bottom" />
      </div>
    </SMCard>
  );
}
