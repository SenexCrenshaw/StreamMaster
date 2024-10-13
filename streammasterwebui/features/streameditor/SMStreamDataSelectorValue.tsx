import SMButton from '@components/sm/SMButton';

import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { GetMessage } from '@lib/common/intl';
import { Logger } from '@lib/common/logger';
import { RemoveSMStreamFromSMChannel, SetSMStreamRanks } from '@lib/smAPI/SMChannelStreamLinks/SMChannelStreamLinksCommands';

import { RemoveSMStreamFromSMChannelRequest, SMChannelDto, SMChannelStreamRankRequest, SMStreamDto, SetSMStreamRanksRequest } from '@lib/smAPI/smapiTypes';
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
    ),
    [smChannel]
  );

  const columns = useMemo(
    (): ColumnMeta[] => [
      { field: 'Name', sortable: false, width: 300 },
      { field: 'Group', sortable: false, width: 200 },
      { field: 'M3UFileName', header: 'M3U', sortable: false, width: 125 },
      {
        align: 'center',
        bodyTemplate: actionTemplate,
        field: '',
        fieldType: 'actions',

        width: 22
      }
    ],
    [actionTemplate]
  );

  if (!smChannel?.SMStreamDtos) {
    return null;
  }

  Logger.debug('SMStreamDataSelectorValue', smChannel.SMStreamDtos);

  return (
    <div
      onClick={() => {
        if (smChannel) {
        }
      }}
    >
      <SMDataTable
        columns={columns}
        dataSource={smChannel.SMStreamDtos}
        defaultSortField="Rank"
        defaultSortOrder={1}
        emptyMessage="No Streams"
        enablePaginator={false}
        headerName={GetMessage('streams').toUpperCase()}
        headerSize="small"
        id={dataKey}
        noSourceHeader
        onRowReorder={(event: DataTableValue[]) => {
          const channels = event as unknown as SMStreamDto[];
          if (smChannel === undefined || channels === undefined) {
            return;
          }

          const tosend: SMChannelStreamRankRequest[] = channels.map((item, index) => {
            return { Rank: index, SMChannelId: smChannel.Id, SMStreamId: item.Id } as SMChannelStreamRankRequest;
          });

          SetSMStreamRanks({ Requests: tosend } as SetSMStreamRanksRequest)
            .then((response) => {
              // console.log('SetSMStreamRanks', response);
            })
            .catch((error) => {
              Logger.error('SetSMStreamRanks', error.message);
            });

          // console.log('tosend', tosend);
        }}
        reorderable
        rows={5}
      />
    </div>
  );
};

export default memo(SMStreamDataSelectorValue);
