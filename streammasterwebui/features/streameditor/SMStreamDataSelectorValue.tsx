import SMButton from '@components/sm/SMButton';

import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { GetMessage } from '@lib/common/intl';
import { RemoveSMStreamFromSMChannel, SetSMStreamRanks } from '@lib/smAPI/SMChannelStreamLinks/SMChannelStreamLinksCommands';

import { RemoveSMStreamFromSMChannelRequest, SMChannelDto, SMChannelRankRequest, SMStreamDto, SetSMStreamRanksRequest } from '@lib/smAPI/smapiTypes';
import { DataTableValue } from 'primereact/datatable';
import { memo, useCallback, useMemo } from 'react';
interface SMStreamDataSelectorValueProperties {
  readonly id: string;
  readonly smChannel: SMChannelDto;
}

const SMStreamDataSelectorValue = ({ id, smChannel }: SMStreamDataSelectorValueProperties) => {
  const dataKey = `${id}-SMStreamDataSelectorValue`;

  const actionTemplate = useCallback(
    (smStream: SMStreamDto) => (
      <div className="flex align-content-center justify-content-end">
        <SMButton
          icon="pi-minus"
          buttonClassName="w-2rem border-noround icon-red"
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
      { field: 'Name', sortable: false },
      { field: 'Group', sortable: false, width: '5rem' },
      { field: 'M3UFileName', header: 'M3U', sortable: false, width: '5rem' },
      {
        align: 'right',
        bodyTemplate: actionTemplate,
        field: '',
        fieldType: 'actions',
        header: 'Actions',
        width: '5rem'
      }
    ],
    [actionTemplate]
  );

  if (!smChannel?.SMStreams) {
    return null;
  }

  return (
    <div
      onClick={() => {
        if (smChannel) {
        }
      }}
    >
      <SMDataTable
        headerSize="small"
        enablePaginator
        rows={5}
        noSourceHeader
        reorderable
        columns={columns}
        defaultSortField="rank"
        defaultSortOrder={1}
        dataSource={smChannel.SMStreams}
        emptyMessage="No Streams"
        headerName={GetMessage('streams').toUpperCase()}
        // isLoading={isLoading}
        onRowReorder={(event: DataTableValue[]) => {
          const channels = event as unknown as SMStreamDto[];
          if (smChannel === undefined || channels === undefined) {
            return;
          }

          const tosend: SMChannelRankRequest[] = channels.map((item, index) => {
            return { Rank: index, SMChannelId: smChannel.Id, SMStreamId: item.Id } as SMChannelRankRequest;
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
        id={dataKey}
        selectedItemsKey={'SMStreamDataSelectorValue-selectSelectedSMStreamDtoItems'}
      />
    </div>
  );
};

export default memo(SMStreamDataSelectorValue);
