import { BlockUI } from "primereact/blockui";
import { Dialog } from "primereact/dialog";
import { OverlayPanel } from "primereact/overlaypanel";
import React, { useCallback, useEffect, useRef, useState } from "react";

type Severity = 'error' | 'info' | 'success' | 'warn';

type InfoMessageOverLayDialogProps = {
  readonly blocked?: boolean;
  readonly children: React.ReactNode;
  readonly closable?: boolean;
  readonly header?: string;
  readonly infoMessage: string | undefined;
  readonly maximizable?: boolean;
  readonly onClose: () => void;
  readonly overlayColSize?: number;
  readonly severity?: Severity | null;
  readonly show: boolean;
}

const InfoMessageOverLayDialog: React.FC<InfoMessageOverLayDialogProps> = (props) => {
  const {
    blocked = false,
    children,
    closable = true,
    header = '',
    infoMessage,
    maximizable = true,
    onClose,
    overlayColSize = 4,
    severity,
    show
  } = props;

  const [showDialog, setShowDialog] = useState<boolean>(show);

  const op = useRef<OverlayPanel | null>(null);
  const anchorRef = useRef<Dialog | null>(null);

  const hideOverlayAndDialog = useCallback(() => {
    op.current?.hide();
    setShowDialog(false);
    onClose();
  }, [onClose]);

  useEffect(() => {
    setShowDialog(show);
  }, [show]);

  useEffect(() => {
    if (!infoMessage) return;

    if (!anchorRef.current?.getElement()) return;

    op.current?.show(null, anchorRef.current.getElement());

    const timer = setTimeout(hideOverlayAndDialog, 1500);

    return () => clearTimeout(timer);
  }, [infoMessage, hideOverlayAndDialog]);

  const determineSeverityColor = (): string => {
    switch (severity) {
      case 'info':
        return 'text-primary-500';
      case 'error':
        return 'text-red-500';
      case 'success':
        return 'text-green-500';
      case 'warn':
        return 'text-yellow-500';
      default:
        if (infoMessage?.toLowerCase().includes('error') ||
          infoMessage?.toLowerCase().includes('failed')) {
          return 'text-red-500';
        }

        return 'text-green-500';
    }
  };

  return (
    <>
      <Dialog
        className={`col-${overlayColSize} p-0`}
        closable={closable}
        header={header}
        maximizable={maximizable}
        modal
        onHide={hideOverlayAndDialog}
        ref={anchorRef}
        visible={showDialog}
      >
        <BlockUI blocked={blocked}>
          <div className='flex p-0 pt-3 pb-3 border-1 border-round surface-border justify-contents-center align-items-center'>
            {children}
          </div>
        </BlockUI>
      </Dialog>

      <OverlayPanel
        className={`col-${overlayColSize} p-0`}
        dismissable={false}
        ref={op}
        showCloseIcon={false}
      >
        <div className='flex m-0  p-1 border-1 border-round surface-border justify-contents-center'>
          <div className='surface-overlay surface-overlay min-h-full min-w-full'>
            <h4 className={`text-center ${determineSeverityColor()}`}>{infoMessage}</h4>
          </div>
        </div>
      </OverlayPanel>
    </>
  );
};

InfoMessageOverLayDialog.displayName = 'InfoMessageOverLayDialog';

export default React.memo(InfoMessageOverLayDialog);
