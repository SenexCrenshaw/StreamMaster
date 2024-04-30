import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import StreamGroupDeleteDialog from '@components/streamGroup/StreamGroupDeleteDialog';
import { useSelectedItems } from '@lib/redux/slices/useSelectedItemsSlice';
import { useSelectedStreamGroup } from '@lib/redux/slices/useSelectedStreamGroup';
import useGetPagedStreamGroups from '@lib/smAPI/StreamGroups/useGetPagedStreamGroups';
import { StreamGroupDto } from '@lib/smAPI/smapiTypes';
import { DataTableRowClickEvent } from 'primereact/datatable';
import { memo, useCallback, useMemo } from 'react';

export interface StreamGroupDataSelectorProperties {
  readonly id: string;
}

const StreamGroupDataSelector = ({ id }: StreamGroupDataSelectorProperties) => {
  const { selectedStreamGroup, setSelectedStreamGroup } = useSelectedStreamGroup('StreamGroup');
  const { isLoading } = useGetPagedStreamGroups();
  const { setSelectSelectedItems } = useSelectedItems('selectedStreamGroup');

  const actionTemplate = useCallback((rowData: StreamGroupDto) => {
    if (rowData.IsReadOnly === true) {
      return <div />;
    }
    return (
      <div className="flex justify-content-center align-items-center">
        <StreamGroupDeleteDialog streamGroup={rowData} />
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
        sortable: true
      },
      {
        field: 'StreamCount',
        header: '#'
      },
      {
        field: 'Url',
        fieldType: 'url'
      },
      {
        field: 'epglink',
        fieldType: 'epglink'
      },
      {
        field: 'm3ulink',
        fieldType: 'm3ulink'
      },
      {
        align: 'center',
        bodyTemplate: actionTemplate,
        field: 'autoUpdate',
        header: 'Actions',
        width: '6rem'
      }
    ],
    [actionTemplate]
  );

  return (
    <SMDataTable
      noSourceHeader
      enableClick
      selectRow
      columns={columns}
      defaultSortField="Name"
      defaultSortOrder={1}
      emptyMessage="No Stream Groups Files"
      enableExport={false}
      id={id}
      isLoading={isLoading}
      queryFilter={useGetPagedStreamGroups}
      style={{ height: '40vh' }}
      selectionMode="single"
      selectedItemsKey="selectedStreamGroup"
      onRowClick={(e: DataTableRowClickEvent) => {
        if (e.data.Id !== selectedStreamGroup?.Id) {
          console.log('StreamGroupDataSelector', e.data);
          setSelectedStreamGroup(e.data as StreamGroupDto);
          setSelectSelectedItems([e.data as StreamGroupDto]);
        } else {
          setSelectedStreamGroup(undefined);
          setSelectSelectedItems([]);
        }
      }}
    />
  );
};

StreamGroupDataSelector.displayName = 'Stream Group Editor';

export default memo(StreamGroupDataSelector);
