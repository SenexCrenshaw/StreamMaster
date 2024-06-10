import OKButton from '@components/buttons/OKButton';
import BooleanEditor from '@components/inputs/BooleanEditor';
import { Logger } from '@lib/common/logger';
import { useLocalStorage } from 'primereact/hooks';
import React, { useRef } from 'react';
import { SMPopUpProperties } from './Interfaces/SMPopUpProperties';
import SMButton from './SMButton';
import { SMCard } from './SMCard';
import SMOverlay, { SMOverlayRef } from './SMOverlay';

interface RememberProps {
  value: boolean;
  checked: boolean;
}

export const SMPopUp: React.FC<SMPopUpProperties> = ({
  children,
  closeButtonDisabled = false,
  contentWidthSize = '2',
  disabled = false,
  okButtonDisabled = false,
  onCloseClick,
  onOkClick,
  rememberKey,
  showRemember = true,
  ...props
}) => {
  const overlayRef = useRef<SMOverlayRef>(null);
  const [remember, setRemeber] = useLocalStorage<RememberProps | null>(null, 'remember-' + rememberKey);

  const checked = remember?.checked ? remember.checked : false ?? false;
  if (props.title === 'Add M3U') {
    Logger.debug('SMPopUp', props.title, { modal: props.modal, modalCentered: props.modalCentered });
  }
  return (
    <SMOverlay
      ref={overlayRef}
      contentWidthSize={contentWidthSize}
      onAnswered={() => {
        if (rememberKey && rememberKey !== '' && remember !== null) {
          if (remember.checked === true && remember.value !== undefined) {
            if (remember.value === true) {
              onOkClick();
            }
            overlayRef.current?.hide();
          }
        }
      }}
      header={
        <div className="flex align-items-center gap-1">
          <OKButton
            disabled={disabled || okButtonDisabled} // Combine the disabled states
            onClick={(e) => {
              if (rememberKey && rememberKey !== '' && remember?.checked === true) {
                setRemeber({ checked: true, value: true } as RememberProps);
              }
              overlayRef.current?.hide();
              onOkClick();
            }}
            tooltip="Ok"
          />
          <SMButton
            icon="pi-times"
            iconFilled
            className="icon-red"
            disabled={disabled || closeButtonDisabled} // Combine the disabled states
            onClick={(e) => {
              onCloseClick?.(); // Call the custom onClick handler if provided
              if (rememberKey && rememberKey !== '' && remember?.checked === true) {
                setRemeber({ checked: true, value: false } as RememberProps);
                Logger.debug('Remember', { remember });
              }
              overlayRef.current?.hide();
            }}
            tooltip="Close"
          />
        </div>
      }
      answer={remember?.checked ? remember?.value : undefined ?? undefined}
      {...props}
    >
      <SMCard darkBackGround>
        <div>
          {children}
          {showRemember && (
            <div className="flex pl-1 w-full">
              <div className="w-7">
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
        </div>
      </SMCard>
    </SMOverlay>
  );
};

export default SMPopUp;
