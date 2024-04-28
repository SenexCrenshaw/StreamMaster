import BaseButton from '@components/buttons/BaseButton';
import XButton from '@components/buttons/XButton';
import { OverlayPanel } from 'primereact/overlaypanel';
import { useRef } from 'react';
import { SMCard } from './SMCard';

interface SMOverlayProperties {
  readonly children: React.ReactNode;
  readonly header: React.ReactNode;
  readonly buttonClassName?: string | undefined;
  readonly buttonLabel?: string | undefined;
  readonly icon?: string | undefined;
  readonly title: string | undefined;
  readonly tooltip?: string | undefined;
  readonly widthSize?: string;
  onHide?(): void;
}

export const SMOverlay = ({
  buttonClassName = '',
  buttonLabel = '',
  children,
  header,
  icon = 'pi-plus',
  onHide,
  tooltip = '',
  title = '',
  widthSize = '4'
}: SMOverlayProperties) => {
  const op = useRef<OverlayPanel>(null);

  return (
    <>
      <OverlayPanel className={`sm-overlay w-${widthSize}`} ref={op} showCloseIcon={false} onHide={() => onHide && onHide()}>
        <SMCard
          title={title}
          header={
            <div className="justify-content-end flex-row flex gap-1">
              {header}
              <XButton iconFilled onClick={(e) => op.current?.toggle(e)} tooltip="Close" />
            </div>
          }
        >
          {children}
        </SMCard>
      </OverlayPanel>
      <BaseButton className={buttonClassName} iconFilled icon={icon} tooltip={tooltip} label={buttonLabel} onClick={(e) => op.current?.toggle(e)} />
    </>
  );
};
