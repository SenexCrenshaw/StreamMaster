import XButton from '@components/buttons/XButton';
import SMButton from '@components/sm/SMButton';
import { OverlayPanel } from 'primereact/overlaypanel';
import React, { ReactNode, useRef } from 'react';
import { SMCard } from './SMCard';

interface SMOverlayProperties {
  readonly buttonTemplate?: ReactNode;
  readonly buttonDarkBackground?: boolean;
  readonly children: React.ReactNode;
  readonly header?: React.ReactNode;
  readonly buttonClassName?: string | undefined;
  readonly buttonLabel?: string | undefined;
  readonly iconFilled?: boolean;
  readonly icon?: string | undefined;
  readonly title?: string | undefined;
  readonly simple?: boolean;
  readonly tooltip?: string | undefined;
  readonly widthSize?: string;
  onHide?(): void;
}

export const SMOverlay = ({
  buttonTemplate,
  buttonClassName = '',
  buttonLabel = '',
  buttonDarkBackground = false,
  iconFilled = false,
  children,
  header,
  icon,
  onHide,
  simple = false,
  tooltip = '',
  title,
  widthSize = '4'
}: SMOverlayProperties) => {
  const op = useRef<OverlayPanel>(null);

  const renderButton = () => {
    if (buttonTemplate) {
      return (
        <SMButton
          darkBackGround={buttonDarkBackground}
          className={buttonClassName}
          iconFilled={iconFilled}
          icon={icon}
          tooltip={tooltip}
          label={buttonLabel}
          onClick={(e) => op.current?.toggle(e)}
        >
          {buttonTemplate}
        </SMButton>
      );
    }
    return (
      <SMButton
        darkBackGround={buttonDarkBackground}
        className={buttonClassName}
        iconFilled={iconFilled}
        icon={icon}
        tooltip={tooltip}
        label={buttonLabel}
        onClick={(e) => op.current?.toggle(e)}
      />
    );
  };

  return (
    <>
      <OverlayPanel className={`sm-overlay w-${widthSize}`} ref={op} showCloseIcon={false} onHide={() => onHide && onHide()}>
        <SMCard
          simple={simple}
          title={title}
          header={
            <div className="justify-content-end align-items-center flex-row flex gap-1">
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
