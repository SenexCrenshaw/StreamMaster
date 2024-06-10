import CloseButton from '@components/buttons/CloseButton';
import {
  FloatingArrow,
  FloatingFocusManager,
  FloatingOverlay,
  FloatingPortal,
  arrow,
  autoPlacement,
  autoUpdate,
  flip,
  offset,
  shift,
  useClick,
  useDismiss,
  useFloating,
  useId,
  useInteractions,
  useRole,
  useTransitionStyles
} from '@floating-ui/react';
import { Logger } from '@lib/common/logger';
import { BlockUI } from 'primereact/blockui';
import { CSSProperties, ReactNode, SyntheticEvent, forwardRef, useCallback, useImperativeHandle, useMemo, useRef, useState } from 'react';
import { CombinedProvider } from './Context/CombinedContext';
import { useSMButton } from './Context/useSMButton';
import { SMOverlayProperties } from './Interfaces/SMOverlayProperties';
import { SMCard } from './SMCard';

export interface SMOverlayRef {
  hide: () => void;
  show: (event: any) => void;
}

interface ExtendedSMOverlayProperties extends SMOverlayProperties {
  readonly children?: ReactNode;
}

const SMOverlayInner = forwardRef<SMOverlayRef, ExtendedSMOverlayProperties>(
  (
    {
      buttonDisabled = false,
      closeOnLostFocus: closeOnFocusOut = false,
      placement = 'bottom',
      contentWidthSize = '4',
      answer,
      children,
      header,
      info,
      isLoading = false,
      onAnswered,
      showClose = false,
      autoPlacement: innerAutoPlacement = false,
      ...props
    },
    ref
  ) => {
    const [isOpen, setIsOpen] = useState(false);
    const arrowRef = useRef(null);
    if (props.title === 'Add M3U') {
      Logger.debug('SMOverlay', props.title, { modal: props.modal, modalCentered: props.modalCentered });
    }
    const ARROW_HEIGHT = 12;
    const ARROW_HEIGHT_OFFSET = ARROW_HEIGHT - 4;

    const middleware = useMemo(() => {
      if (props.modalCentered) {
        return [
          offset(({ rects }) => {
            return -rects.reference.height / 2 - rects.floating.height / 2;
          }),
          arrow({ element: arrowRef })
        ];
      }
      return innerAutoPlacement
        ? [offset(() => ARROW_HEIGHT_OFFSET, [ARROW_HEIGHT_OFFSET]), autoPlacement(), arrow({ element: arrowRef })]
        : [offset(() => ARROW_HEIGHT_OFFSET, [ARROW_HEIGHT_OFFSET]), shift(), flip(), arrow({ element: arrowRef })];
    }, [ARROW_HEIGHT_OFFSET, innerAutoPlacement, props.modalCentered]);

    const { refs, floatingStyles, context } = useFloating({
      middleware,
      onOpenChange(nextOpen, event, reason) {
        Logger.debug('onOpenChange', { closeOnLostFocus: closeOnFocusOut, event, reason });

        if (props.modal && !nextOpen) return;
        if (!event?.type) {
          setIsOpen(nextOpen ?? false);
          return;
        }
        if (!closeOnFocusOut && event.type === 'focusout') return;
        if (props.modal && event.type === 'focusout') return;

        setIsOpen(nextOpen ?? false);
      },
      open: isOpen,
      placement,
      strategy: 'absolute',
      transform: true,
      whileElementsMounted: autoUpdate
    });

    const { styles: transitionStyles } = useTransitionStyles(context);
    const { getReferenceProps, getFloatingProps } = useInteractions([useClick(context), useDismiss(context), useRole(context)]);

    useImperativeHandle(ref, () => ({
      hide: () => setIsOpen(false),
      show: () => setIsOpen(true)
    }));

    const openPanel = useCallback(
      (e: SyntheticEvent) => {
        if (answer !== undefined) {
          onAnswered?.();
          return;
        }
        setIsOpen((prev) => !prev);
      },
      [answer, onAnswered]
    );

    const headingId = useId();
    const borderClass = info ? 'info-header-text-bottom-border' : 'info-header-text';

    const { buttonElement } = useSMButton({
      buttonDisabled,
      ...props,
      getReferenceProps,
      refs
    });

    const content = useMemo(() => {
      return (
        <SMCard
          darkBackGround
          header={
            <div className="justify-content-end align-items-center flex-row flex gap-1">
              {header}
              {showClose && <CloseButton onClick={openPanel} tooltip="Close" />}
            </div>
          }
          {...props}
        >
          <div className="sm-card-children">
            {info && <div className={`${borderClass} sm-card-children-info`}>{info}</div>}
            <div className="sm-card-children-content">{children}</div>
          </div>
        </SMCard>
      );
    }, [borderClass, children, header, info, openPanel, props, showClose]);

    const getStyle = useMemo((): CSSProperties => {
      if (props.modal && props.modalCentered) {
        return { left: '50%', position: 'absolute', top: '50%', transform: 'translate(-50%, -50%)', width: '100%' };
      }
      return floatingStyles;
    }, [props.modal, props.modalCentered, floatingStyles]);

    if (isLoading) {
      return <BlockUI blocked={isLoading}>{/* Add loading content here */}</BlockUI>;
    }

    return (
      <>
        {buttonElement}
        {!buttonDisabled && (
          <FloatingPortal>
            {isOpen && (
              <div>
                <FloatingOverlay className={props.modal ? 'Dialog-overlay' : ''} lockScroll />
                <FloatingFocusManager context={context} modal={props.modal}>
                  <>
                    <div
                      className={`sm-overlay sm-popover sm-w-${contentWidthSize} pt-2 ${props.modal ? 'sm-modal' : ''}`}
                      ref={refs.setFloating}
                      style={getStyle}
                      aria-labelledby={headingId}
                      {...getFloatingProps()}
                    >
                      {props.modal !== true && (
                        <FloatingArrow ref={arrowRef} context={context} height={ARROW_HEIGHT} fill="var(--surface-border)" tipRadius={4} />
                      )}
                      <div style={transitionStyles}>{content}</div>
                    </div>
                  </>
                </FloatingFocusManager>
              </div>
            )}
          </FloatingPortal>
        )}
      </>
    );
  }
);

const SMOverlay = forwardRef<SMOverlayRef, ExtendedSMOverlayProperties>((props, ref) => (
  <CombinedProvider>
    <SMOverlayInner ref={ref} {...props} />
  </CombinedProvider>
));

export default SMOverlay;
