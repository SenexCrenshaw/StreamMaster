import { SMTextDefaults } from '@components/sm/SMTextDefaults';
import { Logger } from '@lib/common/logger';
import { useMemo } from 'react';

type GetLineProps = {
  value: React.ReactElement;
  help?: string | null;
  defaultSetting?: string | null;
};

export function GetLine({ value, help, defaultSetting }: GetLineProps): React.ReactElement {
  Logger.debug('GetLine', { defaultSetting, help, value });

  const getWidths = useMemo(() => {
    const baseWidth = 12; // Total width for the component
    const valueWidth = 5; // Assuming value always takes a part of the width

    const helpWidth = help !== null && help !== undefined && help !== '' ? 5 : 0;
    const defaultSettingWidth = defaultSetting !== null && defaultSetting !== undefined && defaultSetting !== '' ? 2 : 0;

    // Adjusting widths based on the presence of help and defaultSetting
    const remainingWidth = baseWidth - valueWidth - helpWidth - defaultSettingWidth;

    return {
      defaultSetting: `w-${defaultSettingWidth}`,
      help: `w-${helpWidth}`,
      value: `w-${valueWidth + remainingWidth}`
    };
  }, [help, defaultSetting]);

  return (
    <div className="sm-w-12 flex align-items-center justify-content-start settings-line ">
      <div className={getWidths.value + ' pr-2'}>{value}</div>
      {help !== null && help !== undefined && help !== '' && <div className={getWidths.help}>{help}</div>}
      {defaultSetting !== null && defaultSetting !== undefined && defaultSetting !== '' && (
        <div className={getWidths.defaultSetting + ' flex justify-content-end pr-2 '}>
          {defaultSetting && <SMTextDefaults italicized text={defaultSetting} />}
        </div>
      )}
    </div>
  );
}
