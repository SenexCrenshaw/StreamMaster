import React from "react";
import { Button } from "primereact/button";
import { getTopToolOptions } from "../common/common";
import type * as StreamMasterApi from '../store/iptvApi';
import { Checkbox } from "primereact/checkbox";
import { Dialog } from "primereact/dialog";
import { type DeleteM3UFileRequest } from "../store/iptvApi";
import { DeleteM3UFile } from "../store/signlar_functions";
import { Toast } from 'primereact/toast';

const M3UFileRemoveDialog = (props: M3UFileRemoveDialogProps) => {
  const toast = React.useRef<Toast>(null);
  const [deleteDialog, setDeleteDialog] = React.useState<boolean>();
  const [deleteFSFile, setDeleteFSFile] = React.useState<boolean>(true);

  const deleteFile = async () => {
    if (!props.selectedFile) {
      toast.current?.show({
        detail: `M3U File Delete Failed`,
        life: 3000,
        severity: 'error',
        summary: 'Error',
      });
      return;
    }

    const tosend = {} as DeleteM3UFileRequest;

    tosend.id = props.selectedFile.id;
    tosend.deleteFile = deleteFSFile;

    await DeleteM3UFile(tosend)
      .then((returnData) => {
        if (toast.current) {
          if (returnData) {
            toast.current.show({
              detail: `M3U File Delete Successful`,
              life: 3000,
              severity: 'success',
              summary: 'Successful',
            });
          } else {
            toast.current.show({
              detail: `M3U File Delete Failed`,
              life: 3000,
              severity: 'error',
              summary: 'Error',
            });
          }
        }
      }).catch((e) => {
        if (toast.current) {
          toast.current.show({
            detail: `M3U File Delete Failed`,
            life: 3000,
            severity: 'error',
            summary: 'Error ' + e.message,
          });
        }
      });

    props.onFileDeleted();
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
        </div>
      </Dialog>
      <Button
        disabled={props.selectedFile === undefined || props.selectedFile.name === undefined || props.selectedFile.name === ''}
        icon="pi pi-minus"
        onClick={() => setDeleteDialog(true)}
        rounded
        severity="danger"
        size="small"
        tooltip="Delete M3U File"
        tooltipOptions={getTopToolOptions}
      />
    </>

  );
}

M3UFileRemoveDialog.displayName = 'M3UFileRemoveDialog';

type M3UFileRemoveDialogProps = {
  onFileDeleted: () => void;
  selectedFile?: StreamMasterApi.M3UFilesDto;
};

export default React.memo(M3UFileRemoveDialog);
