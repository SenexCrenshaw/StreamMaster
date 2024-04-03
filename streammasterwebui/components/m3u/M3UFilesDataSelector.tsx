import { formatJSONDateString, getTopToolOptions } from '@lib/common/common';

import { Checkbox, type CheckboxChangeEvent } from 'primereact/checkbox';
import { memo, useCallback, useMemo } from 'react';

import NumberEditorBodyTemplate from '../inputs/NumberEditorBodyTemplate';
import StringEditorBodyTemplate from '../inputs/StringEditorBodyTemplate';
import M3UFileRefreshDialog from './M3UFileRefreshDialog';
import M3UFileRemoveDialog from './M3UFileRemoveDialog';
import M3UFileTagsDialog from './M3UFileTagsDialog';

import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import useGetPagedM3UFiles from '@lib/smAPI/M3UFiles/useGetPagedM3UFiles';
import { M3UFileDto } from '@lib/smAPI/smapiTypes';
import { DataTableRowExpansionTemplate } from 'primereact/datatable';
import SMDataTable from '../smDataTable/SMDataTable';
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
  const onM3UUpdateClick = useCallback(async (props: M3UUpdateProperties) => {
    if (props.id < 1) {
      return;
    }

    const { id, ...restProperties } = props;

    // Check if all values of the rest of the properties are null or undefined
    if (Object.values(restProperties).every((value) => value === null || value === undefined)) {
      return;
    }

    const { auto, hours, maxStreams, name, url, startingChannelNumber, overwriteChannelNumbers } = restProperties;

    const tosend = {} as any;
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

    // await UpdateM3UFile(tosend)
    //   .then(() => {
    //     if (toast.current) {
    //       toast.current.show({
    //         detail: 'M3U File Update Successful',
    //         life: 3000,
    //         severity: 'success',
    //         summary: 'Successful'
    //       });
    //     }
    //   })
    //   .catch((error) => {
    //     if (toast.current) {
    //       toast.current.show({
    //         detail: 'M3U File Update Failed',
    //         life: 3000,
    //         severity: 'error',
    //         summary: `Error ${error.message}`
    //       });
    //     }
    //   });
  }, []);

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

    return <div className="flex justify-content-center">{formatJSONDateString(rowData.lastDownloaded ?? '')}</div>;
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

  const autoUpdateTemplate = useCallback(
    (rowData: M3UFileDto) => {
      if (rowData.id === 0) {
        return <div />;
      }

      return (
        <div className="flex w-6 justify-content-start align-items-center">
          <Checkbox
            checked={rowData.autoUpdate}
            onChange={async (e: CheckboxChangeEvent) => {
              await onM3UUpdateClick({ auto: e.checked ?? false, id: rowData.id });
            }}
            tooltip="Enable Auto Update"
            tooltipOptions={getTopToolOptions}
          />
          <div className="autoUpdate">
            <NumberEditorBodyTemplate
              onChange={async (e) => {
                await onM3UUpdateClick({ auto: rowData.autoUpdate, hours: e, id: rowData.id, maxStreams: rowData.maxStreamCount ?? 0 });
              }}
              suffix=" hours"
              value={rowData.hoursToUpdate}
            />
          </div>
        </div>
      );
    },
    [onM3UUpdateClick]
  );

  const actionBodyTemplate = useCallback((rowData: M3UFileDto) => {
    if (rowData.id === 0) {
      return <div />;
    }
    return (
      <div className="flex justify-content-center align-items-center">
        <M3UFileRefreshDialog selectedFile={rowData} />
        <M3UFileRemoveDialog selectedFile={rowData} />
      </div>
    );
  }, []);

  const overwriteTemplate = useCallback(
    (rowData: M3UFileDto) => {
      if (rowData.id === 0) {
        return <div />;
      }
      return (
        <div className="flex justify-content-center align-items-center">
          <Checkbox
            checked={rowData.overwriteChannelNumbers}
            onChange={async (e: CheckboxChangeEvent) => {
              await onM3UUpdateClick({ overwriteChannelNumbers: e.checked ?? false, id: rowData.id });
            }}
            tooltip="Autoset Channel #s"
            tooltipOptions={getTopToolOptions}
          />
        </div>
      );
    },
    [onM3UUpdateClick]
  );

  const expandedColumns = useMemo(
    (): ColumnMeta[] => [
      {
        bodyTemplate: urlEditorBodyTemplate,
        field: 'url',
        width: '12rem'
      },
      {
        bodyTemplate: startingChannelNumberTemplate,
        field: 'startingChannelNumber',
        header: 'Start Ch#',
        width: '4rem'
      },
      {
        bodyTemplate: maxStreamCountTemplate,
        field: 'maxStreamCount',
        header: 'Max Streams',
        width: '4rem'
      },
      {
        bodyTemplate: autoUpdateTemplate,
        field: 'autoUpdate',
        width: '8.2rem'
      },

      {
        bodyTemplate: overwriteTemplate,
        field: 'overwrite',
        header: 'Set Ch #s',
        width: '5.6rem'
      },

      {
        align: 'center',
        bodyTemplate: actionBodyTemplate,
        field: 'actions',
        width: '5rem'
      },
      { align: 'center', bodyTemplate: tagEditorBodyTemplate, field: 'vodTags', header: 'URL (ignore)', width: '8rem' }
    ],
    [
      urlEditorBodyTemplate,
      startingChannelNumberTemplate,
      maxStreamCountTemplate,
      autoUpdateTemplate,
      overwriteTemplate,
      actionBodyTemplate,
      tagEditorBodyTemplate
    ]
  );

  const rowExpansionTemplate = useCallback(
    (data: any, options: DataTableRowExpansionTemplate) => {
      console.log('expanded data', data);
      return (
        <div className="border-2 border-round-lg border-200 ml-3 m-1">
          <SMDataTable noSourceHeader id={'M3UFileDataSelectorValues'} columns={expandedColumns} dataSource={[data]} />
        </div>
      );
    },
    [expandedColumns]
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
      {
        bodyTemplate: lastDownloadedTemplate,
        field: 'lastDownloaded',
        header: 'Downloaded',
        sortable: true,
        width: '12rem'
      },

      {
        bodyTemplate: stationCountTemplate,
        field: 'stationCount',
        header: 'Streams',
        sortable: true,
        width: '6rem'
      }
    ],
    [nameEditorBodyTemplate, lastDownloadedTemplate, stationCountTemplate]
  );

  return (
    <SMDataTable
      noSourceHeader
      columns={columns}
      defaultSortField="name"
      emptyMessage="No M3U Files"
      enableExport={false}
      id="m3ufilesdataselector"
      queryFilter={useGetPagedM3UFiles}
      rowExpansionTemplate={rowExpansionTemplate}
      showExpand
    />
  );
};

export default memo(M3UFilesDataSelector);
