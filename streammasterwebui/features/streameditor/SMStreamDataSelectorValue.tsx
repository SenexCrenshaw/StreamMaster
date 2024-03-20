import MinusButton from '@components/buttons/MinusButton';
import { ColumnMeta } from '@components/dataSelector/DataSelectorTypes';

import { SMStreamDto, SMStreamSMChannelRequest } from '@lib/apiDefs';

import { GetMessage } from '@lib/common/common';
import { useSelectedSMChannel } from '@lib/redux/slices/selectedSMChannel';
import { RemoveSMStreamFromSMChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';

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
      { field: 'logo', fieldType: 'image', width: '4rem' },
      { field: 'name', sortable: true },
      { field: 'group', sortable: true, width: '5rem' },
      { field: 'm3UFileName', header: 'M3U', sortable: true, width: '5rem' },
      {
        align: 'right',
        bodyTemplate: actionBodyTemplate,
        field: 'isHidden',
        fieldType: 'actions',
        header: 'Actions',
        width: '5rem'
      }
    ],
    []
  );

  return (
    <Suspense fallback={<div>Loading...</div>}>
      <DataSelectorValues
        noSourceHeader
        reorderable
        columns={columns}
        defaultSortField="name"
        defaultSortOrder={1}
        emptyMessage="No Streams"
        headerName={GetMessage('streams').toUpperCase()}
        dataSource={data}
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
