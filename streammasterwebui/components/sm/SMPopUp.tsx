import { Checkbox } from 'primereact/checkbox';
import { useLocalStorage } from 'primereact/hooks';
import { OverlayPanel } from 'primereact/overlaypanel';
import { useEffect, useRef } from 'react';
import SMButton, { SeverityType } from './SMButton';
import { SMOverlay } from './SMOverlay';

interface SMPopUpProperties {
  readonly children: React.ReactNode;
  readonly title: string;
  readonly message?: string;
  readonly hidden?: boolean;
  readonly tooltip?: string;
  readonly icon?: string;
  readonly iconFilled?: boolean;
  readonly severity?: SeverityType;
  onHide?(): void;
  onShow?(): void;
  OK(): void;
  Cancel?(): void;
}

export const SMPopUp = ({ children, hidden, icon, iconFilled, severity, tooltip, onHide: clientHide, OK, Cancel, onShow, title }: SMPopUpProperties) => {
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
    <SMOverlay iconFilled={iconFilled} title={title} widthSize="2" icon="pi-times" buttonClassName="icon-red">
      <div className="p-4">{children}</div>
      <div className="flex flex-row justify-content-end align-items-center gap-1 pb-1 pr-1">
        <div className="flex flex-column align-items-center">
          <Checkbox checked={remember ?? false} onChange={(e) => setRemeber(e.checked)} />
          <div className="ml-2 text-xs font-italic">Don't Ask Again</div>
        </div>

        {/* <SMButton
          label="Cancel"
          icon="pi-times"
          className="icon-red-filled"
          onClick={(event) => {
            op.current?.hide();
            Cancel && Cancel();
          }}
        /> */}

        <SMButton
          label="Ok"
          icon="pi-check"
          className="icon-green-filled"
          onClick={(event) => {
            op.current?.hide();
            OK();
          }}
        />
      </div>
    </SMOverlay>
  );
};
