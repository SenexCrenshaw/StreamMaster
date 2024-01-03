import { formatJSONDateString, getTopToolOptions } from '@lib/common/common';
import { useM3UFilesGetPagedM3UFilesQuery, type M3UFileDto, type UpdateM3UFileRequest } from '@lib/iptvApi';
import { UpdateM3UFile } from '@lib/smAPI/M3UFiles/M3UFilesMutateAPI';
import { Checkbox, type CheckboxChangeEvent } from 'primereact/checkbox';
import { Toast } from 'primereact/toast';
import { memo, useCallback, useMemo, useRef } from 'react';
import NumberEditorBodyTemplate from '../NumberEditorBodyTemplate';
import StringEditorBodyTemplate from '../StringEditorBodyTemplate';
import { type ColumnMeta } from '../dataSelector/DataSelectorTypes';
import M3UFileRefreshDialog from './M3UFileRefreshDialog';
import M3UFileRemoveDialog from './M3UFileRemoveDialog';

import DataSelector from '../dataSelector/DataSelector';
import M3UFileTagsDialog from './M3UFileTagsDialog';
interface M3UUpdateProperties {
  id: number;
  auto?: boolean | null;
  hours?: number | null;
  maxStreams?: number | null;
  overwriteChannelNumbers?: boolean | null;
  name?: string | null;
  url?: string | null;
  startingChannelNumber?: number | null;
}

const M3UFilesDataSelector = () => {
  const toast = useRef<Toast>(null);

  const onM3UUpdateClick = useCallback(
    async (props: M3UUpdateProperties) => {
      if (props.id < 1) {
        return;
      }

      const { id, ...restProperties } = props;

      // Check if all values of the rest of the properties are null or undefined
      if (Object.values(restProperties).every((value) => value === null || value === undefined)) {
        return;
      }

      const { auto, hours, maxStreams, name, url, startingChannelNumber, overwriteChannelNumbers } = restProperties;

      const tosend = {} as UpdateM3UFileRequest;
      tosend.id = id;

      if (auto !== undefined) {
        tosend.autoUpdate = auto;
      }

      if (hours) {
        tosend.hoursToUpdate = hours;
      }

      if (hours) {
        tosend.hoursToUpdate = hours;
      }

      if (name) {
        tosend.name = name;
      }

      if (overwriteChannelNumbers !== undefined) {
        tosend.overWriteChannels = overwriteChannelNumbers;
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
              detail: 'M3U File Update Successful',
              life: 3000,
              severity: 'success',
              summary: 'Successful'
            });
          }
        })
        .catch((error) => {
          if (toast.current) {
            toast.current.show({
              detail: 'M3U File Update Failed',
              life: 3000,
              severity: 'error',
              summary: `Error ${error.message}`
            });
          }
        });
    },
    [toast]
  );

  // const StreamURLPrefixEditorBodyTemplate = useCallback(
  //   (rowData: M3UFileDto) => {
  //     if (rowData.id === 0) {
  //       return <div />;
  //     }

  //     return (
  //       <div className="flex justify-content-center ">
  //         <StreamURLPrefixSelector
  //           onChange={async (e) => {
  //             await onM3UUpdateClick({ id: rowData.id, streamURLPrefix: e });
  //           }}
  //           value={rowData.streamURLPrefix}
  //         />
  //       </div>
  //     );
  //   },
  //   [onM3UUpdateClick],
  // );

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
            await onM3UUpdateClick({ id: rowData.id, name: e });
          }}
          value={rowData.name}
        />
      );
    },
    [onM3UUpdateClick]
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
            await onM3UUpdateClick({ id: rowData.id, url: e });
          }}
          tooltip={rowData.url}
          value={rowData.url}
        />
      );
    },
    [onM3UUpdateClick]
  );

  const tagEditorBodyTemplate = useCallback((rowData: M3UFileDto) => {
    if (rowData.id === 0) {
      return <div></div>;
    }

    return <M3UFileTagsDialog m3uFileDto={rowData} />;
  }, []);

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
    [onM3UUpdateClick]
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
    [onM3UUpdateClick]
  );

  const stationCountTemplate = useCallback((rowData: M3UFileDto) => {
    if (rowData.id === 0) {
      return <div />;
    }

    return <div className="flex p-0 m-0 justify-content-center align-items-center">{rowData.stationCount}</div>;
  }, []);

  const actionBodyTemplate = useCallback(
    (rowData: M3UFileDto) => {
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
                  await onM3UUpdateClick({ auto: e.checked ?? false, id: rowData.id });
                }}
                tooltip="Enable Auto Update"
                tooltipOptions={getTopToolOptions}
              />

              <NumberEditorBodyTemplate
                onChange={async (e) => {
                  await onM3UUpdateClick({ auto: rowData.autoUpdate, hours: e, id: rowData.id, maxStreams: rowData.maxStreamCount ?? 0 });
                }}
                suffix=" hours"
                value={rowData.hoursToUpdate}
              />
            </div>
          </div>
          <div className="col-4 p-0 justify-content-end align-items-center">
            <Checkbox
              checked={rowData.overwriteChannelNumbers}
              onChange={async (e: CheckboxChangeEvent) => {
                await onM3UUpdateClick({ overwriteChannelNumbers: e.checked ?? false, id: rowData.id });
              }}
              tooltip="Autoset Channel #s"
              tooltipOptions={getTopToolOptions}
            />
            <M3UFileRefreshDialog selectedFile={rowData} />
            <M3UFileRemoveDialog selectedFile={rowData} />
          </div>
        </div>
      );
    },
    [onM3UUpdateClick]
  );

  const columns = useMemo(
    (): ColumnMeta[] => [
      {
        bodyTemplate: nameEditorBodyTemplate,
        field: 'name',
        header: 'Name',
        sortable: true,
        width: '22rem'
      },
      // {
      //   bodyTemplate: StreamURLPrefixEditorBodyTemplate,
      //   field: 'streamURLPrefix',
      //   header: 'Output Type',
      //   width: '14rem',
      // },
      {
        bodyTemplate: lastDownloadedTemplate,
        field: 'lastDownloaded',
        header: 'Downloaded',
        sortable: true,
        width: '12rem'
      },
      {
        bodyTemplate: startingChannelNumberTemplate,
        field: 'startingChannelNumber',
        header: 'Start Ch#',
        sortable: true,
        width: '6rem'
      },
      {
        bodyTemplate: maxStreamCountTemplate,
        field: 'maxStreamCount',
        header: 'Max Streams',
        sortable: true,
        width: '8rem'
      },
      {
        bodyTemplate: stationCountTemplate,
        field: 'stationCount',
        header: 'Streams',
        sortable: true,
        width: '6rem'
      },
      { bodyTemplate: urlEditorBodyTemplate, field: 'url', sortable: true },
      { align: 'center', bodyTemplate: tagEditorBodyTemplate, field: 'vodTags', header: 'VODs (ignore)', width: '8rem' },

      {
        align: 'center',
        bodyTemplate: actionBodyTemplate,
        field: 'autoUpdate',
        width: '16rem'
      }
    ],
    [
      nameEditorBodyTemplate,
      lastDownloadedTemplate,
      startingChannelNumberTemplate,
      maxStreamCountTemplate,
      stationCountTemplate,
      urlEditorBodyTemplate,
      tagEditorBodyTemplate,
      actionBodyTemplate
    ]
  );

  return (
    <>
      <Toast position="bottom-right" ref={toast} />

      <DataSelector
        columns={columns}
        defaultSortField="name"
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
