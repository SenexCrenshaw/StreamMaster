import MinusButton from '@components/buttons/MinusButton';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';

import { useSelectedItems } from '@lib/redux/slices/useSelectedItemsSlice';
import { RemoveSMStreamFromSMChannel, SetSMStreamRanks } from '@lib/smAPI/SMChannelStreamLinks/SMChannelStreamLinksCommands';
import useGetSMChannelStreams from '@lib/smAPI/SMChannelStreamLinks/useGetSMChannelStreams';
import {
  GetSMChannelStreamsRequest,
  RemoveSMStreamFromSMChannelRequest,
  SMChannelDto,
  SMChannelRankRequest,
  SMStreamDto,
  SetSMStreamRanksRequest
} from '@lib/smAPI/smapiTypes';
import { DataTableValue } from 'primereact/datatable';
import { memo, useCallback, useEffect, useMemo, useState } from 'react';

interface SSMChannelSMStreamNewDataSelectorProperties {
  readonly enableEdit?: boolean;
  readonly height?: string;
  readonly id?: string;
  readonly name: string | undefined;
  readonly smChannel?: SMChannelDto;
}

const SMChannelSMStreamNewDataSelector = ({ enableEdit: propsEnableEdit, height, id, name, smChannel }: SSMChannelSMStreamNewDataSelectorProperties) => {
  const dataKey = `${id}-SMChannelSMStreamNewDataSelector`;
  const { selectSelectedItems, setSelectSelectedItems } = useSelectedItems<SMStreamDto>(`${id}-SMStreamDataForSMChannelSelector`);
  const [enableEdit, setEnableEdit] = useState<boolean>(true);
  const { data: smChannelData, isLoading: smChannelIsLoading } = useGetSMChannelStreams({ SMChannelId: smChannel?.Id } as GetSMChannelStreamsRequest);

  useEffect(() => {
    if (propsEnableEdit !== enableEdit) {
      setEnableEdit(propsEnableEdit ?? true);
    }
  }, [enableEdit, propsEnableEdit]);

  const actionBodyTemplate = useCallback(
    (smStream: SMStreamDto) => (
      <div className="flex p-0 justify-content-end align-items-center">
        <MinusButton
          iconFilled={false}
          onClick={() => {
            if (!smStream.Id || smChannel === undefined) {
              return;
            }

            const request: RemoveSMStreamFromSMChannelRequest = { SMChannelId: smChannel.Id, SMStreamId: smStream.Id };
            RemoveSMStreamFromSMChannel(request)
              .then((response) => {
                console.log('Remove Stream', response);
              })
              .catch((error) => {
                console.error('Remove Stream', error.message);
              });
          }}
          tooltip="Remove Stream"
        />
      </div>
    ),
    [smChannel]
  );

  const columns = useMemo(
    (): ColumnMeta[] => [
      { field: 'Name', filter: true, sortable: true, width: '8rem' },
      { field: 'Group', filter: true, sortable: true },
      { field: 'M3UFileName', filter: true, header: 'M3U', sortable: true },
      {
        align: 'right',
        bodyTemplate: actionBodyTemplate,
        field: 'IsHidden',
        fieldType: 'actions',
        header: 'Actions',
        width: '5rem'
      }
    ],
    [actionBodyTemplate]
  );

  const headerName = useMemo(() => {
    if (name) {
      return name + ' Streams';
    }
    return 'Streams';
  }, [name]);

  if (smChannel) {
    return (
      <SMDataTable
        columns={columns}
        defaultSortField="Rank"
        defaultSortOrder={1}
        reorderable
        onRowReorder={(event: DataTableValue[]) => {
          const channels = [...(event as unknown as SMStreamDto[])];

          if (smChannel) {
            const tosend: SMChannelRankRequest[] = channels.map((item, index) => {
              return { SMChannelId: smChannel.Id, SMStreamId: item.Id, Rank: index } as SMChannelRankRequest;
            });
            SetSMStreamRanks({ Requests: tosend } as SetSMStreamRanksRequest)
              .then((response) => {
                console.log('SetSMStreamRanks', response);
              })
              .catch((error) => {
                console.error('SetSMStreamRanks', error.message);
              });
          } else {
            const updatedChannels = channels.map((item, index) => {
              return {
                ...item,
                Rank: index
              };
            });
            setSelectSelectedItems(updatedChannels);
          }
        }}
        dataSource={smChannelData}
        enablePaginator
        emptyMessage="No Streams"
        headerName={headerName}
        isLoading={smChannelIsLoading}
        id={dataKey}
        style={{ height: height ?? 'calc(100vh - 100px)' }}
      />
    );
  }

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
