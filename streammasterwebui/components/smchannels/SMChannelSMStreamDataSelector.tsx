import SMButton from '@components/sm/SMButton';
import { SMTriSelectShowHidden } from '@components/sm/SMTriSelectShowHidden';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';

import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { useShowHidden } from '@lib/redux/hooks/showHidden';
import { RemoveSMStreamFromSMChannel, SetSMStreamRanks } from '@lib/smAPI/SMChannelStreamLinks/SMChannelStreamLinksCommands';
import useGetSMChannelStreams from '@lib/smAPI/SMChannelStreamLinks/useGetSMChannelStreams';
import {
  GetSMChannelStreamsRequest,
  RemoveSMStreamFromSMChannelRequest,
  SMChannelDto,
  SMChannelRankRequest,
  SMStreamDto,
  SetSMStreamRanksRequest
} from '@lib/smAPI/smapiTypes';
import { DataTableValue } from 'primereact/datatable';
import { ReactNode, memo, useCallback, useMemo } from 'react';

interface SMChannelSMStreamDataSelectorProperties {
  readonly enableEdit?: boolean;
  readonly height?: string;
  readonly id?: string;
  readonly name: string | undefined;
  readonly smChannel?: SMChannelDto;
}

const SMChannelSMStreamDataSelector = ({ enableEdit: propsEnableEdit, height, id, name, smChannel }: SMChannelSMStreamDataSelectorProperties) => {
  const dataKey = `${id}-SMChannelSMStreamDataSelector`;
  const { selectedItems, setSelectedItems } = useSelectedItems<SMStreamDto>(`${id}-SMChannelSMStreamDataSelector`);
  const { showHidden } = useShowHidden(dataKey);
  // const [enableEdit, setEnableEdit] = useState<boolean>(true);
  const { data: smChannelData, isLoading: smChannelIsLoading } = useGetSMChannelStreams({ SMChannelId: smChannel?.Id } as GetSMChannelStreamsRequest);

  // useEffect(() => {
  //   if (propsEnableEdit !== enableEdit) {
  //     setEnableEdit(propsEnableEdit ?? true);
  //   }
  // }, [enableEdit, propsEnableEdit]);

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
              const newSelectedItems = selectedItems.filter((item) => item.Id !== smStream.Id);
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
    return (
      <div className="flex align-content-center justify-content-center">
        <SMTriSelectShowHidden dataKey={dataKey} />{' '}
      </div>
    );
  }, [dataKey]);

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

  const getSMChannelData = useMemo(() => {
    if (!smChannelData) {
      return undefined;
    }

    if (showHidden === true) {
      return smChannelData.filter((a: SMStreamDto) => a.IsHidden !== true);
    }

    if (showHidden === false) {
      return smChannelData.filter((a: SMStreamDto) => a.IsHidden === true);
    }

    return smChannelData;
  }, [showHidden, smChannelData]);

  if (smChannel) {
    return (
      <SMDataTable
        columns={columns}
        defaultSortField="Rank"
        defaultSortOrder={1}
        reorderable
        onRowReorder={(event: DataTableValue[]) => {
          const channels = [...(event as unknown as SMStreamDto[])];

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
        }}
        dataSource={getSMChannelData}
        enablePaginator
        emptyMessage="No Streams"
        headerName="ACTIVE STREAMS"
        headerClassName="header-text-channels"
        isLoading={smChannelIsLoading}
        id={dataKey}
        style={{ height: height ?? 'calc(100vh - 100px)' }}
      />
    );
  }

  return (
    <SMDataTable
      columns={columns}
      defaultSortField="Rank"
      defaultSortOrder={1}
      reorderable
      onRowReorder={(event: DataTableValue[]) => {
        const channels = [...(event as unknown as SMStreamDto[])];
        const updatedChannels = channels.map((item, index) => {
          return {
            ...item,
            Rank: index
          };
        });

        setSelectedItems(updatedChannels);
      }}
      dataSource={selectedItems}
      enablePaginator
      emptyMessage="No Streams"
      headerName="ACTIVE STREAMS"
      headerClassName="header-text-channels"
      isLoading={false}
      id={dataKey}
      style={{ height: height ?? 'calc(100vh - 100px)' }}
    />
  );
};

export default memo(SMChannelSMStreamDataSelector);
