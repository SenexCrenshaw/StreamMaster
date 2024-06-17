import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { useFilters } from '@lib/redux/hooks/filters';
import { useSortInfo } from '@lib/redux/hooks/sortInfo';
import useGetVideoProfiles from '@lib/smAPI/Profiles/useGetVideoProfiles';
import { VideoOutputProfile } from '@lib/smAPI/smapiTypes';
import { DataTableRowClickEvent } from 'primereact/datatable';
import { memo, useCallback, useMemo } from 'react';

const StreamGroupVideoProfileDataSelector = () => {
  const id = 'StreamGroupVideoProfileDataSelector';
  const { filters } = useFilters(id);
  const { sortInfo } = useSortInfo(id);
  const { data } = useGetVideoProfiles();

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

  const actionTemplate = useCallback((rowData: VideoOutputProfile) => {
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
        width: 150
      },

      {
        align: 'center',
        bodyTemplate: actionTemplate,
        field: 'autoUpdate',
        header: 'Actions',
        width: 40
      }
    ],
    [actionTemplate]
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
      id={id}
      lazy
      onRowClick={(e: DataTableRowClickEvent) => {}}
      style={{ height: '30vh' }}
    />
  );
};

StreamGroupVideoProfileDataSelector.displayName = 'StreamGroupVideoProfileDataSelector';

export default memo(StreamGroupVideoProfileDataSelector);
