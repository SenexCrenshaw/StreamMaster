import React from "react";
import { Button } from "primereact/button";
import { getTopToolOptions } from "../common/common";
import type * as StreamMasterApi from '../store/iptvApi';
import { Checkbox } from "primereact/checkbox";
import { type DeleteM3UFileRequest } from "../store/iptvApi";
import { DeleteM3UFile } from "../store/signlar_functions";
import InfoMessageOverLayDialog from "./InfoMessageOverLayDialog";

const M3UFileRemoveDialog = (props: M3UFileRemoveDialogProps) => {

  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const [infoMessage, setInfoMessage] = React.useState('');


  const [deleteFSFile, setDeleteFSFile] = React.useState<boolean>(true);

  const ReturnToParent = React.useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
  }, []);

  const deleteFile = () => {

    if (!props.selectedFile) {
      return;
    }

    setBlock(true);
    const tosend = {} as DeleteM3UFileRequest;

    tosend.id = props.selectedFile.id;
    tosend.deleteFile = deleteFSFile;

    DeleteM3UFile(tosend)
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
        icon="pi pi-minus"
        onClick={() => setShowOverlay(true)}
        rounded
        severity="danger"
        size="small"
        style={{
          ...{
            maxHeight: "2rem",
            maxWidth: "2rem"
          }
        }}
        tooltip="Delete M3U File"
        tooltipOptions={getTopToolOptions}
      />

    </>
  );
}

M3UFileRemoveDialog.displayName = 'M3UFileRemoveDialog';

type M3UFileRemoveDialogProps = {
  // onFileDeleted: () => void;
  selectedFile?: StreamMasterApi.M3UFilesDto;
};

export default React.memo(M3UFileRemoveDialog);
