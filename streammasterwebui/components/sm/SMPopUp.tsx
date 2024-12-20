import { forwardRef, useCallback, useImperativeHandle, useRef } from 'react';
import { SMPopUpProperties } from './Interfaces/SMPopUpProperties';
import { SMCard } from './SMCard';
import SMOverlay, { SMOverlayRef } from './SMOverlay';

export interface SMPopUpRef {
  getOpen: () => boolean;
  hide: () => void;
  show: (event: any) => void;
}

// interface RememberProps {
//   value: boolean;
//   checked: boolean;
// }

const SMPopUp = forwardRef<SMPopUpRef, SMPopUpProperties>(
  (
    {
      children,
      contentWidthSize = '2',
      disabled = false,
      noFullScreen = false,
      noCloseButton = true,
      isPopupLoading = false,
      onCloseClick,
      onOkClick,
      // rememberKey,
      // showRemember = false,
      ...props
    },
    ref
  ) => {
    const overlayRef = useRef<SMOverlayRef>(null);
    // const [remember, setRemeber] = useLocalStorage<RememberProps | null>(null, 'remember-' + rememberKey);
    // const checked = remember?.checked ? remember.checked : false ?? false;

    const ok = useCallback((): void | undefined => {
      onOkClick?.(); // Call the custom onClick handler if provided
      // if (rememberKey && rememberKey !== '' && remember?.checked === true) {
      //   setRemeber({ checked: true, value: false } as RememberProps);
      //   Logger.debug('Remember', { remember });
      // }
      overlayRef.current?.hide();
    }, [onOkClick]);

    const closed = useCallback(() => {
      onCloseClick?.(); // Call the custom onClick handler if provided
      // if (rememberKey && rememberKey !== '' && remember?.checked === true) {
      //   setRemeber({ checked: true, value: false } as RememberProps);
      //   Logger.debug('Remember', { remember });
      // }
      overlayRef.current?.hide();
    }, [onCloseClick]);

    useImperativeHandle(ref, () => ({
      getOpen: () => {
        return overlayRef.current?.getOpen() ?? false;
      },
      hide: () => overlayRef.current?.hide(),
      show: (event: any) => overlayRef.current?.show(event)
    }));

    // Logger.debug('Remember', { remember });

    return (
      <SMOverlay
        ref={overlayRef}
        buttonDisabled={disabled}
        noFullScreen={noFullScreen}
        noCloseButton={noCloseButton}
        isOverLayLoading={isPopupLoading}
        contentWidthSize={contentWidthSize}
        onOkClick={onOkClick ? ok : undefined}
        onCloseClick={closed}
        // onAnswered={() => {
        //   if (rememberKey && rememberKey !== '' && remember !== null) {
        //     if (remember.checked === true) {
        //       onOkClick && onOkClick();
        //       overlayRef.current?.hide();
        //     }
        //   }
        // }}
        // answer={remember?.checked ? remember?.value : undefined ?? undefined}
        {...props}
      >
        {
          children && (
            // (showRemember && rememberKey && ( */}
            <SMCard>
              <>
                {children}
                {/* {showRemember && !props.modal && (
                  <div className="flex w-full align-items-center justify-content-end pt-1">
                    <div className="sm-border-divider pt-1">
                      <BooleanEditor
                        label="Don't Ask Again?"
                        labelInline
                        labelSmall
                        checked={checked}
                        onChange={(e) => setRemeber({ checked: e, value: e } as RememberProps)}
                      />
                    </div>
                  </div>
                )} */}
              </>
            </SMCard>
          )
          // {/* )) */}
        }
      </SMOverlay>
    );
  }
);

export default SMPopUp;
