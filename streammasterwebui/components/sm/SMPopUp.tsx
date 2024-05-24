import CloseButton from '@components/buttons/CloseButton';
import OKButton from '@components/buttons/OKButton';
import { Checkbox } from 'primereact/checkbox';
import { useLocalStorage } from 'primereact/hooks';
import { OverlayPanel } from 'primereact/overlaypanel';
import { useEffect, useRef } from 'react';
import { SMCard } from './SMCard';
import { SMOverlay } from './SMOverlay';

interface SMPopUpProperties {
  readonly buttonClassName?: string;
  readonly hidden?: boolean;
  readonly icon?: string;
  readonly iconFilled?: boolean;
  readonly title: string;
  readonly tooltip?: string;
  OK(): void;
}

export const SMPopUp = ({ buttonClassName = 'icon-red', hidden, icon, iconFilled, OK, title, tooltip }: SMPopUpProperties) => {
  const op = useRef<OverlayPanel>(null);
  const anchorRef = useRef<HTMLDivElement>(null);
  const [remember, setRemeber] = useLocalStorage<boolean | undefined>(undefined, 'remember-' + title);

  const borderClass = 'info-header-text';

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
    <SMOverlay iconFilled={iconFilled} simple title={title} widthSize="2" icon={icon} buttonClassName={buttonClassName} tooltip={tooltip}>
      <SMCard
        darkBackGround
        title={title}
        header={
          <div className="flex align-items-center">
            <Checkbox checked={remember ?? false} onChange={(e) => setRemeber(e.checked)} />
            <div className="ml-2 text-xs font-italic">Don't Ask Again</div>
            <OKButton
              onClick={(e) => {
                op.current?.hide();
                OK();
              }}
              tooltip="Close"
            />
            <CloseButton onClick={(e) => op.current?.hide()} tooltip="Close" />
          </div>
        }
      >
        <div className="sm-card-children">
          <div className={`${borderClass} sm-card-children-info`}></div>
          <div className="sm-card-children-content">
            <div className="justify-content-end align-items-center flex-row flex gap-1"></div>
          </div>
        </div>
      </SMCard>

      {/* <div className="p-4">{children}</div>
      <div className="flex flex-row justify-content-end align-items-center gap-1 pb-1 pr-1">
        <div className="flex flex-column align-items-center">
          <Checkbox checked={remember ?? false} onChange={(e) => setRemeber(e.checked)} />
          <div className="ml-2 text-xs font-italic">Don't Ask Again</div>
        </div>
        <SMButton
          label="Ok"
          icon="pi-check"
          className="icon-green-filled"
          onClick={(event) => {
            op.current?.hide();
            OK();
          }}
        />
      </div> */}
    </SMOverlay>
  );
};
