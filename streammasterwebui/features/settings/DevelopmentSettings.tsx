import SyncCustomListsButton from '@components/customPlayList/SyncCustomListsButton';
import SMButton from '@components/sm/SMButton';
import { baseHostURL } from '@lib/settings';
import React from 'react';
import { BaseSettings } from './BaseSettings';
import { useSMContext } from '@lib/context/SMProvider';
import BooleanEditor from '@components/inputs/BooleanEditor';

export function DevelopmentSettings(): React.ReactElement {
  const { enableFetchDebug, fetchDebug } = useSMContext();

  return (
    <BaseSettings title="DEVELOPMENT">
      <div className="sm-center-stuff">
        <div className="sm-w-7rem">
          <SMButton
            icon="pi-bookmark-fill"
            iconFilled
            buttonClassName="icon-blue"
            label="Swagger"
            onClick={() => {
              const link = `${baseHostURL}/swagger`;
              window.open(link);
            }}
            tooltip="Swagger Link"
          />
        </div>
        <div className="sm-w-9rem">
          <SyncCustomListsButton />
          <BooleanEditor checked={fetchDebug} onChange={enableFetchDebug} label="Fetch Debug" />
        </div>
      </div>
    </BaseSettings>
  );
}
