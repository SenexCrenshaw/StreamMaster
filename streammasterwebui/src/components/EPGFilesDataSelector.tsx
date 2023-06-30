import React from 'react';
import * as StreamMasterApi from '../store/iptvApi';
import * as Hub from "../store/signlar_functions";

import DataSelector from '../features/dataSelector/DataSelector';
import { formatJSONDateString, getTopToolOptions } from '../common/common';
import { Toast } from 'primereact/toast';
import { type CheckboxChangeEvent } from 'primereact/checkbox';
import { Checkbox } from 'primereact/checkbox';
import NumberEditorBodyTemplate from './NumberEditorBodyTemplate';
import StringEditorBodyTemplate from './StringEditorBodyTemplate';
import { type ColumnMeta } from '../features/dataSelector/DataSelectorTypes';

const EPGFilesDataSelector = (props: EPGFilesDataSelectorProps) => {
  const toast = React.useRef<Toast>(null);

  const [selectedEPGFile, setSelectedEPGFile] = React.useState<StreamMasterApi.EpgFilesDto>({} as StreamMasterApi.EpgFilesDto);

  const epgFilesQuery = StreamMasterApi.useEpgFilesGetEpgFilesQuery();

  React.useMemo(() => {
    if (props.value?.id !== undefined) {

      setSelectedEPGFile(props.value);
    }
  }, [props.value, setSelectedEPGFile]);

  const SetSelectedEPGFileChanged = React.useCallback((data: StreamMasterApi.EpgFilesDto) => {
    if (!data) {
      return;
    }

    if (props.value === undefined || props.value.id === data.id) {
      return;
    }

    setSelectedEPGFile(data);

    props.onChange?.(data);
  }, [props]);

  const onEPGUpdateClick = React.useCallback(async (id: number, auto?: boolean | null, days?: number | null, name?: string | null) => {

    if (id < 1) {
      return;
    }

    if (!auto && !days && !name) {
      return;
    }

    const tosend = {} as StreamMasterApi.UpdateEpgFileRequest;

    tosend.id = id;
    if (auto) {
      tosend.autoUpdate = auto;
    }

    if (days) {
      tosend.daysToUpdate = days;
    }

    if (name) {
      tosend.name = name;
    }

    await Hub.UpdateEPGFile(tosend)
      .then((returnData) => {
        if (toast.current) {
          if (returnData) {
            toast.current.show({
              detail: `EPG File Update Successful`,
              life: 3000,
              severity: 'success',
              summary: 'Successful',
            });
          } else {
            toast.current.show({
              detail: `EPG File Update Failed`,
              life: 3000,
              severity: 'error',
              summary: 'Error',
            });
          }
        }
      }).catch((e) => {
        if (toast.current) {
          toast.current.show({
            detail: `EPG File Update Failed`,
            life: 3000,
            severity: 'error',
            summary: 'Error ' + e.message,
          });
        }
      });

  }, [toast]);

  const lastDownloadedTemplate = React.useCallback((rowData: StreamMasterApi.EpgFilesDto) => {
    if (rowData.id === 0) {
      return (<div />);
    }

    return (
      <div className="flex justify-content-center ">
        {formatJSONDateString(rowData.lastDownloaded ?? '')}
      </div>
    );
  }, []);

  const nameEditorBodyTemplate = React.useCallback((rowData: StreamMasterApi.EpgFilesDto) => {
    if (rowData.id === 0) {
      return (
        <div className='p-0 relative'
          style={{
            ...{
              backgroundColor: 'var(--mask-bg)',
              overflow: 'hidden',
              textOverflow: 'ellipsis',
              whiteSpace: 'nowrap',
            },
          }}
        >
          {rowData.name}
        </div>
      )
    }

    return (
      <StringEditorBodyTemplate
        onChange={async (e) => {
          await onEPGUpdateClick(rowData.id, null, null, e)
        }}

        value={rowData.name}
      />
    )
  }, [onEPGUpdateClick]);

  const programmeCountTemplate = React.useCallback((rowData: StreamMasterApi.EpgFilesDto) => {
    if (rowData.id === 0) {
      return (<div />);
    }

    return (
      <div>{rowData.programmeCount}</div>
    );
  }, []);

  const targetActionBodyTemplate = React.useCallback((rowData: StreamMasterApi.EpgFilesDto) => {
    if (rowData.id === 0) {
      return (<div />);
    }

    return (
      <div className="dataselector p-inputgroup align-items-center">
        <Checkbox
          checked={rowData.autoUpdate}
          onChange={async (e: CheckboxChangeEvent) => {
            console.log(e.checked);
            await onEPGUpdateClick(rowData.id, e.checked ?? false);
          }
          }
          tooltip="Enable Auto Update"
          tooltipOptions={getTopToolOptions}
        />

        <NumberEditorBodyTemplate
          onChange={async (e) => {
            await onEPGUpdateClick(rowData.id, null, e);
          }}
          suffix=' days'
          value={rowData.daysToUpdate}
        />

      </div>
    );
  }, [onEPGUpdateClick]);

  const sourceColumns = React.useMemo((): ColumnMeta[] => {
    return [
      { bodyTemplate: nameEditorBodyTemplate, field: 'name', filter: true, header: 'Name', sortable: true },
      { bodyTemplate: lastDownloadedTemplate, field: 'lastDownloaded', header: 'Downloaded', sortable: true },
      { bodyTemplate: programmeCountTemplate, field: 'programmeCount', header: 'Programmes', sortable: true },
      {
        align: 'center', bodyTemplate: targetActionBodyTemplate, field: 'autoUpdate',
        style: {
          maxWidth: '8rem',
          width: '8rem',
        } as React.CSSProperties,
      },

    ]
  }, [lastDownloadedTemplate, nameEditorBodyTemplate, programmeCountTemplate, targetActionBodyTemplate]);

  return (
    <>
      <Toast position="bottom-right" ref={toast} />

      <DataSelector
        columns={sourceColumns}
        dataSource={epgFilesQuery.data}
        emptyMessage="No EPG Files"
        globalSearchEnabled={false}
        id='epgfilesdataselector'
        isLoading={epgFilesQuery.isLoading}
        onSelectionChange={(e) =>
          SetSelectedEPGFileChanged(e as StreamMasterApi.EpgFilesDto)
        }
        selection={selectedEPGFile}
        style={{ height: 'calc(50vh - 40px)' }}
      />
    </>
  );
};

EPGFilesDataSelector.displayName = 'EPGFilesDataSelector';
type EPGFilesDataSelectorProps = {
  onChange?: (value: StreamMasterApi.EpgFilesDto) => void;
  value?: StreamMasterApi.EpgFilesDto | undefined;
};

export default React.memo(EPGFilesDataSelector);
