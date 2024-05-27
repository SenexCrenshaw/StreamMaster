import SMButton from '@components/sm/SMButton';
import { OverlayPanel } from 'primereact/overlaypanel';
import React, { ReactNode, SyntheticEvent, forwardRef, useCallback, useImperativeHandle, useMemo, useRef } from 'react';
import { SMCard } from './SMCard';
import CloseButton from '@components/buttons/CloseButton';

interface SMOverlayProperties {
  readonly answer?: boolean;
  readonly buttonClassName?: string | undefined;
  readonly buttonDarkBackground?: boolean;
  readonly buttonFlex?: boolean | undefined;
  readonly buttonLabel?: string | undefined;
  readonly buttonTemplate?: ReactNode;
  readonly children: React.ReactNode;
  readonly header?: React.ReactNode;
  readonly icon?: string | undefined;
  readonly iconFilled?: boolean;
  readonly isLoading?: boolean;
  readonly simple?: boolean;
  readonly title?: string | undefined;
  readonly tooltip?: string | undefined;
  readonly widthSize?: string;
  onAnswered?(): void;
  onHide?(): void;
  onShow?(): void;
}

export interface SMOverlayRef {
  hide: () => void;
  show: (event: any) => void;
}

const SMOverlay = forwardRef<SMOverlayRef, SMOverlayProperties>((props: SMOverlayProperties, ref) => {
  const {
    answer,
    buttonClassName = '',
    buttonDarkBackground = false,
    buttonFlex = false,
    buttonLabel = '',
    buttonTemplate,
    children,
    header,
    icon,
    iconFilled = false,
    isLoading = false,
    onAnswered,
    onHide,
    onShow,
    simple = false,
    title,
    tooltip = '',
    widthSize = '4'
  } = props;

  useImperativeHandle(ref, () => ({
    hide: () => op.current?.hide(),
    props,
    show: (event: any) => op.current?.show(null, event)
  }));

  const op = useRef<OverlayPanel>(null);

  const openPanel = useCallback(
    (e: SyntheticEvent) => {
      if (answer !== undefined) {
        onAnswered && onAnswered();
        return;
      }
      op.current?.toggle(e);
    },
    [answer, onAnswered]
  );

  const renderButton = useMemo(() => {
    if (buttonTemplate) {
      if (buttonFlex) {
        return (
          <div className="flex align-items-center justify-content-center">
            <SMButton
              darkBackGround={buttonDarkBackground}
              className={buttonClassName}
              iconFilled={iconFilled}
              icon={icon}
              isLoading={isLoading}
              tooltip={tooltip}
              label={buttonLabel}
              onClick={(e) => openPanel(e)}
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
          isLoading={isLoading}
          tooltip={tooltip}
          label={buttonLabel}
          onClick={(e) => openPanel(e)}
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
        isLoading={isLoading}
        tooltip={tooltip}
        label={buttonLabel}
        onClick={(e) => openPanel(e)}
      />
    );
  }, [buttonClassName, buttonDarkBackground, buttonFlex, buttonLabel, buttonTemplate, icon, iconFilled, isLoading, openPanel, tooltip]);

  if (isLoading === true) {
    return <div className="w-full">{renderButton}</div>;
  }

  return (
    <>
      <OverlayPanel className={`sm-overlay w-${widthSize}`} ref={op} showCloseIcon={false} onShow={onShow} onHide={() => onHide && onHide()}>
        <SMCard
          simple={simple}
          title={title}
          header={
            <div className="justify-content-end align-items-center flex-row flex gap-1">
              {header}
              <CloseButton onClick={(e) => openPanel(e)} tooltip="Close" />
            </div>
          }
        >
          {children}
        </SMCard>
      </OverlayPanel>
      {renderButton}
    </>
  );
});

export default SMOverlay;
