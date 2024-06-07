import CloseButton from '@components/buttons/CloseButton';
import SMButton from '@components/sm/SMButton';
import { BlockUI } from 'primereact/blockui';
import { OverlayPanel } from 'primereact/overlaypanel';
import React, { ReactNode, SyntheticEvent, forwardRef, useCallback, useImperativeHandle, useMemo, useRef } from 'react';
import { SMCard } from './SMCard';

interface SMOverlayProperties {
  readonly answer?: boolean;
  readonly buttonClassName?: string | undefined;
  readonly buttonDarkBackground?: boolean;
  readonly buttonLabel?: string | undefined;
  readonly buttonTemplate?: ReactNode;
  readonly center?: React.ReactNode;
  readonly children: React.ReactNode;
  readonly header?: React.ReactNode;
  readonly icon?: string | undefined;
  readonly iconFilled?: boolean;
  readonly isLoading?: boolean;
  readonly showClose?: boolean;
  readonly simple?: boolean;
  readonly title?: string | undefined;
  readonly tooltip?: string | undefined;
  readonly contentWidthSize?: string;
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
    buttonLabel = '',
    buttonTemplate,
    center,
    children,
    header,
    icon,
    iconFilled = false,
    isLoading = false,
    onAnswered,
    onHide,
    onShow,
    showClose = false,
    simple = false,
    title,
    tooltip = '',
    contentWidthSize = '4'
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
  }, [buttonClassName, buttonDarkBackground, buttonLabel, buttonTemplate, icon, iconFilled, isLoading, openPanel, tooltip]);

  if (isLoading === true) {
    return (
      <BlockUI blocked={isLoading}>
        <div className="w-full">{renderButton}</div>
      </BlockUI>
    );
  }

  return (
    <>
      <OverlayPanel className={`sm-overlay sm-w-${contentWidthSize}`} ref={op} showCloseIcon={false} onShow={onShow} onHide={() => onHide && onHide()}>
        <SMCard
          center={center}
          header={
            <div className="justify-content-end align-items-center flex-row flex gap-1">
              {header}
              {showClose && <CloseButton onClick={(e) => openPanel(e)} tooltip="Close" />}
            </div>
          }
          simple={simple}
          title={title}
        >
          {children}
        </SMCard>
      </OverlayPanel>
      {renderButton}
    </>
  );
});

export default SMOverlay;
