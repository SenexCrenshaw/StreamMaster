import SMButton from '@components/sm/SMButton';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { Logger } from '@lib/common/logger';
import { useShowHidden } from '@lib/redux/hooks/showHidden';
import { RemoveSMStreamFromSMChannel, SetSMStreamRanks } from '@lib/smAPI/SMChannelStreamLinks/SMChannelStreamLinksCommands';
import { RemoveSMStreamFromSMChannelRequest, SetSMStreamRanksRequest, SMChannelDto, SMChannelRankRequest, SMStreamDto } from '@lib/smAPI/smapiTypes';
import { DataTableValue } from 'primereact/datatable';
import { memo, ReactNode, useCallback, useMemo, useState } from 'react';

interface SMChannelSMStreamDataSelectorProperties {
  readonly enableEdit?: boolean;
  readonly height?: string;
  readonly smChannel: SMChannelDto;
  readonly selectionKey: string;
}

const SMChannelSMStreamDataSelector = ({ selectionKey, height, smChannel }: SMChannelSMStreamDataSelectorProperties) => {
  // const { selectedItems, setSelectedItems } = useSelectedItems<SMStreamDto>(dataKey);
  const { showHidden } = useShowHidden(selectionKey);
  const [selectedItems, setSelectedItems] = useState<SMStreamDto[]>([]);

  const actionTemplate = useCallback(
    (smStream: SMStreamDto) => (
      <div className="flex align-content-center justify-content-center">
        <SMButton
          buttonClassName="icon-red"
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
              const newSelectedItems = selectedItems.filter((item) => item.Id !== smStream.Id).sort((a, b) => a.Rank - b.Rank);
              setSelectedItems(newSelectedItems);
            }
          }}
          tooltip="Remove Stream"
        />
      </div>
    ),
    [selectedItems, setSelectedItems, smChannel]
  );

  const addOrRemoveHeaderTemplate = useMemo((): ReactNode => {
    return <div className="flex align-content-center justify-content-center">{/* <SMTriSelectShowHidden dataKey={dataKey} />{' '} */}</div>;
  }, []);

  const columns = useMemo((): ColumnMeta[] => {
    const w = '12rem';
    const z = '4rem';

    return [
      { field: 'Name', maxWidth: w, minWidth: w, width: w },
      { field: 'M3UFileName', header: 'M3U', maxWidth: z, minWidth: z, width: z },
      {
        bodyTemplate: actionTemplate,
        field: 'custom',
        fieldType: 'custom',
        headerTemplate: addOrRemoveHeaderTemplate,
        maxWidth: '2rem',
        minWidth: '2rem',
        width: '2rem'
      }
    ];
  }, [actionTemplate, addOrRemoveHeaderTemplate]);

  const dataSource = useMemo(() => {
    if (!smChannel || !smChannel.SMStreams) {
      return [];
    }
    try {
      const toRet = [...smChannel.SMStreams];
      const smChannelData = toRet.sort((a, b) => a.Rank - b.Rank);
      if (showHidden === true) {
        return smChannelData.filter((a: SMStreamDto) => a.IsHidden !== true);
      }
      if (showHidden === false) {
        return smChannelData.filter((a: SMStreamDto) => a.IsHidden === true);
      }
      return smChannelData;
    } catch (e) {
      console.error(e);
    }
  }, [showHidden, smChannel]);

  if (!smChannel?.SMStreams) {
    return null;
  }

  if (!smChannel.SMStreams || !Array.isArray(smChannel.SMStreams)) {
    return null;
  }

  Logger.debug('SMChannelSMStreamDataSelector', selectionKey, smChannel.Name, { data: smChannel.SMStreams });
  return (
    <SMDataTable
      columns={columns}
      dataSource={dataSource}
      defaultSortField="Rank"
      defaultSortOrder={1}
      emptyMessage="No Streams"
      enablePaginator
      headerClassName="header-text-channels"
      headerName="ACTIVE STREAMS s"
      id="hey"
      // id={dataKey}
      // isLoading={smChannelIsLoading}
      onRowReorder={(event: DataTableValue[]) => {
        const channels = [...(event as unknown as SMStreamDto[])];

        const updatedChannels = channels.map((item, index) => {
          return {
            ...item,
            Rank: index
          };
        });

        const tosend: SMChannelRankRequest[] = updatedChannels.map((item, index) => {
          return { Rank: index, SMChannelId: smChannel.Id, SMStreamId: item.Id } as SMChannelRankRequest;
        });

        SetSMStreamRanks({ Requests: tosend } as SetSMStreamRanksRequest)
          .then((response) => {
            // console.log('SetSMStreamRanks', response);
            setSelectedItems(updatedChannels);
          })
          .catch((error) => {
            Logger.error('SetSMStreamRanks', error.message);
          });
      }}
      reorderable
      style={{ height: height ?? 'calc(100vh - 100px)' }}
    />
  );
};

export default memo(SMChannelSMStreamDataSelector);
