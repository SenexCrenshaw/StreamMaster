import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import StreamGroupDeleteDialog from '@components/streamGroup/StreamGroupDeleteDialog';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { useSelectedStreamGroup } from '@lib/redux/hooks/selectedStreamGroup';
import useGetPagedStreamGroups from '@lib/smAPI/StreamGroups/useGetPagedStreamGroups';
import { StreamGroupDto } from '@lib/smAPI/smapiTypes';
import { DataTableRowClickEvent, DataTableRowData, DataTableRowExpansionTemplate } from 'primereact/datatable';
import { memo, useCallback, useMemo } from 'react';
import StreamGroupDataSelectorValue from './StreamGroupDataSelectorValue';

export interface StreamGroupDataSelectorProperties {
  readonly id: string;
}

const StreamGroupDataSelector = ({ id }: StreamGroupDataSelectorProperties) => {
  const { selectedStreamGroup, setSelectedStreamGroup } = useSelectedStreamGroup('StreamGroup');
  const { isLoading } = useGetPagedStreamGroups();
  const { setSelectedItems } = useSelectedItems('selectedStreamGroup');

  const rowExpansionTemplate = useCallback((rowData: DataTableRowData<any>, options: DataTableRowExpansionTemplate) => {
    const streamGroupDto = rowData as unknown as StreamGroupDto;
    // setSelectedStreamGroup(channel);

    return (
      <div className="ml-3 m-1">
        <StreamGroupDataSelectorValue streamGroupDto={streamGroupDto} id={streamGroupDto.Id + '-streams'} />
      </div>
    );
  }, []);

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
        sortable: true,
        width: 150
      },
      {
        align: 'right',
        field: 'StreamCount',
        header: '#',
        width: 40
      },
      {
        field: 'Url',
        fieldType: 'url',
        width: 100
      },
      {
        field: 'epglink',
        fieldType: 'epglink',
        width: 40
      },
      {
        field: 'm3ulink',
        fieldType: 'm3ulink',
        width: 40
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
      defaultSortField="Name"
      defaultSortOrder={1}
      emptyMessage="No Stream Groups"
      enableClick
      enableExport={false}
      id={id}
      isLoading={isLoading}
      noSourceHeader
      showExpand
      onRowClick={(e: DataTableRowClickEvent) => {
        if (e.data.Id !== selectedStreamGroup?.Id) {
          console.log('StreamGroupDataSelector', e.data);
          setSelectedStreamGroup(e.data as StreamGroupDto);
          setSelectedItems([e.data as StreamGroupDto]);
        }
        // else {
        //   setSelectedStreamGroup(undefined);
        //   setSelectedItems([]);
        // }
      }}
      // onRowExpand={(e: DataTableRowEvent) => {
      //   if (e.data.Id !== selectedStreamGroup?.Id) {
      //     setSelectedStreamGroup(e.data as StreamGroupDto);
      //   }
      // }}
      // onRowCollapse={(e: DataTableRowEvent) => {
      //   if (e.data.Id === selectedStreamGroup?.Id) {
      //     setSelectedStreamGroup(undefined);
      //   }
      // }}
      rowExpansionTemplate={rowExpansionTemplate}
      queryFilter={useGetPagedStreamGroups}
      selectedItemsKey="selectedStreamGroup"
      selectionMode="single"
      selectRow
      style={{ height: '30vh' }}
    />
  );
};

StreamGroupDataSelector.displayName = 'Stream Group Editor';

export default memo(StreamGroupDataSelector);
