import OKButton from '@components/buttons/OKButton';
import BooleanEditor from '@components/inputs/BooleanEditor';
import { Logger } from '@lib/common/logger';
import { useLocalStorage } from 'primereact/hooks';
import { useEffect, useRef } from 'react';
import SMButton from './SMButton';
import { SMCard } from './SMCard';
import SMOverlay, { SMOverlayRef } from './SMOverlay';

interface SMPopUpProperties {
  readonly buttonClassName?: string;
  readonly buttonDisabled?: boolean;
  readonly children?: React.ReactNode;
  readonly disabled?: boolean;
  readonly hidden?: boolean;
  readonly icon?: string;
  readonly iconFilled?: boolean;
  readonly label?: string;
  readonly rememberKey?: string;
  readonly showRemember?: boolean;
  readonly title: string;
  readonly tooltip?: string;
  OK(): void;
}

interface RememberProps {
  value: boolean;
  checked: boolean;
}

export const SMPopUp = ({
  buttonClassName = 'icon-red',
  buttonDisabled = false,
  children,
  disabled = false,
  hidden: parentHidden,
  icon,
  iconFilled,
  label,
  rememberKey,
  showRemember = true,
  OK,
  title,
  tooltip
}: SMPopUpProperties) => {
  const anchorRef = useRef<HTMLDivElement>(null);
  const overlayRef = useRef<SMOverlayRef>(null);
  const [remember, setRemeber] = useLocalStorage<RememberProps | null>(null, 'remember-' + rememberKey);

  useEffect(() => {
    if (parentHidden !== undefined) {
      if (parentHidden) {
        overlayRef.current?.hide();
      } else {
        overlayRef.current?.show(anchorRef.current);
      }
    }
  }, [parentHidden]);

  const checked = remember?.checked ? remember.checked : false ?? false;

  return (
    <SMOverlay
      onAnswered={() => {
        if (rememberKey && rememberKey !== '' && remember !== null) {
          if (remember.checked === true && remember.value !== undefined) {
            if (remember.value === true) {
              OK();
            }
            overlayRef.current?.hide();
          }
        }
      }}
      answer={remember?.checked ? remember?.value : undefined ?? undefined}
      buttonClassName={buttonClassName}
      buttonDisabled={buttonDisabled}
      contentWidthSize="2"
      label={label}
      icon={icon}
      iconFilled={iconFilled}
      ref={overlayRef}
      simple
      title={title}
      tooltip={tooltip}
    >
      <SMCard
        darkBackGround
        title={title}
        header={
          <div className="flex align-items-center gap-1">
            Are you sure?
            <OKButton
              disabled={disabled}
              onClick={(e) => {
                if (rememberKey && rememberKey !== '' && remember?.checked === true) {
                  setRemeber({ checked: true, value: true } as RememberProps);
                }
                overlayRef.current?.hide();
                OK();
              }}
              tooltip="Ok"
            />
            <SMButton
              icon="pi-times"
              iconFilled
              className="icon-red"
              onClick={(e) => {
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
      >
        <div className="sm-card-children">
          <div className="flex flex-column w-full justify-items-center align-items-center align-content-center settings-line">
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
        </div>
      </SMCard>
    </SMOverlay>
  );
};
