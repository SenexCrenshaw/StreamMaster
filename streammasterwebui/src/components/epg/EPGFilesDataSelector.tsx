import { type CheckboxChangeEvent } from "primereact/checkbox";
import { Checkbox } from "primereact/checkbox";
import { Toast } from "primereact/toast";
import { type CSSProperties } from "react";
import { useRef, useState, useCallback, useMemo, memo } from "react";
import { formatJSONDateString, getTopToolOptions } from "../../common/common";
import { type EpgFilesGetEpgFilesApiArg, type EpgFilesDto, type UpdateEpgFileRequest, type M3UFileDto } from "../../store/iptvApi";
import { useEpgFilesUpdateEpgFileMutation, useEpgFilesGetEpgFilesQuery } from "../../store/iptvApi";
import NumberEditorBodyTemplate from "../NumberEditorBodyTemplate";
import StringEditorBodyTemplate from "../StringEditorBodyTemplate";
import DataSelector from "../dataSelector/DataSelector";
import { type ColumnMeta } from "../dataSelector/DataSelectorTypes";

const EPGFilesDataSelector = (props: EPGFilesDataSelectorProps) => {
  const toast = useRef<Toast>(null);

  // const [selectedEPGFile, setSelectedEPGFile] = useState<EpgFilesDto>({} as EpgFilesDto);
  const [pageSize, setPageSize] = useState<number>(25);
  const [pageNumber, setPageNumber] = useState<number>(1);

  const [epgFilesUpdateEpgFileMutation] = useEpgFilesUpdateEpgFileMutation();
  const epgFilesQuery = useEpgFilesGetEpgFilesQuery({ pageNumber: pageNumber === 0 ? 1 : pageNumber, pageSize: pageSize } as EpgFilesGetEpgFilesApiArg);

  // useMemo(() => {
  //   if (props.value?.id !== undefined) {

  //     setSelectedEPGFile(props.value);
  //   }
  // }, [props.value, setSelectedEPGFile]);

  const SetSelectedEPGFileChanged = useCallback((data: EpgFilesDto) => {
    if (!data) {
      return;
    }

    if (props.value === undefined || props.value.id === data.id) {
      return;
    }

    // setSelectedEPGFile(data);

    props.onChange?.(data);
  }, [props]);

  const onEPGUpdateClick = useCallback(async (id: number, auto?: boolean | null, hours?: number | null, name?: string | null, url?: string | null) => {

    if (id < 1) {
      return;
    }

    if (auto === undefined && !url && !hours && !name) {
      return;
    }

    const tosend = {} as UpdateEpgFileRequest;

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

    if (url !== undefined) {
      tosend.url = url;
    }

    await epgFilesUpdateEpgFileMutation(tosend)
      .then(() => {
        if (toast.current) {

          toast.current.show({
            detail: `EPG File Update Successful`,
            life: 3000,
            severity: 'success',
            summary: 'Successful',
          });

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

  }, [epgFilesUpdateEpgFileMutation]);

  const lastDownloadedTemplate = useCallback((rowData: EpgFilesDto) => {
    if (rowData.id === 0) {
      return (<div />);
    }

    return (
      <div className="flex justify-content-center ">
        {formatJSONDateString(rowData.lastDownloaded ?? '')}
      </div>
    );
  }, []);

  const nameEditorBodyTemplate = useCallback((rowData: EpgFilesDto) => {
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

  const programmeCountTemplate = useCallback((rowData: EpgFilesDto) => {
    if (rowData.id === 0) {
      return (<div />);
    }

    return (
      <div>{rowData.programmeCount}</div>
    );
  }, []);

  const targetActionBodyTemplate = useCallback((rowData: EpgFilesDto) => {
    if (rowData.id === 0) {
      return (<div />);
    }

    return (
      <div className="dataselector p-inputgroup align-items-center">
        <Checkbox
          checked={rowData.autoUpdate}
          onChange={async (e: CheckboxChangeEvent) => {
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
          suffix=' hours'
          value={rowData.hoursToUpdate}
        />

      </div>
    );
  }, [onEPGUpdateClick]);

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
          await onEPGUpdateClick(rowData.id, null, null, null, e)
        }}
        tooltip={rowData.url}
        value={rowData.url}
      />
    )
  }, [onEPGUpdateClick]);

  const sourceColumns = useMemo((): ColumnMeta[] => {
    return [
      { bodyTemplate: nameEditorBodyTemplate, field: 'name', filter: true, header: 'Name', sortable: true },
      { bodyTemplate: lastDownloadedTemplate, field: 'lastDownloaded', header: 'Downloaded', sortable: true },
      { bodyTemplate: programmeCountTemplate, field: 'programmeCount', header: 'Programmes', sortable: true },
      { bodyTemplate: urlEditorBodyTemplate, field: 'url', sortable: true },
      {
        align: 'center', bodyTemplate: targetActionBodyTemplate, field: 'autoUpdate',
        style: {
          maxWidth: '8rem',
          width: '8rem',
        } as CSSProperties,
      },

    ]
  }, [lastDownloadedTemplate, nameEditorBodyTemplate, programmeCountTemplate, targetActionBodyTemplate, urlEditorBodyTemplate]);

  return (
    <>
      <Toast position="bottom-right" ref={toast} />
      <DataSelector
        columns={sourceColumns}
        dataSource={epgFilesQuery.data}
        emptyMessage="No EPG Files"
        id='epgfilesdataselector'
        isLoading={epgFilesQuery.isLoading}
        onPage={(pageInfo) => {
          if (pageInfo.page !== undefined) {
            setPageNumber(pageInfo.page + 1);
          }

          if (pageInfo.rows !== undefined) {
            setPageSize(pageInfo.rows);
          }
        }}
        onSelectionChange={(e) =>
          SetSelectedEPGFileChanged(e as EpgFilesDto)
        }
        style={{ height: 'calc(50vh - 40px)' }}
      />
    </>
  );
};

EPGFilesDataSelector.displayName = 'EPGFilesDataSelector';
type EPGFilesDataSelectorProps = {
  onChange?: (value: EpgFilesDto) => void;
  value?: EpgFilesDto | undefined;
};

export default memo(EPGFilesDataSelector);
