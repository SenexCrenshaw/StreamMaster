import React from "react";

import { Button } from "primereact/button";
import { getTopToolOptions } from "../common/common";
import { Checkbox } from "primereact/checkbox";
import { Dialog } from "primereact/dialog";
import type * as StreamMasterApi from '../store/iptvApi';
import { DeleteEPGFile } from "../store/signlar_functions";
import { Toast } from 'primereact/toast';

const EPGFileRemoveDialog = (props: EPGFileRemoveDialogProps) => {
  const toast = React.useRef<Toast>(null);
  const [deleteDialog, setDeleteDialog] = React.useState<boolean>();
  const [deleteFSFile, setDeleteFSFile] = React.useState<boolean>(true);

  const deleteFile = async () => {
    if (!props.selectedFile) {
      toast.current?.show({
        detail: `EPG File Delete Failed`,
        life: 3000,
        severity: 'error',
        summary: 'Error',
      });
      return;
    }

    const tosend = {} as StreamMasterApi.DeleteEpgFileRequest;

    tosend.id = props.selectedFile.id;
    tosend.deleteFile = deleteFSFile;

    await DeleteEPGFile(tosend)
      .then((returnData) => {
        if (toast.current) {
          if (returnData) {
            toast.current.show({
              detail: `EPG File Delete Successful`,
              life: 3000,
              severity: 'success',
              summary: 'Successful',
            });
          } else {
            toast.current.show({
              detail: `EPG File Delete Failed`,
              life: 3000,
              severity: 'error',
              summary: 'Error',
            });
          }
        }
      }).catch((e) => {
        if (toast.current) {
          toast.current.show({
            detail: `EPG File Delete Failed`,
            life: 3000,
            severity: 'error',
            summary: 'Error ' + e.message,
          });
        }
      });

    setDeleteDialog(false);
  };

  const deleteDialogFooter = (
    <>
      <Button
        className="p-button-text"
        icon="pi pi-times"
        label="No"
        onClick={() => setDeleteDialog(false)}
      />
      <Button
        className="p-button-text"
        icon="pi pi-check"
        label="Yes"
        onClick={deleteFile}
      />
    </>
  );

  return (
    <>
      <Toast position="bottom-right" ref={toast} />
      <Dialog
        footer={deleteDialogFooter}
        header="Confirm"
        modal
        onHide={() => setDeleteDialog(false)}
        style={{ width: '450px' }}
        visible={deleteDialog}
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
        </div>
      </Dialog>
      <Button
        disabled={props.selectedFile === undefined || props.selectedFile.name === undefined || props.selectedFile.name === ''}
        icon="pi pi-minus"
        onClick={() => setDeleteDialog(true)}
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
  selectedFile?: StreamMasterApi.EpgFilesDto;
};

export default React.memo(EPGFileRemoveDialog);
