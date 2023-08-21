
import '../../styles/EPGFilesEditor.css';
import { Button } from 'primereact/button';
import { ConfirmDialog } from 'primereact/confirmdialog';
import EPGFilesDataSelector from './EPGFilesDataSelector';
import EPGFileRemoveDialog from './EPGFileRemoveDialog';
import { Toast } from 'primereact/toast';
import { useRef, useState, useMemo, useCallback, memo } from 'react';
import { getTopToolOptions } from '../../common/common';
import { type EpgFilesDto, type RefreshEpgFileRequest } from '../../store/iptvApi';
import FileDialog from '../FileDialog';
import { RefreshEPGFile } from '../../store/signlar_functions';

const EPGFilesEditor = (props: EPGFilesEditorProps) => {
  const toast = useRef<Toast>(null);

  const [selectedEPGFile, setSelectedEPGFile] = useState<EpgFilesDto>({} as EpgFilesDto)

  const [reloadDialogVisible, setReloadDialogVisible] = useState<boolean>(false);

  useMemo(() => {
    if (props.value?.id !== undefined) {
      setSelectedEPGFile(props.value);
    }
  }, [props]);

  const EPGFileChanged = useCallback((e: EpgFilesDto): void => {
    if (!e || e === undefined || e.id === undefined) {
      return;
    }

    setSelectedEPGFile(e);
    props?.onClick?.(e);

  }, [props, setSelectedEPGFile]);

  const acceptReload = useCallback(async () => {

    if (!selectedEPGFile) {
      return;
    }

    const data = {} as RefreshEpgFileRequest;

    data.id = selectedEPGFile.id;

    await RefreshEPGFile(data)
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
  onClick?: (e: EpgFilesDto) => void;
  value?: EpgFilesDto | undefined;
};


export default memo(EPGFilesEditor);
