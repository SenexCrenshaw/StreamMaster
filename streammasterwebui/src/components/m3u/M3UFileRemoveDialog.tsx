import { Checkbox } from "primereact/checkbox";
import { useState, useCallback, memo } from "react";
import { type M3UFileDto } from "../../store/iptvApi";
import { type DeleteM3UFileRequest } from "../../store/iptvApi";
import { DeleteM3UFile } from "../../store/signlar_functions";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import DeleteButton from "../buttons/DeleteButton";

const M3UFileRemoveDialog = (props: M3UFileRemoveDialogProps) => {

  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [block, setBlock] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');


  const [deleteFSFile, setDeleteFSFile] = useState<boolean>(true);

  const ReturnToParent = useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
  }, []);

  const deleteFile = async () => {

    if (!props.selectedFile) {
      return;
    }

    setBlock(true);
    const tosend = {} as DeleteM3UFileRequest;

    tosend.id = props.selectedFile.id;
    tosend.deleteFile = deleteFSFile;

    await DeleteM3UFile(tosend)
      .then(() => {
        setInfoMessage('M3U File Removed Successfully');
      }).catch((e) => {
        setInfoMessage('M3U File Removed Error: ' + e.message);
      });


  };


  return (
    <>

      <InfoMessageOverLayDialog
        blocked={block}
        header='Delete M3U File'
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        show={showOverlay}
      >
        <div className="confirmation-content ">
          <i
            className="pi pi-exclamation-triangle mr-3"
            style={{ fontSize: '2rem' }}
          />
          {props.selectedFile && (
            <span>Are you sure you want to delete the selected? </span>
          )}
          <span className="flex flex-column align-items-center justify-content-center font-bold">
            {props.selectedFile && (
              <span className="font-bold">{props.selectedFile.name}</span>
            )}
            {props.selectedFile?.url && props.selectedFile.url !== '' && (
              <div className="flex align-items-center justify-content-center font-normal mt-3">
                <Checkbox
                  checked={deleteFSFile}
                  className="mr-2"
                  onChange={(e) => setDeleteFSFile(e.checked as boolean)}
                />
                Delete file from server?
              </div>
            )}
          </span>
          <div className="card flex mt-3 flex-wrap gap-2 justify-content-center">

            <DeleteButton label="Delete M3U File" onClick={async () => await deleteFile()} />

          </div>
        </div>
      </InfoMessageOverLayDialog>

      <DeleteButton iconFilled={false} onClick={() => setShowOverlay(true)} tooltip="Delete M3U File" />

    </>
  );
}

M3UFileRemoveDialog.displayName = 'M3UFileRemoveDialog';

type M3UFileRemoveDialogProps = {
  selectedFile: M3UFileDto;
};

export default memo(M3UFileRemoveDialog);
