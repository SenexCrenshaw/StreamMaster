import { useSMChannelLogoColumnConfig } from '@components/columns/useSMChannelLogoColumnConfig';
import { useSMChannelNameColumnConfig } from '@components/columns/useSMChannelNameColumnConfig';
import { useSMChannelNumberColumnConfig } from '@components/columns/useSMChannelNumberColumnConfig';

import BaseButton from '@components/buttons/BaseButton';

import { useSMChannelEPGColumnConfig } from '@components/columns/useSMChannelEPGColumnConfig';
import EPGFilesButton from '@components/epgFiles/EPGFilesButton';
import { SMPopUp } from '@components/sm/SMPopUp';
import SMDataTable from '@components/smDataTable/SMDataTable';
import getRecord from '@components/smDataTable/helpers/getRecord';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import StreamCopyLinkDialog from '@components/smstreams/StreamCopyLinkDialog';
import StreamGroupButton from '@components/streamGroup/StreamGroupButton';
import { GetMessage } from '@lib/common/common';
import { useQueryFilter } from '@lib/redux/slices/useQueryFilter';
import { DeleteSMChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import useGetPagedSMChannels from '@lib/smAPI/SMChannels/useGetPagedSMChannels';
import { DeleteSMChannelRequest, SMChannelDto } from '@lib/smAPI/smapiTypes';
import { DataTableRowClickEvent, DataTableRowData, DataTableRowEvent, DataTableRowExpansionTemplate } from 'primereact/datatable';
import { memo, useCallback, useEffect, useMemo, useState } from 'react';
import SMStreamDataSelectorValue from './SMStreamDataSelectorValue';
import useSelectedSMItems from './useSelectedSMItems';

interface SMChannelDataSelectorProperties {
  readonly enableEdit?: boolean;
  readonly id: string;
  readonly reorderable?: boolean;
}

const SMChannelDataSelector = ({ enableEdit: propsEnableEdit, id, reorderable }: SMChannelDataSelectorProperties) => {
  const dataKey = `${id}-SMChannelDataSelector`;
  const { selectedSMChannel, setSelectedSMChannel } = useSelectedSMItems();
  // const { selectedStreamGroup } = useSelectedStreamGroup('StreamGroup');

  const [enableEdit, setEnableEdit] = useState<boolean>(true);

  const { columnConfig: channelNumberColumnConfig } = useSMChannelNumberColumnConfig({ enableEdit, useFilter: false });
  const { columnConfig: channelLogoColumnConfig } = useSMChannelLogoColumnConfig({ enableEdit });
  const { columnConfig: channelNameColumnConfig } = useSMChannelNameColumnConfig({ enableEdit });
  const { columnConfig: epgColumnConfig } = useSMChannelEPGColumnConfig({ enableEdit });
  // const { data: smChannelStreamsData } = useGetStreamGroupSMChannels({ StreamGroupId: selectedSMChannel?.Id } as GetStreamGroupSMChannelsRequest);

  const { queryFilter } = useQueryFilter(dataKey);
  const { isLoading } = useGetPagedSMChannels(queryFilter);

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
    [selectedSMChannel, setSelectedSMChannel]
  );

  const columns = useMemo(
    (): ColumnMeta[] => [
      channelNumberColumnConfig,
      channelLogoColumnConfig,
      channelNameColumnConfig,
      epgColumnConfig,
      { field: 'Group', filter: false, sortable: true, width: '5rem' },
      { align: 'right', bodyTemplate: actionTemplate, field: 'actions', fieldType: 'actions', filter: false, header: 'Actions', width: '5rem' }
    ],
    [actionTemplate, channelLogoColumnConfig, channelNameColumnConfig, channelNumberColumnConfig, epgColumnConfig]
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
      <div className="flex flex-row justify-content-start align-items-center w-full gap-2 pr-2">
        {/* <StreamMultiVisibleDialog iconFilled selectedItemsKey="selectSelectedSMStreamDtoItems" id={dataKey} skipOverLayer /> */}
        <div>
          <StreamGroupButton />
        </div>
        <div className="flex flex-row justify-content-end align-items-center w-full gap-2 pr-2">
          <div>
            <EPGFilesButton />
          </div>

          <BaseButton className="button-red" icon="pi pi-times" rounded onClick={() => {}} />
          <BaseButton className="button-yellow" icon="pi-plus" rounded onClick={() => {}} />
          <BaseButton className="button-orange" icon="pi pi-bars" rounded onClick={() => {}} />
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
      </div>
    ),
    []
  );

  const headerTitle = useCallback(() => {
    const name = GetMessage('channels').toUpperCase();
    // if (selectedStreamGroup?.Name) {
    //   return name + ' - ' + selectedStreamGroup.Name;
    // }
    return name;
  }, []);

  // const addOrRemoveTemplate = useCallback(
  //   (data: any) => {
  //     if (selectedSMChannel?.Name === undefined) {
  //       return;
  //     }
  //     // const found = smChannelStreamsData?.some((item) => item.Id === data.Id) ?? false;

  //     // let toolTip = 'Add Channel';

  //     // toolTip = 'Remove Stream From "' + selectedSMChannel.Name + '"';
  //     // if (found)
  //     //   return (
  //     //     <div className="flex justify-content-between align-items-center p-0 m-0 pl-1">
  //     //       <MinusButton
  //     //         iconFilled={false}
  //     //         onClick={() => {
  //     //           if (!data.Id || selectedSMChannel === undefined) {
  //     //             return;
  //     //           }
  //     //           const request: RemoveSMStreamFromSMChannelRequest = { SMChannelId: selectedSMChannel.Id, SMStreamId: data.Id };
  //     //           // RemoveSMStreamFromSMChannel(request)
  //     //           //   .then((response) => {
  //     //           //     console.log('Remove Stream', response);
  //     //           //   })
  //     //           //   .catch((error) => {
  //     //           //     console.error('Remove Stream', error.message);
  //     //           //   });
  //     //         }}
  //     //         tooltip={toolTip}
  //     //       />
  //     //     </div>
  //     //   );

  //     let toolTip = 'Add Stream To "' + selectedSMChannel.Name + '"';
  //     return (
  //       <div className="flex justify-content-between align-items-center p-0 m-0 pl-1">
  //         <AddButton
  //           iconFilled={false}
  //           onClick={() => {
  //             // AddSMStreamToSMChannel({ SMChannelId: selectedSMChannel?.Id ?? 0, SMStreamId: data.Id })
  //             //   .then((response) => {})
  //             //   .catch((error) => {
  //             //     console.error(error.message);
  //             //     throw error;
  //             //   });
  //           }}
  //           tooltip={toolTip}
  //         />

  //         {/* {showSelection && <Checkbox checked={isSelected} className="pl-1" onChange={() => addSelection(data)} />} */}
  //       </div>
  //     );
  //   },
  //   [selectedSMChannel]
  // );

  // function addOrRemoveHeaderTemplate() {
  //   return <TriSelectShowHidden dataKey={dataKey} />;
  // }

  return (
    <SMDataTable
      // addOrRemoveTemplate={addOrRemoveTemplate}
      // addOrRemoveHeaderTemplate={addOrRemoveHeaderTemplate}
      columns={columns}
      enableClick
      selectRow
      showExpand
      defaultSortField="Name"
      defaultSortOrder={1}
      emptyMessage="No Channels"
      enablePaginator
      headerRightTemplate={rightHeaderTemplate}
      headerName={headerTitle()}
      id={dataKey}
      isLoading={isLoading}
      onRowClick={(e: DataTableRowClickEvent) => {
        // setSelectedSMEntity(e.data as SMChannelDto, true);
        // if (e.data !== selectedSMChannel) {
        //   setSelectedSMChannel(e.data as SMChannelDto);
        // }
        //console.log(e);
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
      style={{ height: 'calc(100vh - 100px)' }}
    />
  );
};

export default memo(SMChannelDataSelector);
