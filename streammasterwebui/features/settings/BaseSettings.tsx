import { SMCard } from '@components/sm/SMCard';
import React from 'react';

interface BaseSettingsProps {
  readonly children: React.ReactNode;
  readonly title: string;
}

export function BaseSettings({ children, title }: BaseSettingsProps): React.ReactElement {
  return (
    <SMCard
      info=""
      noCloseButton
      darkBackGround={false}
      noBorderChildren
      title={title}
      noFullScreen
      // header={<div className="justify-content-end align-items-center flex-row flex gap-1">{/* {header}                */}</div>}
    >
      <div className="sm-card-children-noborder sm-headerBg">
        <div className="sm-card-children-content">
          <div className="settings-lines">
            <div className="layout-padding-bottom" />
            {children}
            <div className="layout-padding-bottom" />
          </div>
        </div>
      </div>
    </SMCard>
  );
}
