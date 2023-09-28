
import { formatJSONDateString, getTopToolOptions } from "@/lib/common/common";
import { useM3UFilesGetPagedM3UFilesQuery, type M3UFileDto, type UpdateM3UFileRequest } from '@/lib/iptvApi';
import { UpdateM3UFile } from "@/lib/smAPI/M3UFiles/M3UFilesMutateAPI";
import { Checkbox, type CheckboxChangeEvent } from "primereact/checkbox";
import { Toast } from "primereact/toast";
import { memo, useCallback, useMemo, useRef, type CSSProperties } from "react";
import NumberEditorBodyTemplate from "../NumberEditorBodyTemplate";
import StringEditorBodyTemplate from "../StringEditorBodyTemplate";
import DataSelector from "../dataSelector/DataSelector";
import { type ColumnMeta } from "../dataSelector/DataSelectorTypes";
import M3UFileRefreshDialog from "./M3UFileRefreshDialog";
import M3UFileRemoveDialog from "./M3UFileRemoveDialog";


const M3UFilesDataSelector = () => {
  const toast = useRef<Toast>(null);

  const onM3UUpdateClick = useCallback(async (id: number, auto?: boolean | null, hours?: number | null, maxStreams?: number | null, name?: string | null, url?: string | null, startingChannelNumber?: number | null) => {

    if (id < 1) {
      return;
    }

    if (auto === undefined && !hours && !maxStreams && !name && !url && !startingChannelNumber) {
      return;
    }

    const tosend = {} as UpdateM3UFileRequest;

    tosend.id = id;

    if (auto !== undefined) {
      tosend.autoUpdate = auto;
    }

    if (hours) {
      tosend.hoursToUpdate = hours;
    }

    if (name) {
      tosend.name = name;
    }

    if (maxStreams) {
      tosend.maxStreamCount = maxStreams;
    }

    if (url) {
      tosend.url = url;
    }

    if (startingChannelNumber) {
      tosend.startingChannelNumber = startingChannelNumber;
    }

    await UpdateM3UFile(tosend)
      .then(() => {
        if (toast.current) {

          toast.current.show({
            detail: `M3U File Update Successful`,
            life: 3000,
            severity: 'success',
            summary: 'Successful',
          });

        }
      }).catch((e) => {
        if (toast.current) {
          toast.current.show({
            detail: `M3U File Update Failed`,
            life: 3000,
            severity: 'error',
            summary: 'Error ' + e.message,
          });
        }
      });

  }, [toast]);

  const lastDownloadedTemplate = useCallback((rowData: M3UFileDto) => {
    if (rowData.id === 0) {
      return (<div />);
    }

    return (
      <div className="flex justify-content-center ">
        {formatJSONDateString(rowData.lastDownloaded ?? '')}
      </div>
    );
  }, []);

  const nameEditorBodyTemplate = useCallback((rowData: M3UFileDto) => {
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
          await onM3UUpdateClick(rowData.id, null, null, null, e)
        }}

        value={rowData.name}
      />
    )
  }, [onM3UUpdateClick]);

  const urlEditorBodyTemplate = useCallback((rowData: M3UFileDto) => {
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
          {rowData.url}
        </div>
      )
    }

    return (
      <StringEditorBodyTemplate
        onChange={async (e) => {
          await onM3UUpdateClick(rowData.id, null, null, null, null, e)
        }}
        tooltip={rowData.url}
        value={rowData.url}
      />
    )
  }, [onM3UUpdateClick]);

  const maxStreamCountTemplate = useCallback((rowData: M3UFileDto) => {
    if (rowData.id === 0) {
      return (<div />);
    }

    return (
      <NumberEditorBodyTemplate
        onChange={async (e) => {
          await onM3UUpdateClick(rowData.id, null, null, e);
        }}
        // prefix='Max '
        // suffix=' streams'

        value={rowData.maxStreamCount}
      />
    );
  }, [onM3UUpdateClick]);

  const startingChannelNumberTemplate = useCallback((rowData: M3UFileDto) => {
    if (rowData.id === 0) {
      return (<div />);
    }

    return (
      <NumberEditorBodyTemplate
        onChange={async (e) => {
          await onM3UUpdateClick(rowData.id, null, null, null, null, null, e);
        }}
        // prefix='Starting Channel Number '
        // suffix=' streams'

        value={rowData.startingChannelNumber}
      />
    );
  }, [onM3UUpdateClick]);


  const stationCountTemplate = useCallback((rowData: M3UFileDto) => {
    if (rowData.id === 0) {
      return (<div />);
    }

    return (
      <div>{rowData.stationCount}</div>
    );
  }, []);

  const targetActionBodyTemplate = useCallback((rowData: M3UFileDto) => {
    if (rowData.id === 0) {
      return (<div />);
    }

    return (
      <div className='flex grid p-0 justify-content-end align-items-center'>
        <div className='col-6 p-0 justify-content-end align-items-center'>
          <Checkbox
            checked={rowData.autoUpdate}
            onChange={async (e: CheckboxChangeEvent) => {
              await onM3UUpdateClick(rowData.id, e.checked ?? false);
            }
            }
            tooltip="Enable Auto Update"
            tooltipOptions={getTopToolOptions}
          />

          <NumberEditorBodyTemplate
            onChange={async (e) => {
              await onM3UUpdateClick(rowData.id, rowData.autoUpdate, e, rowData.maxStreamCount ?? 0);
            }}
            suffix=' hours'
            value={rowData.hoursToUpdate}
          />
        </div>
        <div className='col-6 p-0 justify-content-end align-items-center'>
          <M3UFileRefreshDialog selectedFile={rowData} />
          <M3UFileRemoveDialog selectedFile={rowData} />
        </div>
      </div>
    );
  }, [onM3UUpdateClick]);

  const sourceColumns = useMemo((): ColumnMeta[] => {
    return [
      { bodyTemplate: nameEditorBodyTemplate, field: 'name', filter: true, header: 'Name', sortable: true },
      { bodyTemplate: lastDownloadedTemplate, field: 'lastDownloaded', header: 'Downloaded', sortable: true },
      {
        bodyTemplate: startingChannelNumberTemplate, field: 'startingChannelNumber', header: 'Start Ch #', sortable: true, style: {
          maxWidth: '8rem',
          width: '8rem',
        } as CSSProperties,
      },
      {
        bodyTemplate: maxStreamCountTemplate, field: 'maxStreamCount', header: 'Max Streams', sortable: true, style: {
          maxWidth: '8rem',
          width: '8rem',
        } as CSSProperties,
      },
      { bodyTemplate: stationCountTemplate, field: 'stationCount', header: 'Streams', sortable: true },
      { bodyTemplate: urlEditorBodyTemplate, field: 'url', sortable: true },
      {
        align: 'center', bodyTemplate: targetActionBodyTemplate, field: 'autoUpdate',
        style: {
          maxWidth: '8rem',
          width: '8rem',
        } as CSSProperties,
      },

    ]
  }, [lastDownloadedTemplate, maxStreamCountTemplate, nameEditorBodyTemplate, startingChannelNumberTemplate, stationCountTemplate, targetActionBodyTemplate, urlEditorBodyTemplate]);

  return (
    <>
      <Toast position="bottom-right" ref={toast} />

      <DataSelector
        columns={sourceColumns}
        emptyMessage="No M3U Files"
        id='m3ufilesdataselector'
        queryFilter={useM3UFilesGetPagedM3UFilesQuery}
        selectedItemsKey='selectSelectedM3UFileDtoItems'
        style={{ height: 'calc(50vh - 40px)' }}
      />
    </>
  );
};

M3UFilesDataSelector.displayName = 'M3UFilesDataSelector';

export default memo(M3UFilesDataSelector);
