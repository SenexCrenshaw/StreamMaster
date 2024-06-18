import StringEditor from '@components/inputs/StringEditor';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { useFilters } from '@lib/redux/hooks/filters';
import { useSortInfo } from '@lib/redux/hooks/sortInfo';
import { UpdateVideoProfile } from '@lib/smAPI/Profiles/ProfilesCommands';
import useGetVideoProfiles from '@lib/smAPI/Profiles/useGetVideoProfiles';
import { UpdateVideoProfileRequest, VideoOutputProfileDto } from '@lib/smAPI/smapiTypes';
import { DataTableRowClickEvent } from 'primereact/datatable';
import { memo, useCallback, useMemo } from 'react';
import CreateVideoProfileDialog from './CreateVideoProfileDialog';
import RemoveVideoProfileDialog from './RemoveVideoProfileDialog';
import { useVideoIsM3U8ColumnConfig } from './columns/useVideoIsM3U8ColumnConfig';
import { useVideoProfileCommandColumnConfig } from './columns/useVideoProfileCommandColumnConfig';
import { useVideoProfileParametersColumnConfig } from './columns/useVideoProfileParametersColumnConfig';
import { useVideoProfileTimeoutColumnConfig } from './columns/useVideoProfileTimeoutColumnConfig';

const StreamGroupVideoProfileDataSelector = () => {
  const id = 'StreamGroupVideoProfileDataSelector';
  const { filters } = useFilters(id);
  const { sortInfo } = useSortInfo(id);
  const { data } = useGetVideoProfiles();

  const videoProfileCommandColumnConfig = useVideoProfileCommandColumnConfig({ width: 40 });
  const videoProfileParametersColumnConfig = useVideoProfileParametersColumnConfig({ width: 200 });
  const videoProfileTimeoutColumnConfig = useVideoProfileTimeoutColumnConfig({ width: 20 });
  const videoProfilIsM3U8ColumnConfig = useVideoIsM3U8ColumnConfig({ width: 20 });

  const filteredValues = useMemo(() => {
    if (!data) {
      return [];
    }

    let ret = [...data];

    if (filters !== undefined && filters['Name'] !== undefined) {
      ret = ret.filter((item: any) => {
        const filterKey = 'Name' as keyof typeof item;
        const itemValue = item[filterKey];
        return typeof itemValue === 'string' && itemValue.toLowerCase().includes(filters['Name'].value.toLowerCase());
      });
    }

    if (sortInfo !== undefined) {
      ret = ret.sort((a: any, b: any) => {
        const sortField = sortInfo.sortField as keyof typeof a;
        if (a[sortField] < b[sortField]) {
          return -1 * sortInfo.sortOrder;
        }
        if (a[sortField] > b[sortField]) {
          return 1 * sortInfo.sortOrder;
        }
        return 0;
      });
    }

    return ret;
  }, [data, filters, sortInfo]);

  const actionTemplate = useCallback((rowData: VideoOutputProfileDto) => {
    return (
      <div className="flex justify-content-center align-items-center">
        <RemoveVideoProfileDialog videoOutputProfileDto={rowData} />
        {/* <StreamGroupDeleteDialog streamGroup={rowData} /> */}
        {/* <M3UFileRefreshDialog selectedFile={rowData} />
         <M3UFileRemoveDialog selectedFile={rowData} /> */}
        {/* <EPGFileEditDialog selectedFile={rowData} /> */}
      </div>
    );
  }, []);

  const headerRightTemplate = useMemo(() => {
    return (
      <>
        <CreateVideoProfileDialog />
      </>
    );
  }, []);

  const update = useCallback((request: UpdateVideoProfileRequest) => {
    UpdateVideoProfile(request)
      .then((res) => {})
      .catch((error) => {
        console.error('error', error);
      })
      .finally();
  }, []);

  const nameTemplate = useCallback(
    (rowData: VideoOutputProfileDto) => {
      if (rowData.Name.toLowerCase() === 'default') {
        return <div className="text-container pl-1">{rowData.Name}</div>;
      }
      return (
        <StringEditor
          value={rowData.Name}
          onSave={(e) => {
            if (e !== undefined) {
              const ret = { Name: rowData.Name, NewName: e } as UpdateVideoProfileRequest;
              update(ret);
            }
          }}
        />
      );
    },
    [update]
  );

  const columns = useMemo(
    (): ColumnMeta[] => [
      {
        bodyTemplate: nameTemplate,
        field: 'Name',
        filter: true,

        sortable: true,
        width: 40
      },
      videoProfileCommandColumnConfig,
      videoProfileParametersColumnConfig,
      videoProfileTimeoutColumnConfig,
      videoProfilIsM3U8ColumnConfig,
      {
        align: 'center',
        bodyTemplate: actionTemplate,
        field: 'autoUpdate',
        header: 'Actions',
        width: 40
      }
    ],
    [
      actionTemplate,
      nameTemplate,
      videoProfilIsM3U8ColumnConfig,
      videoProfileCommandColumnConfig,
      videoProfileParametersColumnConfig,
      videoProfileTimeoutColumnConfig
    ]
  );

  return (
    <SMDataTable
      columns={columns}
      dataSource={filteredValues}
      defaultSortField="Name"
      defaultSortOrder={1}
      emptyMessage="No Profiles"
      enableClick
      enableExport={false}
      headerName="Video Profiles"
      headerRightTemplate={headerRightTemplate}
      id={id}
      lazy
      onRowClick={(e: DataTableRowClickEvent) => {}}
      style={{ height: '30vh' }}
    />
  );
};

StreamGroupVideoProfileDataSelector.displayName = 'StreamGroupVideoProfileDataSelector';

export default memo(StreamGroupVideoProfileDataSelector);
