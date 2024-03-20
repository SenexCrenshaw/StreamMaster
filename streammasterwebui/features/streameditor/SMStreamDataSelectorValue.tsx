import MinusButton from '@components/buttons/MinusButton';
import { ColumnMeta } from '@components/dataSelector/DataSelectorTypes';

import { SMChannelRankRequest, SMChannelRankRequests, SMStreamDto, SMStreamSMChannelRequest } from '@lib/apiDefs';

import { GetMessage } from '@lib/common/common';
import { useSelectedSMChannel } from '@lib/redux/slices/selectedSMChannel';
import { RemoveSMStreamFromSMChannel, SetSMStreamRanks } from '@lib/smAPI/SMChannels/SMChannelsCommands';

import { Suspense, lazy, memo, useCallback, useMemo } from 'react';
const DataSelectorValues = lazy(() => import('@components/dataSelector/DataSelectorValues'));

interface SMStreamDataSelectorValueProperties {
  readonly id: string;
  readonly selectedSMChannelKey: string;
  readonly data: SMStreamDto[];
  readonly isLoading: boolean;
}

const SMStreamDataSelectorValue = ({ data, id, selectedSMChannelKey, isLoading }: SMStreamDataSelectorValueProperties) => {
  const dataKey = `${id}-SMStreamDataSelectorValue`;
  const { selectedSMChannel } = useSelectedSMChannel(selectedSMChannelKey);

  const actionBodyTemplate = useCallback(
    (data: SMStreamDto) => (
      <div className="flex p-0 justify-content-end align-items-center">
        <Suspense>
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
        </Suspense>
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
    <Suspense fallback={<div>Loading...</div>}>
      <DataSelectorValues
        noSourceHeader
        reorderable
        columns={columns}
        defaultSortField="rank"
        defaultSortOrder={1}
        emptyMessage="No Streams"
        headerName={GetMessage('streams').toUpperCase()}
        dataSource={[...data].sort((a, b) => a.rank - b.rank)}
        onRowReorder={(event) => {
          if (selectedSMChannel === undefined) {
            return;
          }
          let tosend: SMChannelRankRequests = event.map((item, index) => {
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
        isLoading={isLoading}
        id={dataKey}
        selectedItemsKey={'SMStreamDataSelectorValue-selectSelectedSMStreamDtoItems'}
        style={{ height: '30vh' }}
        onRowClick={(event) => {
          console.log('row click', event);
        }}
      />
    </Suspense>
  );
};

export default memo(SMStreamDataSelectorValue);
