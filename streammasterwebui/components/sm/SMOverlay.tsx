import SMButton from '@components/sm/SMButton';
import { OverlayPanel } from 'primereact/overlaypanel';
import React, { ReactNode, useMemo, useRef } from 'react';
import { SMCard } from './SMCard';
import CloseButton from '@components/buttons/CloseButton';

interface SMOverlayProperties {
  readonly buttonTemplate?: ReactNode;
  readonly buttonDarkBackground?: boolean;
  readonly children: React.ReactNode;
  readonly header?: React.ReactNode;
  readonly buttonClassName?: string | undefined;
  readonly buttonFlex?: boolean | undefined;
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
  buttonFlex = false,
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

  const renderButton = useMemo(() => {
    const flex = buttonFlex ? 'flex align-items-center justify-content-center' : '';
    if (buttonTemplate) {
      return (
        <div className={flex}>
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
        </div>
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
  }, [buttonClassName, buttonDarkBackground, buttonFlex, buttonLabel, buttonTemplate, icon, iconFilled, tooltip]);

  return (
    <>
      <OverlayPanel className={`sm-overlay w-${widthSize}`} ref={op} showCloseIcon={false} onHide={() => onHide && onHide()}>
        <SMCard
          simple={simple}
          title={title}
          header={
            <div className="justify-content-end align-items-center flex-row flex gap-1">
              {header}
              <CloseButton onClick={(e) => op.current?.toggle(e)} tooltip="Close" />
            </div>
          }
        >
          {children}
        </SMCard>
      </OverlayPanel>
      {renderButton}
    </>
  );
};
