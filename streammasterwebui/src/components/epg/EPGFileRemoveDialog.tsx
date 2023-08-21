import { Button } from "primereact/button";
import { Checkbox } from "primereact/checkbox";
import { useState, useCallback, memo } from "react";
import { getTopToolOptions } from "../../common/common";
import { type DeleteEpgFileRequest, type EpgFilesDto } from "../../store/iptvApi";
import { DeleteEPGFile } from "../../store/signlar_functions";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";

const EPGFileRemoveDialog = (props: EPGFileRemoveDialogProps) => {
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

    const tosend = {} as DeleteEpgFileRequest;

    tosend.id = props.selectedFile.id;
    tosend.deleteFile = deleteFSFile;

    await DeleteEPGFile(tosend)
      .then(() => {
        setInfoMessage('EPG File Removed Successfully');
      }).catch((e) => {
        setInfoMessage('EPG File Removed Error: ' + e.message);
      });

  };



  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        header='Delete EPG File'
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
            <span>Are you sure you want to delete? </span>
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
            <Button
              icon="pi pi-times "
              label="Cancel"
              onClick={(() => ReturnToParent())}
              rounded
              severity="warning"
            />
            <Button
              icon="pi pi-check"
              label="Delete"
              onClick={deleteFile}
              rounded
              severity="danger"
            />
          </div>
        </div>
      </InfoMessageOverLayDialog>
      <Button
        disabled={props.selectedFile === undefined || props.selectedFile.name === undefined || props.selectedFile.name === ''}
        icon="pi pi-minus"
        onClick={() => setShowOverlay(true)}
        rounded
        severity="danger"
        size="small"
        tooltip="Delete EPG File"
        tooltipOptions={getTopToolOptions}
      />
    </ >
  );
}

EPGFileRemoveDialog.displayName = 'EPGFileRemoveDialog';

type EPGFileRemoveDialogProps = {
  selectedFile?: EpgFilesDto;
};

export default memo(EPGFileRemoveDialog);
