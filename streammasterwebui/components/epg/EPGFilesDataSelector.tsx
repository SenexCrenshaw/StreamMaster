import ColorEditor from '@components/ColorEditor';
import { formatJSONDateString, getTopToolOptions } from '@lib/common/common';
import { useEpgFilesGetPagedEpgFilesQuery, useEpgFilesUpdateEpgFileMutation, type EpgFileDto, type M3UFileDto, type UpdateEpgFileRequest } from '@lib/iptvApi';
import { Checkbox, type CheckboxChangeEvent } from 'primereact/checkbox';
import { Toast } from 'primereact/toast';
import { memo, useCallback, useMemo, useRef } from 'react';
import NumberEditorBodyTemplate from '../NumberEditorBodyTemplate';
import StringEditorBodyTemplate from '../StringEditorBodyTemplate';
import DataSelector from '../dataSelector/DataSelector';
import { type ColumnMeta } from '../dataSelector/DataSelectorTypes';
import EPGFileRefreshDialog from './EPGFileRefreshDialog';
import EPGFileRemoveDialog from './EPGFileRemoveDialog';
import EPGPreviewDialog from './EPGPreviewDialog';

const EPGFilesDataSelector = () => {
  interface EPGUpdateProperties {
    id: number;
    auto?: boolean | null;
    hours?: number | null;
    name?: string | null;
    url?: string | null;
    color?: string | null;
    epgNumber?: number | null;
  }

  const toast = useRef<Toast>(null);

  const [epgFilesUpdateEpgFileMutation] = useEpgFilesUpdateEpgFileMutation();

  const onEPGUpdateClick = useCallback(
    async (props: EPGUpdateProperties) => {
      const { id, ...restProperties } = props;

      // Check if all values of the rest of the properties are null or undefined
      if (Object.values(restProperties).every((value) => value === null || value === undefined)) {
        return;
      }

      const { auto, hours, color, name, url, epgNumber } = restProperties;

      if (id < 1) {
        return;
      }

      const tosend = {} as UpdateEpgFileRequest;

      tosend.id = id;

      if (auto !== undefined) {
        tosend.autoUpdate = auto;
      }

      if (color) {
        tosend.color = color;
      }

      if (hours) {
        tosend.hoursToUpdate = hours;
      }

      if (epgNumber !== null && epgNumber !== undefined) {
        tosend.epgNumber = epgNumber;
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
              detail: 'EPG File Update Successful',
              life: 3000,
              severity: 'success',
              summary: 'Successful'
            });
          }
        })
        .catch((error) => {
          if (toast.current) {
            toast.current.show({
              detail: 'EPG File Update Failed',
              life: 3000,
              severity: 'error',
              summary: `Error ${error.message}`
            });
          }
        });
    },
    [epgFilesUpdateEpgFileMutation]
  );

  const lastDownloadedTemplate = useCallback((rowData: EpgFileDto) => {
    if (rowData.id === 0) {
      return <div />;
    }

    return <div className="flex justify-content-center ">{formatJSONDateString(rowData.lastDownloaded ?? '')}</div>;
  }, []);

  const nameEditorBodyTemplate = useCallback(
    (rowData: EpgFileDto) => {
      if (rowData.id === 0) {
        return (
          <div
            className="p-0 relative"
            style={{
              backgroundColor: 'var(--mask-bg)',
              overflow: 'hidden',
              textOverflow: 'ellipsis',
              whiteSpace: 'nowrap'
            }}
          >
            {rowData.name}
          </div>
        );
      }

      return (
        <StringEditorBodyTemplate
          onChange={async (e) => {
            await onEPGUpdateClick({ id: rowData.id, name: e });
          }}
          value={rowData.name}
        />
      );
    },
    [onEPGUpdateClick]
  );

  const colorTemplate = useCallback(
    (rowData: EpgFileDto) => {
      if (rowData.id === 0) {
        return (
          <div
            className="p-0 relative"
            style={{
              backgroundColor: 'var(--mask-bg)',
              overflow: 'hidden',
              textOverflow: 'ellipsis',
              whiteSpace: 'nowrap'
            }}
          >
            {rowData.color}
          </div>
        );
      }

      return (
        <ColorEditor
          onChange={async (e) => {
            console.log(e);
            await onEPGUpdateClick({ id: rowData.id, color: e });
          }}
          color={rowData.color}
        />
      );
    },
    [onEPGUpdateClick]
  );

  const channelCountTemplate = useCallback((rowData: EpgFileDto) => {
    if (rowData.id === 0) {
      return <div />;
    }

    return <div className="flex p-0 m-0 justify-content-center align-items-center">{rowData.channelCount}</div>;
  }, []);

  const programmeCountTemplate = useCallback((rowData: EpgFileDto) => {
    if (rowData.id === 0) {
      return <div />;
    }

    return <div className="flex p-0 m-0 justify-content-center align-items-center">{rowData.programmeCount}</div>;
  }, []);

  const epgNumberBodyTemplate = useCallback(
    (rowData: EpgFileDto) => {
      if (rowData.id === 0) {
        return <div />;
      }

      return (
        <NumberEditorBodyTemplate
          onChange={async (e) => {
            await onEPGUpdateClick({ id: rowData.id, epgNumber: e });
          }}
          value={rowData.epgNumber}
        />
      );
    },
    [onEPGUpdateClick]
  );

  const actionBodyTemplate = useCallback(
    (rowData: EpgFileDto) => {
      if (rowData.id === 0) {
        return <div />;
      }

      return (
        <div className="flex grid p-0 justify-content-end align-items-center">
          <div className="col-8 flex p-0 justify-content-between align-items-center">
            <div className="col-8 flex p-0 justify-content-between align-items-center">
              <Checkbox
                checked={rowData.autoUpdate}
                onChange={async (e: CheckboxChangeEvent) => {
                  await onEPGUpdateClick({ id: rowData.id, auto: e.checked ?? false });
                }}
                tooltip="Enable Auto Update"
                tooltipOptions={getTopToolOptions}
              />

              <NumberEditorBodyTemplate
                onChange={async (e) => {
                  await onEPGUpdateClick({ id: rowData.id, hours: e });
                }}
                suffix=" hours"
                value={rowData.hoursToUpdate}
              />
            </div>
          </div>

          <div className="col-4 p-0 justify-content-end align-items-center">
            <EPGPreviewDialog selectedFile={rowData} />
            <EPGFileRefreshDialog selectedFile={rowData} />
            <EPGFileRemoveDialog selectedFile={rowData} />
          </div>
        </div>
      );
    },
    [onEPGUpdateClick]
  );

  const urlEditorBodyTemplate = useCallback(
    (rowData: M3UFileDto) => {
      if (rowData.id === 0) {
        return (
          <div
            className="p-0 relative"
            style={{
              backgroundColor: 'var(--mask-bg)',
              overflow: 'hidden',
              textOverflow: 'ellipsis',
              whiteSpace: 'nowrap'
            }}
          >
            {rowData.url}
          </div>
        );
      }

      return (
        <StringEditorBodyTemplate
          onChange={async (e) => {
            await onEPGUpdateClick({ id: rowData.id, url: e });
          }}
          tooltip={rowData.url}
          value={rowData.url}
        />
      );
    },
    [onEPGUpdateClick]
  );

  const columns = useMemo(
    (): ColumnMeta[] => [
      {
        bodyTemplate: colorTemplate,
        field: 'color',
        header: 'Color',
        width: '4rem'
      },
      {
        bodyTemplate: nameEditorBodyTemplate,
        field: 'name',
        filter: true,
        header: 'Name',
        sortable: true,
        width: '22rem'
      },
      {
        bodyTemplate: epgNumberBodyTemplate,
        field: 'epgNumber',
        header: 'Number',
        width: '4rem'
      },
      {
        bodyTemplate: lastDownloadedTemplate,
        field: 'lastDownloaded',
        header: 'Downloaded',
        sortable: true,
        width: '12rem'
      },
      {
        bodyTemplate: channelCountTemplate,
        field: 'channelCount',
        header: 'Channels',
        sortable: true,
        width: '6rem'
      },
      {
        bodyTemplate: programmeCountTemplate,
        field: 'programmeCount',
        header: 'Progs',
        sortable: true,
        width: '6rem'
      },
      { bodyTemplate: urlEditorBodyTemplate, field: 'url', sortable: true },
      {
        align: 'center',
        bodyTemplate: actionBodyTemplate,
        field: 'autoUpdate',
        width: '16rem'
      }
    ],
    [
      colorTemplate,
      nameEditorBodyTemplate,
      epgNumberBodyTemplate,
      lastDownloadedTemplate,
      channelCountTemplate,
      programmeCountTemplate,
      urlEditorBodyTemplate,
      actionBodyTemplate
    ]
  );

  return (
    <>
      <Toast position="bottom-right" ref={toast} />
      <DataSelector
        columns={columns}
        defaultSortField="name"
        emptyMessage="No EPG Files"
        id="epgfilesdataselector"
        queryFilter={useEpgFilesGetPagedEpgFilesQuery}
        selectedItemsKey="selectSelectedEPGFileDtoItems"
        style={{ height: 'calc(50vh - 120px)' }}
      />
    </>
  );
};

EPGFilesDataSelector.displayName = 'EPGFilesDataSelector';

export default memo(EPGFilesDataSelector);
