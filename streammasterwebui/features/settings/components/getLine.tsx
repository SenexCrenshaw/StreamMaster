import { SMTextDefaults } from '@components/sm/SMTextDefaults';
import { Logger } from '@lib/common/logger';
import { useMemo } from 'react';

type GetLineProps = {
  value: React.ReactElement;
  help?: string | null;
  defaultSetting?: string | null;
};

export function GetLine({ value, help, defaultSetting }: GetLineProps): React.ReactElement {
  Logger.debug('GetLine', { defaultSetting, help });

  const getWidths = useMemo(() => {
    const baseWidth = 12; // Total width for the component
    const valueWidth = 4; // Base width for value

    let helpWidth = 0;
    let defaultSettingWidth = 0;
    let adjustedValueWidth = valueWidth;

    if (help !== null && help !== undefined && help !== '') {
      helpWidth = 4;
    }

    if (defaultSetting !== null && defaultSetting !== undefined && defaultSetting !== '') {
      defaultSettingWidth = 3;
    }

    // If help is set but defaultSetting is not, make value width bigger
    if (helpWidth > 0 && defaultSettingWidth === 0) {
      adjustedValueWidth = baseWidth - helpWidth;
    }

    // If defaultSetting is set but help is not, make value width bigger
    if (helpWidth === 0 && defaultSettingWidth > 0) {
      adjustedValueWidth = baseWidth - defaultSettingWidth;
    }

    // Calculate remaining width for value when both help and defaultSetting are set
    if (helpWidth > 0 && defaultSettingWidth > 0) {
      adjustedValueWidth = baseWidth - helpWidth - defaultSettingWidth;
    }

    if (helpWidth === 0 && defaultSettingWidth === 0) {
      adjustedValueWidth = baseWidth;
    }

    Logger.debug('GetLine', { adjustedValueWidth, defaultSettingWidth, helpWidth });

    return {
      defaultSetting: defaultSettingWidth > 0 ? `w-${defaultSettingWidth}` : '',
      help: helpWidth > 0 ? `w-${helpWidth}` : '',
      value: `w-${adjustedValueWidth}`
    };
  }, [help, defaultSetting]);

  return (
    <div className="sm-w-12 flex align-items-center justify-content-start settings-line ">
      <div className={getWidths.value + ' pr-2'}>{value}</div>
      {help !== null && help !== undefined && help !== '' && <div className={getWidths.help + ' sm-border-left pl-1'}>{help}</div>}
      {defaultSetting !== null && defaultSetting !== undefined && defaultSetting !== '' && (
        <div className={getWidths.defaultSetting + ' flex justify-content-end sm-border-left pr-2'}>
          {defaultSetting && <SMTextDefaults italicized text={defaultSetting} />}
        </div>
      )}
    </div>
  );
}
