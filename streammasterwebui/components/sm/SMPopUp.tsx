import OKButton from '@components/buttons/OKButton';
import XButton from '@components/buttons/XButton';

import { Checkbox } from 'primereact/checkbox';
import { useLocalStorage } from 'primereact/hooks';
import { OverlayPanel } from 'primereact/overlaypanel';
import { useEffect, useRef } from 'react';
import SMButton, { SeverityType } from './SMButton';
import { SMCard } from './SMCard';
import { SMOverlay } from './SMOverlay';

interface SMPopUpProperties {
  readonly children: React.ReactNode;
  readonly title: string;
  readonly message?: string;
  readonly hidden?: boolean;
  readonly tooltip?: string;
  readonly icon?: string;
  readonly severity?: SeverityType;
  onHide?(): void;
  onShow?(): void;
  OK(): void;
  Cancel?(): void;
}

export const SMPopUp = ({ children, hidden, icon, severity, tooltip, onHide: clientHide, OK, Cancel, onShow, title }: SMPopUpProperties) => {
  const op = useRef<OverlayPanel>(null);
  const anchorRef = useRef<HTMLDivElement>(null);
  const [remember, setRemeber] = useLocalStorage<boolean | undefined>(undefined, 'remember-' + title);

  useEffect(() => {
    if (hidden !== undefined) {
      if (hidden) {
        op.current?.hide();
      } else {
        op.current?.show(null, anchorRef.current);
      }
    }
  }, [hidden]);

  return (
    <SMOverlay
      // buttonTemplate={buttonTemplate}
      iconFilled={false}
      title="CHANNEL GROUPS"
      widthSize="3"
      icon="pi-times"
      buttonClassName="icon-red"
      //buttonLabel="EPG"
    >
      <>
        <OverlayPanel ref={op} onHide={clientHide} onShow={onShow}>
          <SMCard title={title}>
            <div className="p-4">{children}</div>
            <div className="flex flex-row justify-content-end align-items-center gap-1 pb-1 pr-1">
              <div className="flex flex-column align-items-center">
                <Checkbox checked={remember ?? false} onChange={(e) => setRemeber(e.checked)} />
                <div className="ml-2 text-xs font-italic">Don't Ask</div>
              </div>

              <XButton
                label="Cancel"
                onClick={(event) => {
                  op.current?.hide();
                  Cancel && Cancel();
                }}
              />
              <OKButton
                onClick={() => {
                  op.current?.hide();
                  OK();
                }}
              />
            </div>
          </SMCard>
        </OverlayPanel>

        {hidden === undefined && (
          <SMButton
            onClick={(event) => {
              if (remember === true) {
                op.current?.hide();
                OK();
              } else {
                op.current?.toggle(event);
              }
            }}
            tooltip={tooltip}
            iconFilled={false}
            icon={icon ?? 'pi-plus'}
            severity={severity ?? 'info'}
          />
        )}
      </>
    </SMOverlay>
  );
};
