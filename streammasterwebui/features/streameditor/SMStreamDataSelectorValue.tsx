import MinusButton from '@components/buttons/MinusButton';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { GetMessage } from '@lib/common/common';
import { RemoveSMStreamFromSMChannel, SetSMStreamRanks } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import useGetPagedSMChannels from '@lib/smAPI/SMChannels/useGetPagedSMChannels';
import { RemoveSMStreamFromSMChannelRequest, SMChannelDto, SMChannelRankRequest, SMStreamDto, SetSMStreamRanksRequest } from '@lib/smAPI/smapiTypes';
import { DataTableRowEvent, DataTableValue } from 'primereact/datatable';
import { memo, useCallback, useEffect, useMemo } from 'react';
import useSelectedSMItems from './useSelectedSMItems';

interface SMStreamDataSelectorValueProperties {
  readonly id: string;
  readonly smChannel: SMChannelDto;
}

const SMStreamDataSelectorValue = ({ id, smChannel }: SMStreamDataSelectorValueProperties) => {
  const dataKey = `${id}-SMStreamDataSelectorValue`;
  const { setSelectedSMChannel } = useSelectedSMItems();
  const { data } = useGetPagedSMChannels();
  console.log('SMStreamDataSelectorValue', smChannel);

  useEffect(() => {
    if (smChannel) {
      const channel = data?.Data?.find((c) => c.Id === smChannel.Id);
      if (channel) {
        console.log('channel', channel.SMStreams);
      }
    }
  }, [data, smChannel, smChannel.SMStreams, setSelectedSMChannel]);

  const actionBodyTemplate = useCallback(
    (data: SMStreamDto) => (
      <div className="flex p-0 justify-content-end align-items-center">
        <MinusButton
          iconFilled={false}
          onClick={() => {
            if (!data.Id || smChannel === undefined) {
              return;
            }

            const request: RemoveSMStreamFromSMChannelRequest = { SMChannelId: smChannel.Id, SMStreamId: data.Id };
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
      { field: 'Logo', fieldType: 'image', sortable: false, width: '4rem' },
      // { field: 'rank', sortable: false },
      { field: 'Name', sortable: false },
      { field: 'Group', sortable: false, width: '5rem' },
      { field: 'M3UFileName', header: 'M3U', sortable: false, width: '5rem' },
      {
        align: 'right',
        bodyTemplate: actionBodyTemplate,
        field: 'isHidden',
        fieldType: 'actions',
        header: 'Actions',
        width: '5rem'
      }
    ],
    [actionBodyTemplate]
  );

  if (!smChannel?.SMStreams) {
    return null;
  }

  return (
    <div
      onClick={() => {
        if (smChannel) {
          // if (!selectedSMChannel || selectedSMChannel.Id !== smChannel.Id) {
          //   setSelectedSMChannel(smChannel);
          // }
        }
      }}
    >
      <SMDataTable
        noSourceHeader
        reorderable
        columns={columns}
        defaultSortField="rank"
        defaultSortOrder={1}
        dataSource={smChannel.SMStreams && [...smChannel.SMStreams].sort((a, b) => a.Rank - b.Rank)}
        emptyMessage="No Streams"
        headerName={GetMessage('streams').toUpperCase()}
        onRowReorder={(event: DataTableValue[]) => {
          const channels = event as unknown as SMStreamDto[];
          if (smChannel === undefined || channels === undefined) {
            return;
          }

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

          console.log('tosend', tosend);
        }}
        onRowExpand={(e: DataTableRowEvent) => {
          setSelectedSMChannel(e.data as SMChannelDto);
        }}
        id={dataKey}
        selectedItemsKey={'SMStreamDataSelectorValue-selectSelectedSMStreamDtoItems'}
        style={{ height: '20vh' }}
      />
    </div>
  );
};

export default memo(SMStreamDataSelectorValue);
