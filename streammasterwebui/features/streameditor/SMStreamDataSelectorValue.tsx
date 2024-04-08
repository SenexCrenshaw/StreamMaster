import MinusButton from '@components/buttons/MinusButton';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { GetMessage } from '@lib/common/common';
import useGetSMChannelStreams from '@lib/smAPI/SMChannelStreamLinks/useGetSMChannelStreams';

import { RemoveSMStreamFromSMChannel, SetSMStreamRanks } from '@lib/smAPI/SMChannelStreamLinks/SMChannelStreamLinksCommands';
import {
  GetSMChannelStreamsRequest,
  RemoveSMStreamFromSMChannelRequest,
  SMChannelDto,
  SMChannelRankRequest,
  SMStreamDto,
  SetSMStreamRanksRequest
} from '@lib/smAPI/smapiTypes';
import { DataTableRowEvent, DataTableValue } from 'primereact/datatable';
import { memo, useCallback, useMemo } from 'react';
import useSelectedSMItems from './useSelectedSMItems';

interface SMStreamDataSelectorValueProperties {
  readonly id: string;
  readonly smChannel: SMChannelDto;
}

const SMStreamDataSelectorValue = ({ id, smChannel }: SMStreamDataSelectorValueProperties) => {
  const dataKey = `${id}-SMStreamDataSelectorValue`;
  const { data, isLoading } = useGetSMChannelStreams({ SMChannelId: smChannel?.Id } as GetSMChannelStreamsRequest);
  const { setSelectedSMChannel } = useSelectedSMItems();

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
        dataSource={data}
        emptyMessage="No Streams"
        headerName={GetMessage('streams').toUpperCase()}
        isLoading={isLoading}
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
        // style={{ height: '20vh' }}
      />
    </div>
  );
};

export default memo(SMStreamDataSelectorValue);
