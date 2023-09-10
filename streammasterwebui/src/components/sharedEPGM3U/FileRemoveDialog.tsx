import { memo, useCallback, useEffect, useState } from "react";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import DeleteButton from "../buttons/DeleteButton";


type FileRemoveDialogProps = {
  readonly fileType: 'epg' | 'm3u';
  readonly infoMessage?: string;
  readonly onDeleteFile: () => void;
};

const FileRemoveDialog = ({ fileType, infoMessage: inputInfoMessage, onDeleteFile }: FileRemoveDialogProps) => {
  const labelName = fileType.toUpperCase();

  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [block, setBlock] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState<string | undefined>(undefined);

  useEffect(() => {
    setInfoMessage(inputInfoMessage);
  }, [inputInfoMessage]);

  const ReturnToParent = useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
  }, []);

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        closable
        header='Delete EPG File'
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        show={showOverlay}
      >

        <div className="flex grid w-full">
          <div className="flex col-12 justify-content-center align-items-center">
            <i className="pi pi-exclamation-triangle mr-3" style={{ fontSize: '2rem' }} />
            <span>Are you sure you want to delete? </span>
          </div>
          <div className="flex col-12 justify-content-center">
            <DeleteButton label={"Delete " + labelName} onClick={onDeleteFile} tooltip={"Delete " + labelName} />
          </div>
        </div>
      </InfoMessageOverLayDialog>

      <DeleteButton iconFilled={false} onClick={() => setShowOverlay(true)} tooltip={"Delete " + labelName} />


    </ >
  );
}

export default memo(FileRemoveDialog);
