import ColorEditor from '@components/inputs/ColorEditor';
import SMDataTable from '@components/smDataTable/SMDataTable';

import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { formatJSONDateString } from '@lib/common/dateTime';

import { UpdateEPGFile } from '@lib/smAPI/EPGFiles/EPGFilesCommands';
import useGetPagedEPGFiles from '@lib/smAPI/EPGFiles/useGetPagedEPGFiles';
import { EPGFileDto, UpdateEPGFileRequest } from '@lib/smAPI/smapiTypes';
import { memo, useCallback, useMemo } from 'react';
import StringEditor from '../inputs/StringEditor';
import EPGFileDeleteDialog from './EPGFileDeleteDialog';
import EPGFileEditDialog from './EPGFileEditDialog';
import EPGFileRefreshDialog from './EPGFileRefreshDialog';
import EPGPreviewDialog from './EPGPreviewDialog';

const EPGFilesDataSelector = () => {
  interface EPGUpdateProperties {
    auto?: boolean | null;
    color?: string | null;
    epgNumber?: number | null;
    hours?: number | null;
    id: number;
    name?: string | null;
    timeShift?: number | null;
    url?: string | null;
  }

  const onEPGUpdateClick = useCallback(async (props: EPGUpdateProperties) => {
    if (props.id < 1) {
      return;
    }
    const { id, ...restProperties } = props;

    if (Object.values(restProperties).every((value) => value === null || value === undefined)) {
      return;
    }

    const { auto, hours, color, name, url, epgNumber, timeShift } = restProperties;

    if (id < 1) {
      return;
    }

    const request = {} as UpdateEPGFileRequest;

    request.Id = id;

    if (auto !== undefined) {
      request.AutoUpdate = auto === true;
    }

    if (color) {
      request.Color = color;
    }

    if (hours) {
      request.HoursToUpdate = hours;
    }

    if (epgNumber !== null && epgNumber !== undefined) {
      request.EPGNumber = epgNumber;
    }

    if (timeShift !== null && timeShift !== undefined) {
      request.TimeShift = timeShift;
    }

    if (name) {
      request.Name = name;
    }

    if (url) {
      request.Url = url;
    }

    await UpdateEPGFile(request)
      .then(() => {})
      .catch((error) => {
        console.error('Error updating M3U File', error);
        throw error;
      });
  }, []);

  const lastDownloadedTemplate = useCallback((rowData: EPGFileDto) => {
    if (rowData.Id === 0) {
      return <div />;
    }

    return <div className="flex justify-content-center ">{formatJSONDateString(rowData.LastDownloaded ?? '')}</div>;
  }, []);

  const nameEditorTemplate = useCallback(
    (rowData: EPGFileDto) => {
      if (rowData.Id === 0) {
        return <div className="text-container">{rowData.Name}</div>;
      }

      return (
        <StringEditor
          onSave={async (e) => {
            await onEPGUpdateClick({ id: rowData.Id, name: e });
          }}
          value={rowData.Name}
        />
      );
    },
    [onEPGUpdateClick]
  );

  const colorTemplate = useCallback((rowData: EPGFileDto) => {
    if (rowData.Id === 0) {
      return <div> </div>;
    }

    return <ColorEditor editable={false} color={rowData.Color} />;
  }, []);

  const channelCountTemplate = useCallback((rowData: EPGFileDto) => {
    if (rowData.Id === 0) {
      return <div />;
    }

    return <div className="flex p-0 m-0 justify-content-center align-items-center">{rowData.ChannelCount}</div>;
  }, []);

  const programmeCountTemplate = useCallback((rowData: EPGFileDto) => {
    if (rowData.Id === 0) {
      return <div />;
    }

    return <div className="flex p-0 m-0 justify-content-center align-items-center">{rowData.ProgrammeCount}</div>;
  }, []);

  const actionTemplate = useCallback((rowData: EPGFileDto) => {
    if (rowData.Id === 0) {
      return <div />;
    }
    return (
      <div className="flex justify-content-center align-items-center">
        <EPGPreviewDialog selectedFile={rowData} />
        <EPGFileRefreshDialog selectedFile={rowData} />
        <EPGFileDeleteDialog selectedFile={rowData} />
        <EPGFileEditDialog selectedFile={rowData} />
      </div>
    );
  }, []);

  const columns = useMemo(
    (): ColumnMeta[] => [
      {
        bodyTemplate: colorTemplate,
        className: 'px-1',
        field: 'color',
        header: 'ID',
        width: '1.2rem'
      },
      {
        bodyTemplate: nameEditorTemplate,
        field: 'name',
        filter: true,
        header: 'Name',
        sortable: true,
        width: 100
      },
      {
        bodyTemplate: lastDownloadedTemplate,
        field: 'lastDownloaded',
        header: 'Downloaded',
        width: 80
      },
      {
        bodyTemplate: channelCountTemplate,
        field: 'channelCount',
        header: 'Channels',
        width: 40
      },
      {
        bodyTemplate: programmeCountTemplate,
        field: 'programmeCount',
        header: 'Progs',
        width: 40
      },
      {
        align: 'center',
        bodyTemplate: actionTemplate,
        field: 'autoUpdate',
        header: 'Actions',
        width: 46
      }
    ],
    [colorTemplate, nameEditorTemplate, lastDownloadedTemplate, channelCountTemplate, programmeCountTemplate, actionTemplate]
  );

  return (
    <SMDataTable
      noSourceHeader
      columns={columns}
      defaultSortField="Name"
      defaultSortOrder={1}
      emptyMessage="No EPG Files"
      enableExport={false}
      id="epgfilesdataselector"
      queryFilter={useGetPagedEPGFiles}
    />
  );
};

EPGFilesDataSelector.displayName = 'EPGFilesDataSelector';

export default memo(EPGFilesDataSelector);
