import SMButton from '@components/sm/SMButton';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { Logger } from '@lib/common/logger';
import { useShowHidden } from '@lib/redux/hooks/showHidden';
import { RemoveSMChannelFromSMChannel, SetSMChannelRanks } from '@lib/smAPI/SMChannelChannelLinks/SMChannelChannelLinksCommands';
import { RemoveSMChannelFromSMChannelRequest, SetSMChannelRanksRequest, SMChannelChannelRankRequest, SMChannelDto } from '@lib/smAPI/smapiTypes';
import { DataTableValue } from 'primereact/datatable';
import { memo, ReactNode, useCallback, useMemo, useState } from 'react';

interface SMChannelSMChannelDataSelectorProperties {
  readonly enableEdit?: boolean;
  readonly height?: string;
  readonly smChannel: SMChannelDto;
  readonly selectionKey: string;
}

const SMChannelSMChannelDataSelector = ({ selectionKey, height, smChannel }: SMChannelSMChannelDataSelectorProperties) => {
  const { showHidden } = useShowHidden(selectionKey);
  const [selectedItems, setSelectedItems] = useState<SMChannelDto[]>([]);

  const actionTemplate = useCallback(
    (smChannelDto: SMChannelDto) => (
      <div className="flex align-content-center justify-content-center">
        <SMButton
          buttonClassName="icon-red"
          icon="pi-minus"
          iconFilled={false}
          onClick={() => {
            if (smChannelDto) {
              const request: RemoveSMChannelFromSMChannelRequest = { ChildSMChannelId: smChannel.Id, ParentSMChannelId: smChannelDto.Id };
              RemoveSMChannelFromSMChannel(request)
                .then((response) => {})
                .catch((error) => {
                  console.error('Remove Channel', error.message);
                });
            } else {
              const newSelectedItems = selectedItems.filter((item) => item.Id !== smChannel.Id).sort((a, b) => a.Rank - b.Rank);
              setSelectedItems(newSelectedItems);
            }
          }}
          tooltip="Remove Channel"
        />
      </div>
    ),
    [selectedItems, smChannel.Id]
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
    if (!smChannel || !smChannel.SMChannels) {
      return [];
    }
    try {
      const toRet = [...smChannel.SMChannels];
      const smChannelData = toRet.sort((a, b) => a.Rank - b.Rank);
      if (showHidden === true) {
        return smChannelData.filter((a: SMChannelDto) => a.IsHidden !== true);
      }
      if (showHidden === false) {
        return smChannelData.filter((a: SMChannelDto) => a.IsHidden === true);
      }
      return smChannelData;
    } catch (e) {
      console.error(e);
    }
  }, [showHidden, smChannel]);

  if (!smChannel?.SMChannels) {
    return null;
  }

  if (!smChannel.SMChannels || !Array.isArray(smChannel.SMChannels)) {
    return null;
  }

  Logger.debug('SMChannelSMChannelDataSelector', selectionKey, smChannel.Name, { data: smChannel.SMChannels });
  return (
    <SMDataTable
      columns={columns}
      dataSource={dataSource}
      defaultSortField="Rank"
      defaultSortOrder={1}
      emptyMessage="No Channels"
      enablePaginator
      headerClassName="header-text-channels"
      headerName="ACTIVE Channels s"
      id="hey"
      // id={dataKey}
      // isLoading={smChannelIsLoading}
      onRowReorder={(event: DataTableValue[]) => {
        const channels = [...(event as unknown as SMChannelDto[])];

        const updatedChannels = channels.map((item, index) => {
          return {
            ...item,
            Rank: index
          };
        });

        const tosend: SMChannelChannelRankRequest[] = updatedChannels.map((item, index) => {
          return { ChildSMChannelId: item.Id, ParentSMChannelId: smChannel.Id, Rank: index } as SMChannelChannelRankRequest;
        });

        SetSMChannelRanks({ Requests: tosend } as SetSMChannelRanksRequest)
          .then((response) => {
            // console.log('SetSMStreamRanks', response);
            setSelectedItems(updatedChannels);
          })
          .catch((error) => {
            Logger.error('SetSMChannelRanks', error.message);
          });
      }}
      reorderable
      style={{ height: height ?? 'calc(100vh - 100px)' }}
    />
  );
};

export default memo(SMChannelSMChannelDataSelector);
