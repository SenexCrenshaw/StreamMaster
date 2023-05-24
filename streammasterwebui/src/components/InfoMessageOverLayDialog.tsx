
import React from "react";
import { OverlayPanel } from "primereact/overlaypanel";
import { BlockUI } from "primereact/blockui";
import { Dialog } from "primereact/dialog";

const InfoMessageOverLayDialog = (props: InfoMessageOverLayDialogProps) => {

  const [showDialog, setShowDialog] = React.useState<boolean>(false);

  const op = React.useRef<OverlayPanel>(null);
  const anchorRef = React.useRef<Dialog>(null);

  const returnToParent = React.useCallback(() => {
    op.current?.hide();
    setShowDialog(false);
    props.onClose?.();
  }, [props]);

  React.useMemo(() => {
    setShowDialog(props.show);
  }, [props.show]);

  React.useMemo(() => {
    const startTimer = () => {
      setTimeout(() => {
        returnToParent();
      }, 1500);
    };

    if (op.current === null) {
      return;
    }

    if (props.infoMessage !== null && props.infoMessage !== undefined && props.infoMessage !== '') {
      if (anchorRef.current === null || anchorRef.current.getElement() === null) {
        return;
      }

      op.current.show(null, anchorRef.current.getElement());
      startTimer();
    } else {
      op.current.hide();
    }

  }, [props.infoMessage, returnToParent]);

  const getSeverity = () => {

    switch (props.severity) {
      case 'info':
        return 'text-primary-500';
      case 'error':
        return 'text-red-500';
      case 'success':
        return 'text-green-500';
      case 'warn':
        return 'text-yellow-500';
      default:
        {
          if (props.infoMessage !== undefined && props.infoMessage !== '') {
            return props.infoMessage.toLocaleLowerCase().includes('error') ||
              props.infoMessage.toLocaleLowerCase().includes('failed') ? 'text-red-500' : 'text-green-500';
          }

          return 'text-primary-500';
        }
    }
  };

  return (
    <>
      <Dialog
        className={`col-${props.overlayColSize} p-0`}
        header={props.header}
        maximizable
        modal
        onHide={() => { returnToParent(); }}
        ref={anchorRef}
        visible={showDialog}
      >
        <BlockUI blocked={props.blocked}>
          {props.children}
        </BlockUI>
      </Dialog>

      <OverlayPanel
        className={`col-${props.overlayColSize} p-0`}
        dismissable={false}
        ref={op}
        showCloseIcon={false}
      >
        <div className='flex m-0 p-1 border-1 border-round surface-border justify-contents-center'>
          <div className='surface-overlay surface-overlay min-h-full min-w-full'>
            <h4 className={`text-center ${getSeverity()}`}>{props.infoMessage}</h4>
          </div>
        </div>
      </OverlayPanel>

    </>
  );
}

InfoMessageOverLayDialog.displayName = 'InfoMessageOverLayDialog';
InfoMessageOverLayDialog.defaultProps = {
  blocked: false,
  overlayColSize: 4,
};

type InfoMessageOverLayDialogProps = {
  blocked?: boolean | undefined;
  children: React.ReactNode;
  header: string;
  infoMessage: string;
  onClose: () => void;
  overlayColSize?: number;
  severity?: string | null;
  show: boolean;
};

export default React.memo(InfoMessageOverLayDialog);
