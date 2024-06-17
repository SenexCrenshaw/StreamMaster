import OKButton from '@components/buttons/OKButton';
import BooleanEditor from '@components/inputs/BooleanEditor';
import { Logger } from '@lib/common/logger';
import { useLocalStorage } from 'primereact/hooks';
import { forwardRef, useCallback, useImperativeHandle, useRef } from 'react';
import SMButton from './SMButton';
import { SMCard } from './SMCard';
import SMOverlay, { SMOverlayRef } from './SMOverlay';
import { SMPopUpProperties } from './interfaces/SMPopUpProperties';

export interface SMPopUpRef {
  getOpen: () => boolean;
  hide: () => void;
  show: (event: any) => void;
}

interface RememberProps {
  value: boolean;
  checked: boolean;
}

const SMPopUp = forwardRef<SMPopUpRef, SMPopUpProperties>(
  (
    {
      children,
      closeButtonDisabled = false,
      contentWidthSize = '2',
      disabled = false,
      hasCloseButton = true,
      okButtonDisabled = false,
      isPopupLoading = false,
      onCloseClick,
      onOkClick,
      rememberKey,
      showRemember = true,
      ...props
    },
    ref
  ) => {
    const overlayRef = useRef<SMOverlayRef>(null);
    const [remember, setRemeber] = useLocalStorage<RememberProps | null>(null, 'remember-' + rememberKey);

    const checked = remember?.checked ? remember.checked : false ?? false;

    const closed = useCallback(() => {
      onCloseClick?.(); // Call the custom onClick handler if provided
      if (rememberKey && rememberKey !== '' && remember?.checked === true) {
        setRemeber({ checked: true, value: false } as RememberProps);
        Logger.debug('Remember', { remember });
      }
      overlayRef.current?.hide();
    }, [onCloseClick, remember, rememberKey, setRemeber]);

    // Logger.debug('Popup', props.title, isPopupLoading);

    useImperativeHandle(ref, () => ({
      getOpen: () => {
        return overlayRef.current?.getOpen() ?? false;
      },
      hide: () => overlayRef.current?.hide(),
      show: (event: any) => overlayRef.current?.show(event)
    }));

    return (
      <SMOverlay
        ref={overlayRef}
        hasCloseButton={hasCloseButton}
        isOverLayLoading={isPopupLoading}
        contentWidthSize={contentWidthSize}
        onCloseClick={closed}
        onAnswered={() => {
          if (rememberKey && rememberKey !== '' && remember !== null) {
            if (remember.checked === true && remember.value !== undefined) {
              if (remember.value === true) {
                onOkClick && onOkClick();
              }
              overlayRef.current?.hide();
            }
          }
        }}
        header={
          <div className="flex align-items-center gap-1">
            {onOkClick && (
              <OKButton
                buttonDisabled={disabled || okButtonDisabled} // Combine the disabled states
                onClick={(e) => {
                  if (rememberKey && rememberKey !== '' && remember?.checked === true) {
                    setRemeber({ checked: true, value: true } as RememberProps);
                  }
                  overlayRef.current?.hide();
                  onOkClick && onOkClick();
                }}
                tooltip="Ok"
              />
            )}

            <SMButton
              icon="pi-times"
              iconFilled
              buttonClassName="icon-red"
              buttonDisabled={disabled || closeButtonDisabled}
              onClick={() => closed()}
              tooltip="Close"
            />
          </div>
        }
        answer={remember?.checked ? remember?.value : undefined ?? undefined}
        {...props}
      >
        <SMCard>
          <>
            {children}
            {showRemember && !props.modal && (
              <div className="flex w-full align-items-center justify-content-end pt-1">
                <div className="sm-border-divider pt-1">
                  <BooleanEditor
                    label="Don't Ask Again?"
                    labelInline
                    labelSmall
                    checked={checked}
                    onChange={(e) => setRemeber({ checked: e, value: remember?.value } as RememberProps)}
                  />
                </div>
              </div>
            )}
          </>
        </SMCard>
      </SMOverlay>
    );
  }
);

export default SMPopUp;
