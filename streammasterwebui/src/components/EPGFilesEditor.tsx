
import '../styles/EPGFilesEditor.css';
import { Button } from 'primereact/button';
import { ConfirmDialog } from 'primereact/confirmdialog';
import type * as StreamMasterApi from '../store/iptvApi';
import React from 'react';
import * as Hub from "../store/signlar_functions";
import { Toast } from 'primereact/toast';
import FileDialog from './FileDialog';
import { getTopToolOptions } from '../common/common';
import EPGFilesDataSelector from './EPGFilesDataSelector';
import EPGFileRemoveDialog from './EPGFileRemoveDialog';

const EPGFilesEditor = (props: EPGFilesEditorProps) => {
  const toast = React.useRef<Toast>(null);

  const [selectedEPGFile, setSelectedEPGFile] = React.useState<StreamMasterApi.EpgFilesDto>({} as StreamMasterApi.EpgFilesDto)

  const [reloadDialogVisible, setReloadDialogVisible] = React.useState<boolean>(false);

  React.useMemo(() => {
    if (props.value?.id !== undefined) {
      setSelectedEPGFile(props.value);
    }
  }, [props]);

  const EPGFileChanged = React.useCallback((e: StreamMasterApi.EpgFilesDto): void => {
    if (!e || e === undefined || e.id === undefined) {
      return;
    }

    setSelectedEPGFile(e);
    props?.onClick?.(e);

  }, [props, setSelectedEPGFile]);

  const acceptReload = React.useCallback(async () => {

    if (!selectedEPGFile) {
      return;
    }

    const data = {} as StreamMasterApi.RefreshEpgFileRequest;

    data.id = selectedEPGFile.id;

    await Hub.RefreshEPGFile(data)
      .then(() => {
        if (toast.current) {

          toast.current.show({
            detail: `EPG Reload Successful`,
            life: 3000,
            severity: 'success',
            summary: 'Successful',
          });

        }
      }).catch((e) => {
        if (toast.current) {
          toast.current.show({
            detail: `EPG Reload Failed`,
            life: 3000,
            severity: 'error',
            summary: 'Error ' + e.message,
          });
        }
      });
  }, [selectedEPGFile]);

  return (
    <>
      <Toast position="bottom-right" ref={toast} />

      <ConfirmDialog
        accept={acceptReload}
        header="Confirmation"
        icon="pi pi-exclamation-triangle"
        message={'Are you sure you want to reload "' + selectedEPGFile.name + '" ?'}
        onHide={() => setReloadDialogVisible(false)}
        reject={() => { }}
        visible={reloadDialogVisible}
      />
      <div className='ePGFilesEditor flex flex-column col-12 flex-shrink-0'>
        <div className='flex justify-content-between align-items-center mb-1'>
          <span className='m-0 p-0 gap-1' style={{ color: '#FE7600' }}>EPG Files</span>
          <div className='m-0 p-0 flex gap-1'>
            <Button
              disabled={selectedEPGFile.url === undefined || selectedEPGFile.url === ''}
              icon="pi pi-sync"
              onClick={() => setReloadDialogVisible(true)}
              rounded
              size="small"
              tooltip="Refresh Data"
              tooltipOptions={getTopToolOptions}
            />

            <FileDialog

              fileType="epg"
              onHide={() => { }}
            />

            <EPGFileRemoveDialog selectedFile={selectedEPGFile} />

          </div>
        </div>

        <EPGFilesDataSelector
          onChange={(e) => EPGFileChanged(e)}
          value={selectedEPGFile}
        />

      </div>
    </>
  );
}

EPGFilesEditor.displayName = 'EPGFilesEditor';

EPGFilesEditor.defaultProps = {

};

export type EPGFilesEditorProps = {
  onClick?: (e: StreamMasterApi.EpgFilesDto) => void;
  value?: StreamMasterApi.EpgFilesDto | undefined;
};


export default React.memo(EPGFilesEditor);
