import BaseButton from '@components/buttons/BaseButton';
import XButton from '@components/buttons/XButton';
import { OverlayPanel } from 'primereact/overlaypanel';
import React, { ReactNode, useRef } from 'react';
import { SMCard } from './SMCard';

interface SMOverlayProperties {
  readonly buttonTemplate?: ReactNode;
  readonly children: React.ReactNode;
  readonly header?: React.ReactNode;
  readonly buttonClassName?: string | undefined;
  readonly buttonLabel?: string | undefined;
  readonly icon?: string | undefined;
  readonly title: string | undefined;
  readonly tooltip?: string | undefined;
  readonly widthSize?: string;
  onHide?(): void;
}

export const SMOverlay = ({
  buttonTemplate,
  buttonClassName = '',
  buttonLabel = '',
  children,
  header,
  icon,
  onHide,
  tooltip = '',
  title = '',
  widthSize = '4'
}: SMOverlayProperties) => {
  const op = useRef<OverlayPanel>(null);

  const renderButton = () => {
    if (buttonTemplate) {
      return (
        <div className="">
          <BaseButton iconFilled={false} icon={icon} tooltip={tooltip} label={buttonLabel} onClick={(e) => op.current?.toggle(e)}>
            {buttonTemplate}
          </BaseButton>
        </div>
      );
    }
    return <BaseButton className={buttonClassName} iconFilled icon={icon} tooltip={tooltip} label={buttonLabel} onClick={(e) => op.current?.toggle(e)} />;
  };

  return (
    <>
      <OverlayPanel className={`sm-overlay w-${widthSize}`} ref={op} showCloseIcon={false} onHide={() => onHide && onHide()}>
        <SMCard
          title={title}
          header={
            <div className="justify-content-end flex-row flex gap-1">
              {header}
              <XButton iconFilled onClick={(e) => op.current?.toggle(e)} tooltip="Close" />
            </div>
          }
        >
          {children}
        </SMCard>
      </OverlayPanel>
      {renderButton()}
    </>
  );
};
