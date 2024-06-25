import { SMTextColor } from '@components/sm/SMTextColor';
import { Logger } from '@lib/common/logger';

type GetLineProps = {
  value: React.ReactElement;
  help?: string | null;
  defaultSetting?: string | null;
};

export function getLine({ value, help, defaultSetting }: GetLineProps): React.ReactElement {
  Logger.debug('getLine', { defaultSetting, help, value });

  if (help !== null && help !== undefined) {
    return (
      <div className="flex w-full justify-items-center align-items-center align-content-center settings-line">
        <div className="w-6">{value}</div>

        {help !== null && help !== undefined && help !== '' && (
          <>
            <div className="flex pl-2 w-full text-sm align-content-center">
              {!defaultSetting && <>{help} </>}

              {defaultSetting && (
                <>
                  <div className="w-6">{help}</div>
                  <div className="w-6">
                    <span>: </span>
                    <SMTextColor italicized text={defaultSetting} />
                  </div>
                </>
              )}
            </div>
          </>
        )}
      </div>
    );
  }
  if (defaultSetting !== null && defaultSetting !== undefined && defaultSetting !== '') {
    return (
      <div className="flex w-full justify-items-center align-items-center align-content-center settings-line">
        <div className="w-8">{value}</div>
        <div className="w-4 pl-2">
          {defaultSetting && (
            <div>
              <span>: </span>
              <SMTextColor italicized text={defaultSetting} />
            </div>
          )}
        </div>
      </div>
    );
  }

  return <div className="settings-line">{value}</div>;
}
