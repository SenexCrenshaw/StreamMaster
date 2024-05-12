import SMButton from '@components/sm/SMButton';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';

import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
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
  const { selectedItems, setSelectedItems } = useSelectedItems<SMStreamDto>(`${id}-SMStreamDataForSMChannelSelector`);
  const [enableEdit, setEnableEdit] = useState<boolean>(true);
  const { data: smChannelData, isLoading: smChannelIsLoading } = useGetSMChannelStreams({ SMChannelId: smChannel?.Id } as GetSMChannelStreamsRequest);

  useEffect(() => {
    if (propsEnableEdit !== enableEdit) {
      setEnableEdit(propsEnableEdit ?? true);
    }
  }, [enableEdit, propsEnableEdit]);

  const actionTemplate = useCallback(
    (smStream: SMStreamDto) => (
      <div className="flex align-content-center justify-content-end">
        <SMButton
          className="icon-red"
          icon="pi-minus"
          iconFilled={false}
          onClick={() => {
            if (smChannel) {
              const request: RemoveSMStreamFromSMChannelRequest = { SMChannelId: smChannel.Id, SMStreamId: smStream.Id };
              RemoveSMStreamFromSMChannel(request)
                .then((response) => {})
                .catch((error) => {
                  console.error('Remove Stream', error.message);
                });
            } else {
              const newSelectedItems = selectedItems.filter((item) => item.Id !== smStream.Id);
              setSelectedItems(newSelectedItems);
            }
          }}
          tooltip="Remove Stream"
        />
      </div>
    ),
    [selectedItems, setSelectedItems, smChannel]
  );

  const columns = useMemo(
    (): ColumnMeta[] => [
      { field: 'Name', filter: true, sortable: true, width: '8rem' },
      { field: 'M3UFileName', filter: true, header: 'M3U', sortable: true },
      {
        align: 'right',
        bodyTemplate: actionTemplate,
        field: 'IsHidden',
        fieldType: 'actions',
        width: '5rem'
      }
    ],
    [actionTemplate]
  );

  // const headerName = useMemo(() => {
  //   if (name) {
  //     return name + ' Streams';
  //   }
  //   return 'Streams';
  // }, [name]);

  if (smChannel) {
    return (
      <SMDataTable
        columns={columns}
        defaultSortField="Rank"
        defaultSortOrder={1}
        reorderable
        onRowReorder={(event: DataTableValue[]) => {
          const channels = [...(event as unknown as SMStreamDto[])];

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
        }}
        dataSource={smChannelData}
        enablePaginator
        emptyMessage="No Streams"
        headerName="ACTIVE STREAMS"
        headerClassName="header-text-channels"
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

        setSelectedItems(updatedChannels);
      }}
      dataSource={selectedItems}
      enablePaginator
      emptyMessage="No Streams"
      headerName="ACTIVE STREAMS"
      headerClassName="header-text-channels"
      isLoading={false}
      id={dataKey}
      style={{ height: height ?? 'calc(100vh - 100px)' }}
    />
  );
};

export default memo(SMChannelSMStreamNewDataSelector);
