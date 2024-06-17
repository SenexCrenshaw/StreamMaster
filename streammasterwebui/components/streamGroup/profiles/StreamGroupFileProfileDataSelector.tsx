import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { useFilters } from '@lib/redux/hooks/filters';
import { useSortInfo } from '@lib/redux/hooks/sortInfo';
import useGetFileProfiles from '@lib/smAPI/Profiles/useGetFileProfiles';
import { FileOutputProfile } from '@lib/smAPI/smapiTypes';
import { DataTableRowClickEvent } from 'primereact/datatable';
import { memo, useCallback, useMemo } from 'react';
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

  const tvgNameColumnConfig = useFileProfileTVGNameColumnConfig();
  const channelIdColumnConfig = useFileProfileChannelIdColumnConfig();
  const tvgGroupColumnConfig = useFileProfileTVGGroupColumnConfig();
  const tvgIDColumnConfig = useFileProfileTVGIdColumnConfig();
  const groupTitleColumnConfig = useFileProfileGroupTitleColumnConfig();
  const channelNumberColumnConfig = useFileProfileChannelNumberColumnConfig();

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

  const actionTemplate = useCallback((rowData: FileOutputProfile) => {
    return (
      <div className="flex justify-content-center align-items-center">
        {/* <StreamGroupDeleteDialog streamGroup={rowData} /> */}
        {/* <M3UFileRefreshDialog selectedFile={rowData} />
         <M3UFileRemoveDialog selectedFile={rowData} /> */}
        {/* <EPGFileEditDialog selectedFile={rowData} /> */}
      </div>
    );
  }, []);

  const columns = useMemo(
    (): ColumnMeta[] => [
      {
        field: 'Name',
        filter: true,
        sortable: true,
        width: 60
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
        width: 40
      }
    ],
    [tvgNameColumnConfig, channelIdColumnConfig, channelNumberColumnConfig, tvgIDColumnConfig, tvgGroupColumnConfig, groupTitleColumnConfig, actionTemplate]
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
      headerName="M3U/EPG Profiles"
      id={id}
      lazy
      onRowClick={(e: DataTableRowClickEvent) => {}}
      style={{ height: '30vh' }}
    />
  );
};

StreamGroupFileProfileDataSelector.displayName = 'StreamGroupFileProfileDataSelector';

export default memo(StreamGroupFileProfileDataSelector);
