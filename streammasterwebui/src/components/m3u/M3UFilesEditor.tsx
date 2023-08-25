import { Button } from "primereact/button";
import { ConfirmDialog } from "primereact/confirmdialog";
import { Toast } from "primereact/toast";
import { useRef, useState, useMemo, useCallback, memo } from "react";
import { getTopToolOptions } from "../../common/common";
import { type M3UFileDto, type RefreshM3UFileRequest } from "../../store/iptvApi";
import { RefreshM3UFile } from "../../store/signlar_functions";
import FileDialog from "../FileDialog";
import M3UFileRemoveDialog from "./M3UFileRemoveDialog";
import M3UFilesDataSelector from "../dataSelectors/M3UFilesDataSelector";

const M3UFilesEditor = (props: M3UFilesEditorProps) => {
  const toast = useRef<Toast>(null);

  const [selectedM3UFile, setSelectedM3UFile] = useState<M3UFileDto>({} as M3UFileDto)

  const [reloadDialogVisible, setReloadDialogVisible] = useState<boolean>(false);

  useMemo(() => {
    if (props.value?.id !== undefined) {
      setSelectedM3UFile(props.value);
    }
  }, [props]);

  const M3UFileChanged = useCallback((e: M3UFileDto): void => {
    if (!e || e === undefined || e.id === undefined) {
      return;
    }

    setSelectedM3UFile(e);
    props?.onClick?.(e);

  }, [props, setSelectedM3UFile]);

  const acceptReload = useCallback(async () => {

    if (!selectedM3UFile) {
      return;
    }

    const data = {} as RefreshM3UFileRequest;

    data.id = selectedM3UFile.id;

    await RefreshM3UFile(data)
      .then(() => {
        if (toast.current) {

          toast.current.show({
            detail: `M3U Reload Successful`,
            life: 3000,
            severity: 'success',
            summary: 'Successful',
          });

        }
      }).catch((e) => {
        if (toast.current) {
          toast.current.show({
            detail: `M3U Reload Failed`,
            life: 3000,
            severity: 'error',
            summary: 'Error ' + e.message,
          });
        }
      });
  }, [selectedM3UFile]);

  return (
    <>
      <Toast position="bottom-right" ref={toast} />

      <ConfirmDialog
        accept={acceptReload}
        header="Confirmation"
        icon="pi pi-exclamation-triangle"
        message={'Are you sure you want to reload "' + selectedM3UFile.name + '" ?'}
        onHide={() => setReloadDialogVisible(false)}
        reject={() => { }}
        visible={reloadDialogVisible}
      />
      <div className='m3uFilesEditor flex flex-column col-12 flex-shrink-0 '>
        <div className='flex justify-content-between align-items-center mb-1'>
          <span className='m-0 p-0 gap-1' style={{ color: '#FE7600' }}>M3U Files</span>
          <div className='m-0 p-0 flex gap-1'>
            <Button
              disabled={selectedM3UFile.url === undefined || selectedM3UFile.url === ''}
              icon="pi pi-sync"
              onClick={() => setReloadDialogVisible(true)}
              rounded
              size="small"
              tooltip="Refresh Data"
              tooltipOptions={getTopToolOptions}
            />

            <FileDialog
              fileType="m3u"
              onHide={() => { }}
            />

            <M3UFileRemoveDialog selectedFile={selectedM3UFile} />

          </div>
        </div>

        <M3UFilesDataSelector
          onChange={(e) => M3UFileChanged(e)}
          value={selectedM3UFile}
        />

      </div>
    </>
  );
}

M3UFilesEditor.displayName = 'M3UFilesEditor';

M3UFilesEditor.defaultProps = {

};

export type M3UFilesEditorProps = {
  onClick?: (e: M3UFileDto) => void;

  value?: M3UFileDto | undefined;
};


export default memo(M3UFilesEditor);
