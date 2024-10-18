import SMButton from '@components/sm/SMButton';

import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { GetMessage } from '@lib/common/intl';
import { Logger } from '@lib/common/logger';
import { RemoveSMChannelFromSMChannel, SetSMChannelRanks } from '@lib/smAPI/SMChannelChannelLinks/SMChannelChannelLinksCommands';

import { RemoveSMChannelFromSMChannelRequest, SMChannelChannelRankRequest, SMChannelDto, SetSMChannelRanksRequest } from '@lib/smAPI/smapiTypes';
import { DataTableValue } from 'primereact/datatable';
import { memo, useCallback, useMemo } from 'react';
interface SMChannelDataSelectorValueProperties {
  readonly id: string;
  readonly smChannel: SMChannelDto;
}

const SMChannelDataSelectorValue = ({ id, smChannel }: SMChannelDataSelectorValueProperties) => {
  const dataKey = `${id}-SMChannelDataSelectorValue`;

  const actionTemplate = useCallback(
    (smChannelDto: SMChannelDto) => (
      <SMButton
        icon="pi-minus"
        buttonClassName="w-2rem border-noround icon-red"
        iconFilled={false}
        onClick={() => {
          if (!smChannel.Id || smChannel === undefined) {
            return;
          }

          const request: RemoveSMChannelFromSMChannelRequest = { ChildSMChannelId: smChannelDto.Id, ParentSMChannelId: smChannel.Id };
          RemoveSMChannelFromSMChannel(request)
            .then((response) => {
              console.log('Remove Channel', response);
            })
            .catch((error) => {
              console.error('Remove Channel', error.message);
            });
        }}
        tooltip="Remove Channel"
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

  Logger.debug('SMChannelDataSelectorValue', smChannel.SMChannelDtos);

  if (!smChannel?.SMChannelDtos) {
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
        columns={columns}
        dataSource={smChannel.SMChannelDtos}
        defaultSortField="Rank"
        defaultSortOrder={1}
        emptyMessage="No Channels"
        enablePaginator={false}
        headerName={GetMessage('channels').toUpperCase()}
        headerSize="small"
        id={dataKey}
        noSourceHeader
        onRowReorder={(event: DataTableValue[]) => {
          const channels = event as unknown as SMChannelDto[];
          if (smChannel === undefined || channels === undefined) {
            return;
          }

          const tosend: SMChannelChannelRankRequest[] = channels.map((item, index) => {
            return { ChildSMChannelId: item.Id, ParentSMChannelId: smChannel.Id, Rank: index } as SMChannelChannelRankRequest;
          });

          SetSMChannelRanks({ Requests: tosend } as SetSMChannelRanksRequest)
            .then((response) => {
              // console.log('SetSMChannelRanks', response);
            })
            .catch((error) => {
              Logger.error('SetSMChannelRanks', error.message);
            });

          // console.log('tosend', tosend);
        }}
        reorderable
        rows={5}
      />
    </div>
  );
};

export default memo(SMChannelDataSelectorValue);
