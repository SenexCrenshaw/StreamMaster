import BaseButton from '@components/buttons/BaseButton';
import XButton from '@components/buttons/XButton';
import { Dialog } from 'primereact/dialog';
import { useEffect, useState } from 'react';
import { SMCard } from './SMCard';

interface SMDialogProperties {
  readonly children: React.ReactNode;
  readonly buttonClassName?: string;
  readonly info?: string;
  readonly icon?: string;
  readonly iconFilled?: boolean;
  readonly showButton?: boolean | null;
  readonly title: string;
  readonly tooltip?: string;
  readonly close?: boolean;
  readonly widthSize?: number;
  readonly position?: 'center' | 'top' | 'bottom' | 'left' | 'right' | 'top-left' | 'top-right' | 'bottom-left' | 'bottom-right' | undefined;
  onHide?(): void;
}

export const SMDialog = ({
  buttonClassName,
  children,
  close,
  info = '',
  iconFilled = true,
  icon = 'pi pi-plus',
  onHide,
  position,
  showButton,
  tooltip,
  title,
  widthSize = 4
}: SMDialogProperties) => {
  const [visible, setVisible] = useState<boolean>(false);

  const borderClass = info !== '' ? 'info-header-text-bottom-border' : 'info-header-text';

  useEffect(() => {
    if (close === true) {
      setVisible(false);
      onHide && onHide();
    }
  }, [close, onHide]);

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
            title={title}
            header={
              <XButton
                iconFilled={false}
                onClick={(e) => {
                  setVisible(false);
                }}
                tooltip="Close"
              />
            }
          >
            <div className="sm-card-children">
              <span className={`${borderClass} flex`}>{info}</span>
              <div className="sm-card-content-children">{children}</div>
            </div>
          </SMCard>
        )}
      />
      <div hidden={showButton === false}>
        <BaseButton
          className={buttonClassName}
          iconFilled={iconFilled}
          icon={icon}
          tooltip={tooltip}
          onClick={(e) => {
            setVisible(true);
          }}
        />
      </div>
    </>
  );
};
