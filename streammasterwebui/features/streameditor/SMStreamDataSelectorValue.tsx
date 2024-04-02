import MinusButton from '@components/buttons/MinusButton';

import { GetMessage } from '@lib/common/common';
import { RemoveSMStreamFromSMChannel, SetSMStreamRanks } from '@lib/smAPI/SMChannels/SMChannelsCommands';

import { RemoveSMStreamFromSMChannelRequest, SMChannelDto, SMChannelRankRequest, SMStreamDto, SetSMStreamRanksRequest } from '@lib/smAPI/smapiTypes';

import { ColumnMeta } from '@components/smDataTable/ColumnMeta';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { DataTableRowEvent, DataTableValue } from 'primereact/datatable';
import { memo, useCallback, useMemo } from 'react';
import useSelectedSMItems from './useSelectedSMItems';

interface SMStreamDataSelectorValueProperties {
  readonly id: string;

  readonly data: SMStreamDto[];
  readonly smChannel: SMChannelDto;
}

const SMStreamDataSelectorValue = ({ data, id, smChannel }: SMStreamDataSelectorValueProperties) => {
  const dataKey = `${id}-SMStreamDataSelectorValue`;
  const { selectedSMChannel, setSelectedSMChannel } = useSelectedSMItems();

  const actionBodyTemplate = useCallback(
    (data: SMStreamDto) => (
      <div className="flex p-0 justify-content-end align-items-center">
        <MinusButton
          iconFilled={false}
          onClick={() => {
            if (!data.id || selectedSMChannel === undefined) {
              return;
            }

            const request: RemoveSMStreamFromSMChannelRequest = { smChannelId: selectedSMChannel.id, smStreamId: data.id };
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
    [selectedSMChannel]
  );

  const columns = useMemo(
    (): ColumnMeta[] => [
      { field: 'logo', fieldType: 'image', sortable: false, width: '4rem' },
      // { field: 'rank', sortable: false },
      { field: 'name', sortable: false },
      { field: 'group', sortable: false, width: '5rem' },
      { field: 'm3UFileName', header: 'M3U', sortable: false, width: '5rem' },
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

  return (
    <div
      onClick={() => {
        setSelectedSMChannel(smChannel);
      }}
    >
      <SMDataTable
        noSourceHeader
        reorderable
        columns={columns}
        defaultSortField="rank"
        defaultSortOrder={1}
        dataSource={data && [...data].sort((a, b) => a.rank - b.rank)}
        emptyMessage="No Streams"
        headerName={GetMessage('streams').toUpperCase()}
        onRowReorder={(event: DataTableValue[]) => {
          const channels = event as unknown as SMStreamDto[];
          if (selectedSMChannel === undefined || channels === undefined) {
            return;
          }

          const tosend: SMChannelRankRequest[] = channels.map((item, index) => {
            return { smChannelId: selectedSMChannel.id, smStreamId: item.id, rank: index } as SMChannelRankRequest;
          });

          SetSMStreamRanks({ requests: tosend } as SetSMStreamRanksRequest)
            .then((response) => {
              console.log('SetSMStreamRanks', response);
            })
            .catch((error) => {
              console.error('SetSMStreamRanks', error.message);
            });

          console.log('tosend', tosend);
        }}
        onRowExpand={(e: DataTableRowEvent) => {
          setSelectedSMChannel(e.data);
        }}
        id={dataKey}
        selectedItemsKey={'SMStreamDataSelectorValue-selectSelectedSMStreamDtoItems'}
        style={{ height: '20vh' }}
      />
    </div>
  );
};

export default memo(SMStreamDataSelectorValue);
