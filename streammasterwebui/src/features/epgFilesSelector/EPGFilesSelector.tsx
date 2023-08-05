import React from 'react';
import { formatJSONDateString, getTopToolOptions } from '../../common/common';
import * as StreamMasterApi from '../../store/iptvApi';

import DataSelector from '../dataSelector/DataSelector';
import { type CheckboxChangeEvent } from 'primereact/checkbox';
import { Checkbox } from 'primereact/checkbox';
import NumberEditorBodyTemplate from '../../components/NumberEditorBodyTemplate';
import { Toast } from 'primereact/toast';
import * as Hub from "../../store/signlar_functions";
import StringEditorBodyTemplate from '../../components/StringEditorBodyTemplate';
import { type ColumnMeta } from '../dataSelector/DataSelectorTypes';

export const EPGFilesSelector = (props: EPGFilesSelectorProps) => {
  const toast = React.useRef<Toast>(null);
  const [selectedFile, setSelectedFile] = React.useState<StreamMasterApi.EpgFilesDto>({} as StreamMasterApi.EpgFilesDto);

  React.useMemo(() => {
    if (props.SetSelection?.id !== undefined) {
      setSelectedFile(props.SetSelection);
    }
  }, [props.SetSelection]);

  const epgFiles = StreamMasterApi.useEpgFilesGetEpgFilesQuery();

  const SetSelectedFileChanged = (data: StreamMasterApi.EpgFilesDto) => {
    if (!data) {
      return;
    }

    setSelectedFile(data);

    props.onChange?.(data);
  };

  const lastDownloadedTemplate = React.useCallback((rowData: StreamMasterApi.EpgFilesDto) => {
    return (
      <div className="flex justify-content-center ">
        {formatJSONDateString(rowData.lastDownloaded ?? '')}
      </div>
    );
  }, []);

  const onEPGUpdateClick = React.useCallback(async (id: number, auto?: boolean | null, hours?: number | null, name?: string | null) => {

    if (id < 1) {
      return;
    }

    if (!auto && !hours && !name) {
      return;
    }

    const tosend = {} as StreamMasterApi.UpdateEpgFileRequest;

    tosend.id = id;
    if (auto) {
      tosend.autoUpdate = auto;
    }

    if (hours) {
      tosend.hoursToUpdate = hours;
    }

    if (name) {
      tosend.name = name;
    }

    await Hub.UpdateEPGFile(tosend)
      .then(() => {
        if (toast.current) {

          toast.current.show({
            detail: `EPG Update Successful`,
            life: 3000,
            severity: 'success',
            summary: 'Successful',
          });

        }
      }).catch((e) => {
        if (toast.current) {
          toast.current.show({
            detail: `EPG Update Failed`,
            life: 3000,
            severity: 'error',
            summary: 'Error ' + e.message,
          });
        }
      });

  }, [toast]);

  const targetActionBodyTemplate = React.useCallback((data: StreamMasterApi.M3UFileDto) => {
    return (
      <div className="dataselector p-inputgroup align-items-center">
        <Checkbox
          checked={data.autoUpdate}
          onChange={async (e: CheckboxChangeEvent) => {
            console.log(e.checked);
            await onEPGUpdateClick(data.id, e.checked ?? false);
          }
          }
          tooltip="Enable Auto Update"
          tooltipOptions={getTopToolOptions}
        />

        <NumberEditorBodyTemplate
          onChange={async (e) => {
            await onEPGUpdateClick(data.id, null, e);
          }}
          suffix=' hours'
          value={data.hoursToUpdate}
        />

      </div>
    );
  }, [onEPGUpdateClick]);

  const nameEditorBodyTemplate = React.useCallback((data: StreamMasterApi.EpgFilesDto) => {
    return (
      <StringEditorBodyTemplate
        onChange={async (e) => {
          await onEPGUpdateClick(data.id, null, null, e);
        }}

        value={data.name}
      />
    )
  }, [onEPGUpdateClick]);

  const sourceColumns = React.useMemo((): ColumnMeta[] => {
    return [
      { bodyTemplate: nameEditorBodyTemplate, field: 'name', filter: true, header: 'Name', sortable: true },
      { bodyTemplate: lastDownloadedTemplate, field: 'lastDownloaded', header: 'Downloaded', sortable: true },
      { field: 'programmeCount', header: 'Programs', sortable: true },
      {
        align: 'center', bodyTemplate: targetActionBodyTemplate, field: 'autoUpdate',
        style: {
          maxWidth: '8rem',
          width: '8rem',
        } as React.CSSProperties,
      },
    ]
  }, [lastDownloadedTemplate, nameEditorBodyTemplate, targetActionBodyTemplate]);

  return (
    <>
      <Toast position="bottom-right" ref={toast} />
      <DataSelector
        columns={sourceColumns}
        dataSource={epgFiles.data}
        emptyMessage="No EPG Files"
        globalSearchEnabled={false}
        id='epgfilesselector'
        isLoading={epgFiles.isLoading}
        onSelectionChange={(e) => SetSelectedFileChanged(e as StreamMasterApi.EpgFilesDto)}
        selection={selectedFile}
      />
    </>
  );
};

EPGFilesSelector.displayName = 'EPGFilesSelector';
type EPGFilesSelectorProps = {
  SetSelection?: StreamMasterApi.EpgFilesDto;

  onChange?: ((value: StreamMasterApi.EpgFilesDto) => void) | null;
};

export default React.memo(EPGFilesSelector);
