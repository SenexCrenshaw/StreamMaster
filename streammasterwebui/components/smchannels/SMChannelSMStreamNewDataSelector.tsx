import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';

import { useSelectedItems } from '@lib/redux/slices/useSelectedItemsSlice';
import { SMStreamDto } from '@lib/smAPI/smapiTypes';
import { DataTableValue } from 'primereact/datatable';
import { memo, useEffect, useMemo, useState } from 'react';

interface SSMChannelSMStreamNewDataSelectorProperties {
  readonly enableEdit?: boolean;
  readonly height?: string;
  readonly id?: string;
  readonly showSelections?: boolean;
  readonly name: string | undefined;
}

const SMChannelSMStreamNewDataSelector = ({ enableEdit: propsEnableEdit, height, id, name, showSelections }: SSMChannelSMStreamNewDataSelectorProperties) => {
  const dataKey = `${id}-SMStreamDataForSMChannelSelector`;
  const { selectSelectedItems, setSelectSelectedItems } = useSelectedItems<SMStreamDto>(dataKey);
  const [enableEdit, setEnableEdit] = useState<boolean>(true);

  useEffect(() => {
    if (propsEnableEdit !== enableEdit) {
      setEnableEdit(propsEnableEdit ?? true);
    }
  }, [enableEdit, propsEnableEdit]);

  const columns = useMemo(
    (): ColumnMeta[] => [
      { field: 'Name', filter: true, sortable: true, width: '8rem' },
      { field: 'Group', filter: true, sortable: true },
      { field: 'M3UFileName', filter: true, header: 'M3U', sortable: true }
    ],
    []
  );

  const headerName = useMemo(() => {
    if (name) {
      return name + ' Streams';
    }
    return 'Streams';
  }, [name]);

  return (
    <SMDataTable
      columns={columns}
      defaultSortField="Rank"
      defaultSortOrder={1}
      reorderable
      onRowReorder={(event: DataTableValue[]) => {
        const channels = [...(event as unknown as SMStreamDto[])];
        const updatedChannels = channels.map((item, index) => {
          return {
            ...item,
            Rank: index
          };
        });

        setSelectSelectedItems(updatedChannels);
      }}
      dataSource={selectSelectedItems}
      enablePaginator
      emptyMessage="No Streams"
      headerName={headerName}
      isLoading={false}
      id={dataKey}
      style={{ height: height ?? 'calc(100vh - 100px)' }}
    />
  );
};

export default memo(SMChannelSMStreamNewDataSelector);
