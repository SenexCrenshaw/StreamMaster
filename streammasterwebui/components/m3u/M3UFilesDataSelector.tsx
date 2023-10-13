import { formatJSONDateString, getTopToolOptions } from '@lib/common/common';
import { M3UFileStreamUrlPrefix, useM3UFilesGetPagedM3UFilesQuery, type M3UFileDto, type UpdateM3UFileRequest } from '@lib/iptvApi';
import { UpdateM3UFile } from '@lib/smAPI/M3UFiles/M3UFilesMutateAPI';
import { Checkbox, type CheckboxChangeEvent } from 'primereact/checkbox';
import { Toast } from 'primereact/toast';
import { memo, useCallback, useMemo, useRef, type CSSProperties } from 'react';
import NumberEditorBodyTemplate from '../NumberEditorBodyTemplate';
import StringEditorBodyTemplate from '../StringEditorBodyTemplate';
import DataSelector from '../dataSelector/DataSelector';
import { type ColumnMeta } from '../dataSelector/DataSelectorTypes';
import M3UFileRefreshDialog from './M3UFileRefreshDialog';
import M3UFileRemoveDialog from './M3UFileRemoveDialog';
import StreamURLPrefixSelector from './StreamURLPrefixSelector';

type M3UUpdateProps = {
  id: number;
  auto?: boolean | null;
  hours?: number | null;
  maxStreams?: number | null;
  name?: string | null;
  url?: string | null;
  startingChannelNumber?: number | null;
  streamURLPrefix?: M3UFileStreamUrlPrefix | null;
};

const M3UFilesDataSelector = () => {
  const toast = useRef<Toast>(null);

  const onM3UUpdateClick = useCallback(
    async (props: M3UUpdateProps) => {
      if (props.id < 1) {
        return;
      }

      const { id, ...restProps } = props;

      // Check if all values of the rest of the properties are null or undefined
      if (Object.values(restProps).every((value) => value === null || value === undefined)) {
        return;
      }

      const { auto, hours, maxStreams, name, url, startingChannelNumber, streamURLPrefix } = restProps;

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

      if (streamURLPrefix !== null) {
        tosend.streamURLPrefixInt = parseInt(streamURLPrefix?.toString() ?? '0');
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
        })
        .catch((e) => {
          if (toast.current) {
            toast.current.show({
              detail: `M3U File Update Failed`,
              life: 3000,
              severity: 'error',
              summary: 'Error ' + e.message,
            });
          }
        });
    },
    [toast],
  );

  const StreamURLPrefixEditorBodyTemplate = useCallback((rowData: M3UFileDto) => {
    if (rowData.id === 0) {
      return <div />;
    }

    return (
      <div className="flex justify-content-center ">
        <StreamURLPrefixSelector
          onChange={async (e) => {
            await onM3UUpdateClick({ id: rowData.id, streamURLPrefix: e });
          }}
          value={rowData.streamURLPrefix}
        />
      </div>
    );
  }, []);

  const lastDownloadedTemplate = useCallback((rowData: M3UFileDto) => {
    if (rowData.id === 0) {
      return <div />;
    }

    return <div className="flex justify-content-center ">{formatJSONDateString(rowData.lastDownloaded ?? '')}</div>;
  }, []);

  const nameEditorBodyTemplate = useCallback(
    (rowData: M3UFileDto) => {
      if (rowData.id === 0) {
        return (
          <div
            className="p-0 relative"
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
        );
      }

      return (
        <StringEditorBodyTemplate
          onChange={async (e) => {
            await onM3UUpdateClick({ id: rowData.id, name: e });
          }}
          value={rowData.name}
        />
      );
    },
    [onM3UUpdateClick],
  );

  const urlEditorBodyTemplate = useCallback(
    (rowData: M3UFileDto) => {
      if (rowData.id === 0) {
        return (
          <div
            className="p-0 relative"
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
        );
      }

      return (
        <StringEditorBodyTemplate
          onChange={async (e) => {
            await onM3UUpdateClick({ id: rowData.id, url: e });
          }}
          tooltip={rowData.url}
          value={rowData.url}
        />
      );
    },
    [onM3UUpdateClick],
  );

  const maxStreamCountTemplate = useCallback(
    (rowData: M3UFileDto) => {
      if (rowData.id === 0) {
        return <div />;
      }

      return (
        <NumberEditorBodyTemplate
          onChange={async (e) => {
            await onM3UUpdateClick({ id: rowData.id, maxStreams: e });
          }}
          value={rowData.maxStreamCount}
        />
      );
    },
    [onM3UUpdateClick],
  );

  const startingChannelNumberTemplate = useCallback(
    (rowData: M3UFileDto) => {
      if (rowData.id === 0) {
        return <div />;
      }

      return (
        <NumberEditorBodyTemplate
          onChange={async (e) => {
            await onM3UUpdateClick({ id: rowData.id, startingChannelNumber: e });
          }}
          value={rowData.startingChannelNumber}
        />
      );
    },
    [onM3UUpdateClick],
  );

  const stationCountTemplate = useCallback((rowData: M3UFileDto) => {
    if (rowData.id === 0) {
      return <div />;
    }

    return <div>{rowData.stationCount}</div>;
  }, []);

  const targetActionBodyTemplate = useCallback(
    (rowData: M3UFileDto) => {
      if (rowData.id === 0) {
        return <div />;
      }

      return (
        <div className="flex grid p-0 justify-content-end align-items-center">
          <div className="col-6 p-0 justify-content-end align-items-center">
            <Checkbox
              checked={rowData.autoUpdate}
              onChange={async (e: CheckboxChangeEvent) => {
                await onM3UUpdateClick({ id: rowData.id, auto: e.checked ?? false });
              }}
              tooltip="Enable Auto Update"
              tooltipOptions={getTopToolOptions}
            />

            <NumberEditorBodyTemplate
              onChange={async (e) => {
                await onM3UUpdateClick({ id: rowData.id, auto: rowData.autoUpdate, hours: e, maxStreams: rowData.maxStreamCount ?? 0 });
              }}
              suffix=" hours"
              value={rowData.hoursToUpdate}
            />
          </div>
          <div className="col-6 p-0 justify-content-end align-items-center">
            <M3UFileRefreshDialog selectedFile={rowData} />
            <M3UFileRemoveDialog selectedFile={rowData} />
          </div>
        </div>
      );
    },
    [onM3UUpdateClick],
  );

  const columns = useMemo((): ColumnMeta[] => {
    return [
      {
        bodyTemplate: nameEditorBodyTemplate,
        field: 'name',
        header: 'Name',
        sortable: true,
      },
      {
        bodyTemplate: StreamURLPrefixEditorBodyTemplate,
        field: 'streamURLPrefix',
        header: 'streamURLPrefix',
      },
      {
        bodyTemplate: lastDownloadedTemplate,
        field: 'lastDownloaded',
        header: 'Downloaded',
        sortable: true,
      },
      {
        bodyTemplate: startingChannelNumberTemplate,
        field: 'startingChannelNumber',
        header: 'Start Ch #',
        sortable: true,
        style: {
          maxWidth: '8rem',
          width: '8rem',
        } as CSSProperties,
      },
      {
        bodyTemplate: maxStreamCountTemplate,
        field: 'maxStreamCount',
        header: 'Max Streams',
        sortable: true,
        style: {
          maxWidth: '8rem',
          width: '8rem',
        } as CSSProperties,
      },
      {
        bodyTemplate: stationCountTemplate,
        field: 'stationCount',
        header: 'Streams',
        sortable: true,
      },
      { bodyTemplate: urlEditorBodyTemplate, field: 'url', sortable: true },
      {
        align: 'center',
        bodyTemplate: targetActionBodyTemplate,
        field: 'autoUpdate',
        style: {
          maxWidth: '8rem',
          width: '8rem',
        } as CSSProperties,
      },
    ];
  }, [
    StreamURLPrefixEditorBodyTemplate,
    lastDownloadedTemplate,
    maxStreamCountTemplate,
    nameEditorBodyTemplate,
    startingChannelNumberTemplate,
    stationCountTemplate,
    targetActionBodyTemplate,
    urlEditorBodyTemplate,
  ]);

  return (
    <>
      <Toast position="bottom-right" ref={toast} />

      <DataSelector
        columns={columns}
        emptyMessage="No M3U Files"
        id="m3ufilesdataselector"
        queryFilter={useM3UFilesGetPagedM3UFilesQuery}
        selectedItemsKey="selectSelectedM3UFileDtoItems"
        style={{ height: 'calc(50vh - 120px)' }}
      />
    </>
  );
};

export default memo(M3UFilesDataSelector);
