import SMButton from '@components/sm/SMButton';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { SMChannelDto } from '@lib/smAPI/smapiTypes';
import { DataTableValue } from 'primereact/datatable';
import { memo, ReactNode, useCallback, useMemo } from 'react';

interface SMChannelSMChannelDataSelectorForDataKeyProperties {
  readonly dataKey: string;
  readonly selectedItems: SMChannelDto[];
  readonly height?: string;
  onChange: (e: SMChannelDto[]) => void;
}

const SMChannelSMChannelDataSelectorForDataKey = ({ height, dataKey, onChange, selectedItems }: SMChannelSMChannelDataSelectorForDataKeyProperties) => {
  const actionTemplate = useCallback(
    (sMChannel: SMChannelDto) => (
      <div className="flex align-content-center justify-content-center">
        <SMButton
          buttonClassName="icon-red"
          icon="pi-minus"
          iconFilled={false}
          onClick={() => {
            const newSelectedItems = selectedItems.filter((item) => item.Id !== sMChannel.Id).sort((a, b) => a.Rank - b.Rank);
            onChange(newSelectedItems);
          }}
          tooltip="Remove Channel"
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
      emptyMessage="No Channels"
      headerName="ACTIVE CHANNELS"
      headerClassName="header-text-channels"
      id="Hey"
      onRowReorder={(event: DataTableValue[]) => {
        const channels = [...(event as unknown as SMChannelDto[])];
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

export default memo(SMChannelSMChannelDataSelectorForDataKey);
