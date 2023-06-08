
import '../styles/M3UFilesEditor.css';
import { Button } from 'primereact/button';
import { ConfirmDialog } from 'primereact/confirmdialog';
import type * as StreamMasterApi from '../store/iptvApi';
import React from 'react';
import { RefreshM3UFile } from '../store/signlar_functions';
import M3UFilesDataSelector from './M3UFilesDataSelector';
import { Toast } from 'primereact/toast';
import FileDialog from './FileDialog';
import M3UFileRemoveDialog from './M3UFileRemoveDialog';
import { getTopToolOptions } from '../common/common';

const M3UFilesEditor = (props: M3UFilesEditorProps) => {
  const toast = React.useRef<Toast>(null);

  const [selectedM3UFile, setSelectedM3UFile] = React.useState<StreamMasterApi.M3UFilesDto>({} as StreamMasterApi.M3UFilesDto)

  const [reloadDialogVisible, setReloadDialogVisible] = React.useState<boolean>(false);

  React.useMemo(() => {
    if (props.value?.id !== undefined) {
      setSelectedM3UFile(props.value);
    }
  }, [props]);

  const M3UFileChanged = React.useCallback((e: StreamMasterApi.M3UFilesDto): void => {
    if (!e || e === undefined || e.id === undefined) {
      return;
    }

    setSelectedM3UFile(e);
    props?.onClick?.(e);

  }, [props, setSelectedM3UFile]);

  const acceptReload = React.useCallback(async () => {

    if (!selectedM3UFile) {
      return;
    }

    const data = {} as StreamMasterApi.RefreshM3UFileRequest;

    data.m3UFileID = selectedM3UFile.id;

    await RefreshM3UFile(data)
      .then((returnData) => {
        if (toast.current) {
          if (returnData) {
            toast.current.show({
              detail: `M3U Reload Successful`,
              life: 3000,
              severity: 'success',
              summary: 'Successful',
            });
          } else {
            toast.current.show({
              detail: `M3U Reload Failed`,
              life: 3000,
              severity: 'error',
              summary: 'Error',
            });
          }
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
          <div className='m-0 p-0'>
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

            <M3UFileRemoveDialog onFileDeleted={() => setSelectedM3UFile({} as StreamMasterApi.M3UFilesDto)} selectedFile={selectedM3UFile} />

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
  onClick?: (e: StreamMasterApi.M3UFilesDto) => void;

  value?: StreamMasterApi.M3UFilesDto | undefined;
};


export default React.memo(M3UFilesEditor);
