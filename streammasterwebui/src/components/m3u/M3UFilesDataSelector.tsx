/* eslint-disable @typescript-eslint/no-unused-vars */
import { type CheckboxChangeEvent } from "primereact/checkbox";
import { Checkbox } from "primereact/checkbox";
import { useLocalStorage } from "primereact/hooks";
import { Toast } from "primereact/toast";
import { type CSSProperties } from "react";
import { useRef, useState, useMemo, useCallback, memo } from "react";
import { formatJSONDateString, getTopToolOptions } from "../../common/common";
import { type M3UFileDto, type M3UFilesGetM3UFilesApiArg, type UpdateM3UFileRequest } from "../../store/iptvApi";
import { useM3UFilesGetM3UFilesQuery } from "../../store/iptvApi";
import NumberEditorBodyTemplate from "../NumberEditorBodyTemplate";
import StringEditorBodyTemplate from "../StringEditorBodyTemplate";
import DataSelector from "../dataSelector/DataSelector";
import { type ColumnMeta } from "../dataSelector/DataSelectorTypes";
import { UpdateM3UFile } from "../../store/signlar_functions";


const M3UFilesDataSelector = (props: M3UFilesDataSelectorProps) => {
  const toast = useRef<Toast>(null);
  const [pageSize, setPageSize] = useState<number>(25);
  const [pageNumber, setPageNumber] = useState<number>(1);

  const [selectedM3UFile, setSelectedM3UFile] = useLocalStorage<M3UFileDto>({ id: 0, name: 'All' } as M3UFileDto, 'M3UFilesDataSelector-selectedM3UFile');
  const [orderBy, setOrderBy] = useState<string>('user_tvg_name');

  // const m3UFilesQuery = useM3UFilesGetM3UFilesQuery({ orderBy: orderBy, pageNumber: pageNumber === 0 ? 1 : pageNumber, pageSize: pageSize } as M3UFilesGetM3UFilesApiArg);

  useMemo(() => {
    if (props.value?.id !== undefined && selectedM3UFile !== undefined && props.value.id !== selectedM3UFile.id) {

      setSelectedM3UFile(props.value);
    }
  }, [props.value, selectedM3UFile, setSelectedM3UFile]);

  const SetSelectedM3UFileChanged = useCallback((data: M3UFileDto) => {
    if (!data) {
      return;
    }

    if (props.value === undefined || props.value.id === data.id) {
      return;
    }

    setSelectedM3UFile(data);

    props.onChange?.(data);
  }, [props, setSelectedM3UFile]);

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
      <div className="dataselector p-inputgroup align-items-center">
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

        onSelectionChange={(e) =>
          SetSelectedM3UFileChanged(e as M3UFileDto)
        }
        queryFilter={useM3UFilesGetM3UFilesQuery}
        style={{ height: 'calc(50vh - 40px)' }}
      />
    </>
  );
};

M3UFilesDataSelector.displayName = 'M3UFilesDataSelector';
type M3UFilesDataSelectorProps = {
  onChange?: (value: M3UFileDto) => void;
  value?: M3UFileDto | undefined;
};

export default memo(M3UFilesDataSelector);
