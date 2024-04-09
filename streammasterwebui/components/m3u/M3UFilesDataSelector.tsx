import { memo, useCallback, useMemo } from 'react';

import StringEditorBodyTemplate from '../inputs/StringEditorBodyTemplate';
import M3UFileRefreshDialog from './M3UFileRefreshDialog';
import M3UFileRemoveDialog from './M3UFileRemoveDialog';

import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { formatJSONDateString } from '@lib/common/dateTime';
import { UpdateM3UFile } from '@lib/smAPI/M3UFiles/M3UFilesCommands';
import useGetPagedM3UFiles from '@lib/smAPI/M3UFiles/useGetPagedM3UFiles';
import { M3UFileDto, UpdateM3UFileRequest } from '@lib/smAPI/smapiTypes';
import SMDataTable from '../smDataTable/SMDataTable';
import M3UFileEditDialog from './M3UFileEditDialog';

interface M3UUpdateProperties {
  auto?: boolean | null;
  hours?: number | null;
  id: number;
  maxStreams?: number | null;
  name?: string | null;
  overwriteChannelNumbers?: boolean | null;
  startingChannelNumber?: number | null;
  url?: string | null;
}

const M3UFilesDataSelector = () => {
  const onM3UUpdateClick = useCallback(async (props: M3UUpdateProperties) => {
    if (props.id < 1) {
      return;
    }

    const { id, ...restProperties } = props;

    if (Object.values(restProperties).every((value) => value === null || value === undefined)) {
      return;
    }

    const { auto, hours, maxStreams, name, url, startingChannelNumber, overwriteChannelNumbers } = restProperties;

    const request = {} as UpdateM3UFileRequest;
    request.Id = id;

    if (auto !== undefined) {
      request.AutoUpdate = auto === true;
    }

    if (hours) {
      request.HoursToUpdate = hours;
    }

    if (hours) {
      request.HoursToUpdate = hours;
    }

    if (name) {
      request.Name = name;
    }

    if (overwriteChannelNumbers !== undefined) {
      request.OverWriteChannels = overwriteChannelNumbers === true;
    }

    if (maxStreams) {
      request.MaxStreamCount = maxStreams;
    }

    if (url) {
      request.Url = url;
    }

    if (startingChannelNumber) {
      request.StartingChannelNumber = startingChannelNumber;
    }

    await UpdateM3UFile(request)
      .then(() => {})
      .catch((error) => {
        console.error('Error updating M3U File', error);
        throw error;
      });
  }, []);

  const lastDownloadedTemplate = useCallback((rowData: M3UFileDto) => {
    if (rowData.Id === 0) {
      return <div />;
    }

    return <div className="flex justify-content-center">{formatJSONDateString(rowData.LastDownloaded ?? '')}</div>;
  }, []);

  const nameEditorBodyTemplate = useCallback(
    (rowData: M3UFileDto) => {
      if (rowData.Id === 0) {
        return <div>{rowData.Name}</div>;
      }

      return (
        <StringEditorBodyTemplate
          onChange={async (e) => {
            await onM3UUpdateClick({ id: rowData.Id, name: e });
          }}
          value={rowData.Name}
        />
      );
    },
    [onM3UUpdateClick]
  );

  // const urlEditorBodyTemplate = useCallback(
  //   (rowData: M3UFileDto) => {
  //     if (rowData.Id === 0) {
  //       return (
  //         <div
  //           className="p-0 relative"
  //           style={{
  //             backgroundColor: 'var(--mask-bg)',
  //             overflow: 'hidden',
  //             textOverflow: 'ellipsis',
  //             whiteSpace: 'nowrap'
  //           }}
  //         >
  //           {rowData.Url}
  //         </div>
  //       );
  //     }

  //     return (
  //       <StringEditorBodyTemplate
  //         onChange={async (e) => {
  //           await onM3UUpdateClick({ id: rowData.Id, url: e });
  //         }}
  //         tooltip={rowData.Url}
  //         value={rowData.Url}
  //       />
  //     );
  //   },
  //   [onM3UUpdateClick]
  // );

  // const tagEditorBodyTemplate = useCallback((rowData: M3UFileDto) => {
  //   if (rowData.Id === 0) {
  //     return <div></div>;
  //   }

  //   return <M3UFileTagsDialog m3uFileDto={rowData} />;
  // }, []);

  // const maxStreamCountTemplate = useCallback((rowData: M3UFileDto) => {
  //   if (rowData.Id === 0) {
  //     return <div />;
  //   }

  //   return <M3UFilesMaxStreamsEditor data={rowData} />;
  // }, []);

  // const startingChannelNumberTemplate = useCallback(
  //   (rowData: M3UFileDto) => {
  //     if (rowData.Id === 0) {
  //       return <div />;
  //     }

  //     return (
  //       <NumberEditorBodyTemplate
  //         onChange={async (e) => {
  //           await onM3UUpdateClick({ id: rowData.Id, startingChannelNumber: e });
  //         }}
  //         value={rowData.StartingChannelNumber}
  //       />
  //     );
  //   },
  //   [onM3UUpdateClick]
  // );

  const stationCountTemplate = useCallback((rowData: M3UFileDto) => {
    if (rowData.Id === 0) {
      return <div />;
    }

    return <div className="flex p-0 m-0 justify-content-center align-items-center">{rowData.StationCount}</div>;
  }, []);

  // const autoUpdateTemplate = useCallback(
  //   (rowData: M3UFileDto) => {
  //     if (rowData.Id === 0) {
  //       return <div />;
  //     }

  //     return (
  //       <div className="flex w-6 justify-content-start align-items-center">
  //         <Checkbox
  //           checked={rowData.AutoUpdate}
  //           onChange={async (e: CheckboxChangeEvent) => {
  //             await onM3UUpdateClick({ auto: e.checked ?? false, id: rowData.Id });
  //           }}
  //           tooltip="Enable Auto Update"
  //           tooltipOptions={getTopToolOptions}
  //         />
  //         <div className="autoUpdate">
  //           <NumberEditorBodyTemplate
  //             onChange={async (e) => {
  //               await onM3UUpdateClick({ auto: rowData.AutoUpdate, hours: e, id: rowData.Id, maxStreams: rowData.MaxStreamCount ?? 0 });
  //             }}
  //             suffix=" hours"
  //             value={rowData.HoursToUpdate}
  //           />
  //         </div>
  //       </div>
  //     );
  //   },
  //   [onM3UUpdateClick]
  // );

  const actionBodyTemplate = useCallback((rowData: M3UFileDto) => {
    if (rowData.Id === 0) {
      return <div />;
    }
    return (
      <div className="flex justify-content-center align-items-center">
        <M3UFileRefreshDialog selectedFile={rowData} />
        <M3UFileRemoveDialog selectedFile={rowData} />
        <M3UFileEditDialog selectedFile={rowData} />
      </div>
    );
  }, []);

  // const overwriteTemplate = useCallback(
  //   (rowData: M3UFileDto) => {
  //     if (rowData.Id === 0) {
  //       return <div />;
  //     }
  //     return (
  //       <div className="flex justify-content-center align-items-center">
  //         <Checkbox
  //           checked={rowData.OverwriteChannelNumbers}
  //           onChange={async (e: CheckboxChangeEvent) => {
  //             await onM3UUpdateClick({ overwriteChannelNumbers: e.checked ?? false, id: rowData.Id });
  //           }}
  //           tooltip="Autoset Channel #s"
  //           tooltipOptions={getTopToolOptions}
  //         />
  //       </div>
  //     );
  //   },
  //   [onM3UUpdateClick]
  // );

  // const expandedColumns = useMemo(
  //   (): ColumnMeta[] => [
  //     {
  //       align: 'left',
  //       bodyTemplate: urlEditorBodyTemplate,
  //       field: 'url'
  //     },
  //     {
  //       bodyTemplate: startingChannelNumberTemplate,
  //       field: 'startingChannelNumber',
  //       header: 'Start Ch#',
  //       width: '4rem'
  //     },
  //     {
  //       bodyTemplate: maxStreamCountTemplate,
  //       field: 'maxStreamCount',
  //       header: 'Max Streams',
  //       width: '4.4rem'
  //     },
  //     {
  //       bodyTemplate: autoUpdateTemplate,
  //       field: 'autoUpdate',
  //       width: '5rem'
  //     },

  //     {
  //       bodyTemplate: overwriteTemplate,
  //       field: 'overwrite',
  //       header: 'Set Ch #s',
  //       width: '4rem'
  //     },

  //     {
  //       align: 'center',
  //       bodyTemplate: actionBodyTemplate,
  //       field: 'actions',
  //       width: '3.4rem'
  //     },
  //     { align: 'center', bodyTemplate: tagEditorBodyTemplate, field: 'vodTags', header: 'URL (ignore)', width: '6rem' }
  //   ],
  //   [
  //     urlEditorBodyTemplate,
  //     startingChannelNumberTemplate,
  //     maxStreamCountTemplate,
  //     autoUpdateTemplate,
  //     overwriteTemplate,
  //     actionBodyTemplate,
  //     tagEditorBodyTemplate
  //   ]
  // );

  // const rowExpansionTemplate = useCallback(
  //   (data: any, options: DataTableRowExpansionTemplate) => {
  //     return (
  //       <div className="border-2 border-round-lg border-200 ml-3 m-1">
  //         <SMDataTable enableHeaderWrap noSourceHeader id={'m3uFileDataSelectorValues'} columns={expandedColumns} dataSource={[data]} />
  //       </div>
  //     );
  //   },
  //   [expandedColumns]
  // );

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

        width: '12rem'
      },
      {
        bodyTemplate: stationCountTemplate,
        field: 'stationCount',
        header: 'Streams',
        width: '6rem'
      },
      {
        bodyTemplate: actionBodyTemplate,
        field: 'editBodyTemplate',
        header: 'Actions',
        width: '6rem'
      }
    ],
    [nameEditorBodyTemplate, lastDownloadedTemplate, stationCountTemplate, actionBodyTemplate]
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
      // rowExpansionTemplate={rowExpansionTemplate}
    />
  );
};

export default memo(M3UFilesDataSelector);
