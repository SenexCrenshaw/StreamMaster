import SMButton from '@components/sm/SMButton';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { SMStreamDto } from '@lib/smAPI/smapiTypes';
import { DataTableValue } from 'primereact/datatable';
import { memo, ReactNode, useCallback, useMemo } from 'react';

interface SMChannelSMStreamDataSelectorForDataKeyProperties {
  readonly dataKey: string;
  readonly selectedItems: SMStreamDto[];
  readonly height?: string;
  onChange: (e: SMStreamDto[]) => void;
}

const SMChannelSMStreamDataSelectorForDataKey = ({ height, dataKey, onChange, selectedItems }: SMChannelSMStreamDataSelectorForDataKeyProperties) => {
  const actionTemplate = useCallback(
    (smStream: SMStreamDto) => (
      <div className="flex align-content-center justify-content-center">
        <SMButton
          buttonClassName="icon-red"
          icon="pi-minus"
          iconFilled={false}
          onClick={() => {
            const newSelectedItems = selectedItems.filter((item) => item.Id !== smStream.Id).sort((a, b) => a.Rank - b.Rank);
            onChange(newSelectedItems);
          }}
          tooltip="Remove Stream"
        />
      </div>
    ),
    [onChange, selectedItems]
  );

  const addOrRemoveHeaderTemplate = useMemo((): ReactNode => {
    return <div className="flex align-content-center justify-content-center">{/* <SMTriSelectShowHidden dataKey={dataKey} />{' '} */}</div>;
  }, []);

  const columns = useMemo((): ColumnMeta[] => {
    const w = '12rem';
    const z = '4rem';

    return [
      { field: 'Name', maxWidth: w, minWidth: w, width: w },
      { field: 'M3UFileName', header: 'M3U', maxWidth: z, minWidth: z, width: z },
      {
        bodyTemplate: actionTemplate,
        field: 'custom',
        fieldType: 'custom',
        headerTemplate: addOrRemoveHeaderTemplate,
        maxWidth: '2rem',
        minWidth: '2rem',
        width: '2rem'
      }
    ];
  }, [actionTemplate, addOrRemoveHeaderTemplate]);

  return (
    <SMDataTable
      columns={columns}
      defaultSortField="Rank"
      defaultSortOrder={1}
      dataSource={selectedItems}
      enablePaginator
      emptyMessage="No Streams"
      headerName="ACTIVE STREAMS"
      headerClassName="header-text-channels"
      id="Hey"
      onRowReorder={(event: DataTableValue[]) => {
        const channels = [...(event as unknown as SMStreamDto[])];
        const updatedChannels = channels.map((item, index) => {
          return {
            ...item,
            Rank: index
          };
        });

        onChange(updatedChannels);
      }}
      reorderable
      style={{ height: height ?? 'calc(100vh - 100px)' }}
    />
  );
};

export default memo(SMChannelSMStreamDataSelectorForDataKey);
