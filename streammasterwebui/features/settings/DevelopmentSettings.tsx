import SyncCustomListsButton from '@components/customPlayList/SyncCustomListsButton';
import SMButton from '@components/sm/SMButton';
import { baseHostURL } from '@lib/settings';
import React from 'react';
import { BaseSettings } from './BaseSettings';

export function DevelopmentSettings(): React.ReactElement {
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
        </div>
      </div>
    </BaseSettings>
  );
}
