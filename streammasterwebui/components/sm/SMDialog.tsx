import { forwardRef, useEffect, useImperativeHandle, useState } from 'react';

import CloseButton from '@components/buttons/CloseButton';
import { Dialog } from 'primereact/dialog';
import SMButton from './SMButton';
import { SMCard } from './SMCard';

interface SMDialogProperties {
  readonly children: React.ReactNode;
  readonly buttonDisabled?: boolean;
  readonly buttonClassName?: string;
  readonly darkBackGround?: boolean;
  readonly header?: React.ReactNode;
  readonly info?: string;
  readonly icon?: string;
  readonly iconFilled?: boolean;
  readonly label?: string;
  readonly showButton?: boolean | null;
  readonly title: string;
  readonly tooltip?: string;
  readonly widthSize?: number;
  readonly position?: 'center' | 'top' | 'bottom' | 'left' | 'right' | 'top-left' | 'top-right' | 'bottom-left' | 'bottom-right' | undefined;

  onHide?(): void;
}
export interface SMDialogRef {
  hide: () => void;
  show: () => void;
}

const SMDialog = forwardRef<SMDialogRef, SMDialogProperties>((props: SMDialogProperties, ref) => {
  const {
    buttonClassName,
    buttonDisabled,
    children,
    darkBackGround = false,
    header,
    info = 'General',
    icon = 'pi pi-plus',
    iconFilled = false,
    label,
    onHide,
    position,
    showButton,
    tooltip,
    title,
    widthSize = 4
  } = props;

  useImperativeHandle(ref, () => ({
    hide: () => setVisible(false),
    props,
    show: () => setVisible(true)
  }));

  const [visible, setVisible] = useState<boolean>(false);

  const borderClass = info !== '' ? 'info-header-text-bottom-border' : 'info-header-text';

  useEffect(() => {
    if (showButton === false) {
      setVisible(true);
    }
  }, [showButton]);

  return (
    <>
      <Dialog
        position={position}
        className={`sm-dialog w-${widthSize}`}
        visible={visible}
        style={{ width: '40vw' }}
        onHide={() => {
          setVisible(false);
          onHide && onHide();
        }}
        content={({ hide }) => (
          <SMCard
            darkBackGround={darkBackGround}
            title={title}
            header={
              <div className="justify-content-end align-items-center flex-row flex gap-1">
                {header}
                <CloseButton onClick={(e) => setVisible(false)} tooltip="Close" />
              </div>
            }
          >
            <div className="sm-card-children">
              {info && info !== '' && <div className={`${borderClass} sm-card-children-info`}>{info}</div>}
              <div className="sm-card-children-content">{children}</div>
            </div>
          </SMCard>
        )}
      />

      <SMButton
        buttonDisabled={buttonDisabled}
        buttonClassName={buttonClassName}
        iconFilled={iconFilled}
        icon={icon ?? ''}
        label={iconFilled === true ? label : undefined}
        tooltip={tooltip}
        onClick={(e) => {
          setVisible(true);
        }}
      />
    </>
  );
});

export default SMDialog;
