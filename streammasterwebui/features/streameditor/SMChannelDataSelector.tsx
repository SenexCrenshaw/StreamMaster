import { useSMChannelEPGColumnConfig } from '@components/columns/SMChannel/useSMChannelEPGColumnConfig';
import { useSMChannelGroupColumnConfig } from '@components/columns/SMChannel/useSMChannelGroupColumnConfig';
import { useSMChannelLogoColumnConfig } from '@components/columns/SMChannel/useSMChannelLogoColumnConfig';
import { useSMChannelNameColumnConfig } from '@components/columns/SMChannel/useSMChannelNameColumnConfig';
import { useSMChannelNumberColumnConfig } from '@components/columns/SMChannel/useSMChannelNumberColumnConfig';
import { useSMChannelSGColumnConfig } from '@components/columns/SMChannel/useSMChannelSGColumnConfig';

import BaseButton from '@components/buttons/BaseButton';

import EPGFilesButton from '@components/epgFiles/EPGFilesButton';
import { SMPopUp } from '@components/sm/SMPopUp';
import getRecord from '@components/smDataTable/helpers/getRecord';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import CopySMChannelDialog from '@components/smchannels/CopySMChannelDialog';
import CreateSMChannelDialog from '@components/smchannels/CreateSMChannelDialog';
import EditSMChannelDialog from '@components/smchannels/EditSMChannelDialog';
import SMChannelMenu from '@components/smchannels/SMChannelMenu';
import StreamCopyLinkDialog from '@components/smstreams/StreamCopyLinkDialog';
import StreamGroupButton from '@components/streamGroup/StreamGroupButton';
import { GetMessage } from '@lib/common/common';
import { useQueryFilter } from '@lib/redux/slices/useQueryFilter';
import { DeleteSMChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import useGetPagedSMChannels from '@lib/smAPI/SMChannels/useGetPagedSMChannels';
import { DeleteSMChannelRequest, SMChannelDto } from '@lib/smAPI/smapiTypes';
import { DataTableRowClickEvent, DataTableRowData, DataTableRowEvent, DataTableRowExpansionTemplate } from 'primereact/datatable';
import { Suspense, lazy, memo, useCallback, useEffect, useMemo, useState } from 'react';
import SMStreamDataSelectorValue from './SMStreamDataSelectorValue';
import useSelectedSMItems from './useSelectedSMItems';
const SMDataTable = lazy(() => import('@components/smDataTable/SMDataTable'));
interface SMChannelDataSelectorProperties {
  readonly enableEdit?: boolean;
  readonly id: string;
  readonly reorderable?: boolean;
}

const SMChannelDataSelector = ({ enableEdit: propsEnableEdit, id, reorderable }: SMChannelDataSelectorProperties) => {
  const dataKey = `${id}-SMChannelDataSelector`;

  const { selectedSMChannel, setSelectedSMChannel } = useSelectedSMItems();
  const [enableEdit, setEnableEdit] = useState<boolean>(true);

  const { columnConfig: channelNumberColumnConfig } = useSMChannelNumberColumnConfig({ enableEdit, useFilter: false });
  const { columnConfig: channelLogoColumnConfig } = useSMChannelLogoColumnConfig({ enableEdit });
  const { columnConfig: channelNameColumnConfig } = useSMChannelNameColumnConfig({ enableEdit });
  const epgColumnConfig = useSMChannelEPGColumnConfig();
  const groupColumnConfig = useSMChannelGroupColumnConfig();
  const sgColumnConfig = useSMChannelSGColumnConfig();
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
          <CopySMChannelDialog label="Copy Channel" smChannel={data} />
          <SMPopUp title="Remove Channel" OK={() => accept()} icon="pi-times" severity="danger">
            <div>
              "{data.Name}"
              <br />
              Are you sure?
            </div>
          </SMPopUp>
          <EditSMChannelDialog smChannel={data} />
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
      groupColumnConfig,
      sgColumnConfig,
      { align: 'right', bodyTemplate: actionTemplate, field: 'actions', fieldType: 'actions', filter: false, header: 'Actions', width: '5rem' }
    ],
    [actionTemplate, channelLogoColumnConfig, channelNameColumnConfig, channelNumberColumnConfig, epgColumnConfig, groupColumnConfig, sgColumnConfig]
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
      <div className="flex flex-row justify-content-start align-items-center w-full gap-1 pr-2">
        <StreamGroupButton />

        <div className="flex flex-row justify-content-end align-items-center w-full gap-1 pr-2">
          <div>
            <EPGFilesButton />
          </div>

          <BaseButton className="icon-red-filled" icon="pi pi-times" rounded onClick={() => {}} />
          <CreateSMChannelDialog />
          <SMChannelMenu />
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

    return name;
  }, []);

  return (
    <Suspense>
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
    </Suspense>
  );
};

export default memo(SMChannelDataSelector);
