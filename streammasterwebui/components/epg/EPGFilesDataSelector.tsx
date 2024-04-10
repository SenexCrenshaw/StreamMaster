import ColorEditor from '@components/ColorEditor';
import SMDataTable from '@components/smDataTable/SMDataTable';

import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { formatJSONDateString } from '@lib/common/dateTime';
import { UpdateEPGFile } from '@lib/smAPI/EPGFiles/EPGFilesCommands';
import useGetPagedEPGFiles from '@lib/smAPI/EPGFiles/useGetPagedEPGFiles';
import { EPGFileDto, UpdateEPGFileRequest } from '@lib/smAPI/smapiTypes';
import { memo, useCallback, useMemo } from 'react';
import StringEditorBodyTemplate from '../inputs/StringEditorBodyTemplate';
import EPGFileEditDialog from './EPGFileEditDialog';
// import EPGFileRefreshDialog from './EPGFileRefreshDialog';
// import EPGFileRemoveDialog from './EPGFileRemoveDialog';
// import EPGPreviewDialog from './EPGPreviewDialog';

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

  const nameEditorBodyTemplate = useCallback(
    (rowData: EPGFileDto) => {
      if (rowData.Id === 0) {
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
            {rowData.Name}
          </div>
        );
      }

      return (
        <StringEditorBodyTemplate
          onChange={async (e) => {
            await onEPGUpdateClick({ id: rowData.Id, name: e });
          }}
          value={rowData.Name}
        />
      );
    },
    [onEPGUpdateClick]
  );

  const colorTemplate = useCallback(
    (rowData: EPGFileDto) => {
      if (rowData.Id === 0) {
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
            {rowData.Color}
          </div>
        );
      }

      return (
        <ColorEditor
          onChange={async (e) => {
            await onEPGUpdateClick({ id: rowData.Id, color: e });
          }}
          color={rowData.Color}
        />
      );
    },
    [onEPGUpdateClick]
  );

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

  const actionBodyTemplate = useCallback((rowData: EPGFileDto) => {
    if (rowData.Id === 0) {
      return <div />;
    }
    return (
      <div className="flex justify-content-center align-items-center">
        {/* <M3UFileRefreshDialog selectedFile={rowData} />
         <M3UFileRemoveDialog selectedFile={rowData} /> */}
        <EPGFileEditDialog selectedFile={rowData} />
      </div>
    );
  }, []);

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
        bodyTemplate: lastDownloadedTemplate,
        field: 'lastDownloaded',
        header: 'Downloaded',
        width: '12rem'
      },
      {
        bodyTemplate: channelCountTemplate,
        field: 'channelCount',
        header: 'Channels',
        width: '6rem'
      },
      {
        bodyTemplate: programmeCountTemplate,
        field: 'programmeCount',
        header: 'Progs',
        width: '6rem'
      },
      {
        align: 'center',
        bodyTemplate: actionBodyTemplate,
        field: 'autoUpdate',
        header: 'Actions',
        width: '16rem'
      }
    ],
    [colorTemplate, nameEditorBodyTemplate, lastDownloadedTemplate, channelCountTemplate, programmeCountTemplate, actionBodyTemplate]
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

    // <>
    //   <Toast position="bottom-right" ref={toast} />
    //   <DataSelector
    //     columns={columns}
    //     defaultSortField="name"
    //     emptyMessage="No EPG Files"
    //     id="epgfilesdataselector"
    //     queryFilter={useEpgFilesGetPagedEpgFilesQuery}
    //     selectedItemsKey="selectSelectedEPGFileDtoItems"
    //     style={{ height: 'calc(50vh - 120px)' }}
    //   />
    // </>
  );
};

EPGFilesDataSelector.displayName = 'EPGFilesDataSelector';

export default memo(EPGFilesDataSelector);
