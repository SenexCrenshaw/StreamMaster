import StringEditor from '@components/inputs/StringEditor';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { useFilters } from '@lib/redux/hooks/filters';
import { useSortInfo } from '@lib/redux/hooks/sortInfo';
import { UpdateFileProfile } from '@lib/smAPI/Profiles/ProfilesCommands';
import useGetFileProfiles from '@lib/smAPI/Profiles/useGetFileProfiles';
import { FileOutputProfileDto, UpdateFileProfileRequest } from '@lib/smAPI/smapiTypes';
import { DataTableRowClickEvent } from 'primereact/datatable';
import { memo, useCallback, useMemo } from 'react';
import CreateFileProfileDialog from './CreateFileProfileDialog';
import RemoveFileProfileDialog from './RemoveFileProfileDialog';
import { useFileProfileChannelIdColumnConfig } from './columns/useFileProfileChannelIdColumnConfig';
import { useFileProfileChannelNumberColumnConfig } from './columns/useFileProfileChannelNumberColumnConfig';
import { useFileProfileGroupTitleColumnConfig } from './columns/useFileProfileGroupTitleColumnConfig';
import { useFileProfileTVGGroupColumnConfig } from './columns/useFileProfileTVGGroupColumnConfig';
import { useFileProfileTVGIdColumnConfig } from './columns/useFileProfileTVGIdColumnConfig';
import { useFileProfileTVGNameColumnConfig } from './columns/useFileProfileTVGNameColumnConfig';

const StreamGroupFileProfileDataSelector = () => {
  const id = 'StreamGroupFileProfileDataSelector';
  const { filters } = useFilters(id);
  const { sortInfo } = useSortInfo(id);
  const { data } = useGetFileProfiles();

  const tvgNameColumnConfig = useFileProfileTVGNameColumnConfig({ width: 40 });
  const channelIdColumnConfig = useFileProfileChannelIdColumnConfig({ width: 40 });
  const tvgGroupColumnConfig = useFileProfileTVGGroupColumnConfig({ width: 40 });
  const tvgIDColumnConfig = useFileProfileTVGIdColumnConfig({ width: 40 });
  const groupTitleColumnConfig = useFileProfileGroupTitleColumnConfig({ width: 40 });
  const channelNumberColumnConfig = useFileProfileChannelNumberColumnConfig({ width: 40 });

  const filteredValues = useMemo(() => {
    if (!data) {
      return [];
    }

    let ret = data;

    if (filters !== undefined && filters['Name'] !== undefined) {
      ret = data.filter((item: any) => {
        const filterKey = 'Name' as keyof typeof item;
        const itemValue = item[filterKey];
        return typeof itemValue === 'string' && itemValue.toLowerCase().includes(filters['Name'].value.toLowerCase());
      });
    } else {
      ret = data.filter((item: any) => {
        const filterKey = 'Name' as keyof typeof item;
        const itemValue = item[filterKey];

        return typeof itemValue === 'string' && itemValue.toLowerCase().includes('hi');
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

  const actionTemplate = useCallback((rowData: FileOutputProfileDto) => {
    return (
      <div className="flex justify-content-center align-items-center">
        <RemoveFileProfileDialog fileOutputProfileDto={rowData} />
        {/* <StreamGroupDeleteDialog streamGroup={rowData} /> */}
        {/* <M3UFileRefreshDialog selectedFile={rowData} />
         <M3UFileRemoveDialog selectedFile={rowData} /> */}
        {/* <EPGFileEditDialog selectedFile={rowData} /> */}
      </div>
    );
  }, []);

  const update = useCallback((request: UpdateFileProfileRequest) => {
    console.log('update', request);

    UpdateFileProfile(request)
      .then((res) => {})
      .catch((error) => {
        console.log('error', error);
      })
      .finally();
  }, []);

  const nameTemplate = useCallback(
    (rowData: FileOutputProfileDto) => {
      if (rowData.IsReadOnly === true) {
        return <div className="text-container pl-1">{rowData.Name}</div>;
      }
      return (
        <StringEditor
          value={rowData.Name}
          onSave={(e) => {
            if (e !== undefined) {
              const ret = { Name: rowData.Name, NewName: e } as UpdateFileProfileRequest;
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
      tvgNameColumnConfig,
      channelIdColumnConfig,
      channelNumberColumnConfig,
      tvgIDColumnConfig,
      tvgGroupColumnConfig,
      groupTitleColumnConfig,
      {
        align: 'center',
        bodyTemplate: actionTemplate,
        field: 'action',
        filter: false,
        header: 'Actions',
        width: 20
      }
    ],
    [
      nameTemplate,
      tvgNameColumnConfig,
      channelIdColumnConfig,
      channelNumberColumnConfig,
      tvgIDColumnConfig,
      tvgGroupColumnConfig,
      groupTitleColumnConfig,
      actionTemplate
    ]
  );

  const headerRightTemplate = useMemo(() => {
    return (
      <>
        <CreateFileProfileDialog />
      </>
    );
  }, []);

  return (
    <SMDataTable
      columns={columns}
      dataSource={filteredValues}
      defaultSortField="Name"
      defaultSortOrder={1}
      emptyMessage="No Profiles"
      enableClick
      enableExport={false}
      headerName="M3U/EPG Profiles"
      headerRightTemplate={headerRightTemplate}
      id={id}
      lazy
      onRowClick={(e: DataTableRowClickEvent) => {}}
      style={{ height: '30vh' }}
    />
  );
};

StreamGroupFileProfileDataSelector.displayName = 'StreamGroupFileProfileDataSelector';

export default memo(StreamGroupFileProfileDataSelector);
