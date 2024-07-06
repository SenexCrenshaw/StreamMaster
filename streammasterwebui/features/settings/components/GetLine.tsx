import { SMCard } from '@components/sm/SMCard';
import SMPopUp from '@components/sm/SMPopUp';
import { Logger } from '@lib/common/logger';
import { useMemo } from 'react';

type GetLineProps = {
  value: React.ReactElement;
  help?: string | null;
  defaultSetting?: string | null;
};

export function GetLine({ value, help, defaultSetting }: GetLineProps): React.ReactElement {
  const getInfo = useMemo(() => {
    if (help || defaultSetting) {
      return (
        <SMCard>
          <div>{help}</div>
          {defaultSetting && (
            <>
              <hr />
              <div>Default: {defaultSetting}</div>
            </>
          )}
        </SMCard>
      );
    }
    return null;
  }, [defaultSetting, help]);

  if (getInfo === null) {
    return <div className="settings-line">{value}</div>;
  }

  return (
    <div className="settings-line w-full flex w-full justify-content-between">
      <div className="sm-w-11">{value}</div>

      <SMPopUp buttonClassName="icon-orange" icon="pi-question-circle">
        {getInfo}
      </SMPopUp>
    </div>
  );
}
