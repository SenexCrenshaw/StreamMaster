import SMButton from '@components/sm/SMButton';
import { SMCard } from '@components/sm/SMCard';
import { baseHostURL } from '@lib/settings';
import React from 'react';

export function DevelopmentSettings(): React.ReactElement {
  return (
    <SMCard info="" hasCloseButton darkBackGround={false} title={'DEVELOPER TOOLS'}>
      <div className="sm-card-children w-full">
        <div className="layout-padding-bottom" />
        <div className="sm-card-children-content flex justify-content-center align-items-center">
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
        </div>
        <div className="layout-padding-bottom" />
      </div>
    </SMCard>
  );
}
