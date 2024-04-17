import { useSMChannelLogoColumnConfig } from '@components/columns/useSMChannelLogoColumnConfig';
import { useSMChannelNameColumnConfig } from '@components/columns/useSMChannelNameColumnConfig';
import { useSMChannelNumberColumnConfig } from '@components/columns/useSMChannelNumberColumnConfig';

import EPGFilesButton from '@components/epg/EPGFilesButton';
import SMDataTable from '@components/smDataTable/SMDataTable';
import getRecord from '@components/smDataTable/helpers/getRecord';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import StreamCopyLinkDialog from '@components/smstreams/StreamCopyLinkDialog';
import { GetMessage } from '@lib/common/common';
import { useQueryFilter } from '@lib/redux/slices/useQueryFilter';
import { DeleteSMChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import useGetPagedSMChannels from '@lib/smAPI/SMChannels/useGetPagedSMChannels';
import { DeleteSMChannelRequest, SMChannelDto } from '@lib/smAPI/smapiTypes';
import { DataTableRowClickEvent, DataTableRowData, DataTableRowEvent, DataTableRowExpansionTemplate } from 'primereact/datatable';
import { memo, useCallback, useEffect, useMemo, useState } from 'react';
import SMStreamDataSelectorValue from './SMStreamDataSelectorValue';
import useSelectedSMItems from './useSelectedSMItems';
import { SMPopUp } from '@components/sm/SMPopUp';

interface SMChannelDataSelectorProperties {
  readonly enableEdit?: boolean;
  readonly id: string;
  readonly reorderable?: boolean;
}

const SMChannelDataSelector = ({ enableEdit: propsEnableEdit, id, reorderable }: SMChannelDataSelectorProperties) => {
  const dataKey = `${id}-SMChannelDataSelector`;

  const { selectedSMChannel, setSelectedSMChannel } = useSelectedSMItems();
  const { queryFilter } = useQueryFilter(dataKey);
  const { isLoading } = useGetPagedSMChannels(queryFilter);
  const [enableEdit, setEnableEdit] = useState<boolean>(true);

  const { columnConfig: channelNumberColumnConfig } = useSMChannelNumberColumnConfig({ enableEdit, useFilter: false });
  const { columnConfig: channelLogoColumnConfig } = useSMChannelLogoColumnConfig({ enableEdit });
  const { columnConfig: channelNameColumnConfig } = useSMChannelNameColumnConfig({ enableEdit });

  useEffect(() => {
    if (propsEnableEdit !== enableEdit) {
      setEnableEdit(propsEnableEdit ?? true);
    }
  }, [enableEdit, propsEnableEdit]);

  const rowExpansionTemplate = useCallback((data: DataTableRowData<any>, options: DataTableRowExpansionTemplate) => {
    const channel = data as unknown as SMChannelDto;
    return (
      <div className="border-2 border-round-lg border-200 ml-3 m-1">
        <SMStreamDataSelectorValue smChannel={channel} id={channel.Id + '-streams'} />
      </div>
    );
  }, []);

  // const setSelectedSMEntity = useCallback(
  //   (data: DataTableValue, toggle?: boolean) => {
  //     if (toggle === true && selectedSMChannel !== undefined && data !== undefined && data.id === selectedSMChannel.Id) {
  //       setSelectedSMChannel(undefined);
  //     } else {
  //       setSelectedSMChannel(data as SMChannelDto);
  //     }
  //   },
  //   [selectedSMChannel, setSelectedSMChannel]
  // );

  const actionTemplate = useCallback(
    (data: SMChannelDto) => {
      const accept = () => {
        const toSend = {} as DeleteSMChannelRequest;
        toSend.SMChannelId = data.Id;
        DeleteSMChannel(toSend)
          .then((response) => {
            console.log('Removed Channel');
            if (selectedSMChannel?.Id === data.Id) {
              setSelectedSMChannel(undefined);
            }
          })
          .catch((error) => {
            console.error('Remove Channel', error.message);
          });
      };

      return (
        <div className="flex p-0 justify-content-end align-items-center">
          <StreamCopyLinkDialog realUrl={data?.RealUrl} />
          <SMPopUp title="Remove Channel" OK={() => accept()} icon="pi-minus" severity="danger">
            <div>
              "{data.Name}"
              <br />
              Are you sure?
            </div>
          </SMPopUp>
        </div>
      );
    },
    [selectedSMChannel?.Id, setSelectedSMChannel]
  );

  const columns = useMemo(
    (): ColumnMeta[] => [
      channelNumberColumnConfig,
      channelLogoColumnConfig,
      channelNameColumnConfig,
      { field: 'Group', filter: false, sortable: true, width: '5rem' },
      { align: 'right', bodyTemplate: actionTemplate, field: 'actions', fieldType: 'actions', filter: false, header: 'Actions', width: '5rem' }
    ],
    [actionTemplate, channelLogoColumnConfig, channelNameColumnConfig, channelNumberColumnConfig]
  );

  const rowClass = useCallback(
    (data: unknown): string => {
      const isHidden = getRecord(data, 'IsHidden');

      if (isHidden === true) {
        return 'bg-red-900';
      }

      if (selectedSMChannel !== undefined) {
        const id = getRecord(data, 'Id') as number;
        if (id === selectedSMChannel.Id) {
          return 'bg-orange-900';
        }
      }

      return '';
    },
    [selectedSMChannel]
  );

  const rightHeaderTemplate = useMemo(
    () => (
      <div className="flex flex-row justify-content-end align-items-center w-full gap-1">
        {/* <StreamMultiVisibleDialog iconFilled selectedItemsKey="selectSelectedSMStreamDtoItems" id={dataKey} skipOverLayer /> */}
        <div>
          <EPGFilesButton />
        </div>
        {/* <TriSelectShowHidden dataKey={dataKey} /> */}
        {/* <TriSelectShowHidden dataKey={dataKey} />
        <VideoStreamSetTimeShiftsDialog id={dataKey} />
        <VideoStreamResetLogosDialog id={dataKey} />
        <VideoStreamSetLogosFromEPGDialog id={dataKey} />
        <AutoSetChannelNumbers id={dataKey} />
        <VideoStreamVisibleDialog id={dataKey} />
        <VideoStreamSetAutoSetEPGDialog iconFilled id={dataKey} />
        <VideoStreamDeleteDialog iconFilled id={dataKey} />
        <VideoStreamAddDialog group={channelGroupNames?.[0]} /> */}
      </div>
    ),
    []
  );

  return (
    <SMDataTable
      columns={columns}
      enableClick
      selectRow
      showExpand
      defaultSortField="Name"
      defaultSortOrder={1}
      emptyMessage="No Channels"
      enablePaginator
      headerRightTemplate={rightHeaderTemplate}
      headerName={GetMessage('channels').toUpperCase()}
      id={dataKey}
      isLoading={isLoading}
      onRowClick={(e: DataTableRowClickEvent) => {
        // setSelectedSMEntity(e.data as SMChannelDto, true);
        if (e.data !== selectedSMChannel) {
          setSelectedSMChannel(e.data as SMChannelDto);
        }

        console.log(e);
      }}
      onClick={(e: any) => {
        if (e.target.className && e.target.className === 'p-datatable-wrapper') {
          setSelectedSMChannel(undefined);
        }
      }}
      onRowExpand={(e: DataTableRowEvent) => {
        if (e.data !== selectedSMChannel) {
          setSelectedSMChannel(e.data as SMChannelDto);
        }
        // setSelectedSMEntity(e.data);
      }}
      onRowCollapse={(e: DataTableRowEvent) => {
        if (e.data !== selectedSMChannel) {
          setSelectedSMChannel(e.data as SMChannelDto);
        }
        // setSelectedSMEntity(e.data);
      }}
      rowClass={rowClass}
      queryFilter={useGetPagedSMChannels}
      rowExpansionTemplate={rowExpansionTemplate}
      selectedItemsKey="selectSelectedSMChannelDtoItems"
      style={{ height: 'calc(100vh - 84px)' }}
    />
  );
};

export default memo(SMChannelDataSelector);
