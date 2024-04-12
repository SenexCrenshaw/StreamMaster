import { memo, useCallback, useMemo } from 'react';

import StringEditorBodyTemplate from '../inputs/StringEditorBodyTemplate';
import M3UFileRefreshDialog from './M3UFileRefreshDialog';
import M3UFileDeleteDialog from './M3UFileDeleteDialog';

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

    return <div>{formatJSONDateString(rowData.LastDownloaded ?? '')}</div>;
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

  const stationCountTemplate = useCallback((rowData: M3UFileDto) => {
    if (rowData.Id === 0) {
      return <div />;
    }

    return <div className="flex p-0 m-0 justify-content-center align-items-center">{rowData.StationCount}</div>;
  }, []);

  const actionTemplate = useCallback((rowData: M3UFileDto) => {
    if (rowData.Id === 0) {
      return <div />;
    }
    return (
      <div className="flex justify-content-center align-items-center">
        <M3UFileRefreshDialog selectedFile={rowData} />
        <M3UFileDeleteDialog selectedFile={rowData} />
        <M3UFileEditDialog selectedFile={rowData} />
      </div>
    );
  }, []);

  const columns = useMemo(
    (): ColumnMeta[] => [
      {
        bodyTemplate: nameEditorBodyTemplate,
        field: 'Name',
        header: 'Name',
        sortable: true,
        width: '22rem'
      },
      {
        align: 'left',
        alignHeader: 'center',
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
        bodyTemplate: actionTemplate,
        field: 'editBodyTemplate',
        header: 'Actions',
        width: '6rem'
      }
    ],
    [nameEditorBodyTemplate, lastDownloadedTemplate, stationCountTemplate, actionTemplate]
  );

  return (
    <SMDataTable
      noSourceHeader
      columns={columns}
      defaultSortField="Name"
      defaultSortOrder={1}
      emptyMessage="No M3U Files"
      enableExport={false}
      id="m3ufilesdataselector"
      queryFilter={useGetPagedM3UFiles}
    />
  );
};

export default memo(M3UFilesDataSelector);
