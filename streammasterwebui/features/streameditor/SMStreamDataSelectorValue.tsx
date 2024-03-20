import MinusButton from '@components/buttons/MinusButton';
import { ColumnMeta } from '@components/dataSelector/DataSelectorTypes';

import { SMChannelDto, SMChannelRankRequest, SMStreamDto, SMStreamSMChannelRequest } from '@lib/apiDefs';

import { GetMessage } from '@lib/common/common';
import { useSelectedSMChannel } from '@lib/redux/slices/selectedSMChannel';
import { RemoveSMStreamFromSMChannel, SetSMStreamRanks } from '@lib/smAPI/SMChannels/SMChannelsCommands';

import { lazy, memo, useCallback, useMemo } from 'react';
const DataSelectorValues = lazy(() => import('@components/dataSelector/DataSelectorValues'));

interface SMStreamDataSelectorValueProperties {
  readonly id: string;
  readonly selectedSMChannelKey: string;
  readonly selectedSMStreamKey: string;
  readonly data: SMStreamDto[];
  readonly smChannel: SMChannelDto;
}

const SMStreamDataSelectorValue = ({ data, id, selectedSMChannelKey, selectedSMStreamKey, smChannel }: SMStreamDataSelectorValueProperties) => {
  const dataKey = `${id}-SMStreamDataSelectorValue`;
  const { selectedSMChannel, setSelectedSMChannel } = useSelectedSMChannel(selectedSMChannelKey);

  const actionBodyTemplate = useCallback(
    (data: SMStreamDto) => (
      <div className="flex p-0 justify-content-end align-items-center">
        <MinusButton
          iconFilled={false}
          onClick={() => {
            if (!data.id || selectedSMChannel === undefined) {
              return;
            }

            const request: SMStreamSMChannelRequest = { smChannelId: selectedSMChannel.id, smStreamId: data.id };
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
      <DataSelectorValues
        noSourceHeader
        reorderable
        columns={columns}
        defaultSortField="rank"
        defaultSortOrder={1}
        dataSource={data && [...data].sort((a, b) => a.rank - b.rank)}
        emptyMessage="No Streams"
        headerName={GetMessage('streams').toUpperCase()}
        selectedSMStreamKey={selectedSMStreamKey}
        selectedSMChannelKey={selectedSMChannelKey}
        onRowReorder={(event) => {
          if (selectedSMChannel === undefined) {
            return;
          }
          let tosend: SMChannelRankRequest[] = event.map((item, index) => {
            return { smChannelId: selectedSMChannel.id, smStreamId: item.id, rank: index } as SMChannelRankRequest;
          });
          SetSMStreamRanks(tosend)
            .then((response) => {
              console.log('SetSMStreamRanks', response);
            })
            .catch((error) => {
              console.error('SetSMStreamRanks', error.message);
            });

          console.log('tosend', tosend);
        }}
        id={dataKey}
        selectedItemsKey={'SMStreamDataSelectorValue-selectSelectedSMStreamDtoItems'}
        style={{ height: '20vh' }}
      />
    </div>
  );
};

export default memo(SMStreamDataSelectorValue);
