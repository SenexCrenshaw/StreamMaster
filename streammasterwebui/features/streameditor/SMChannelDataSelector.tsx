import MinusButton from '@components/buttons/MinusButton';
import { useSMChannelLogoColumnConfig } from '@components/columns/useSMChannelLogoColumnConfig';
import { useSMChannelNameColumnConfig } from '@components/columns/useSMChannelNameColumnConfig';
import { useSMChannelNumberColumnConfig } from '@components/columns/useSMChannelNumberColumnConfig';

import SMDataTable from '@components/smDataTable/SMDataTable';
import getRecord from '@components/smDataTable/helpers/getRecord';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import StreamCopyLinkDialog from '@components/smstreams/StreamCopyLinkDialog';
import { GetMessage } from '@lib/common/common';
import { useQueryFilter } from '@lib/redux/slices/useQueryFilter';
import { DeleteSMChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import useGetPagedSMChannels from '@lib/smAPI/SMChannels/useGetPagedSMChannels';
import { DeleteSMChannelRequest, SMChannelDto } from '@lib/smAPI/smapiTypes';
import { ConfirmPopup, confirmPopup } from 'primereact/confirmpopup';
import { DataTableRowClickEvent, DataTableRowData, DataTableRowEvent, DataTableRowExpansionTemplate, DataTableValue } from 'primereact/datatable';
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
        <SMStreamDataSelectorValue data={channel.smStreams} smChannel={channel} id={channel.id + '-streams'} />
      </div>
    );
  }, []);

  const setSelectedSMEntity = useCallback(
    (data: DataTableValue, toggle?: boolean) => {
      if (toggle === true && selectedSMChannel !== undefined && data !== undefined && data.id === selectedSMChannel.id) {
        setSelectedSMChannel(undefined);
      } else {
        setSelectedSMChannel(data as SMChannelDto);
      }
    },
    [selectedSMChannel, setSelectedSMChannel]
  );

  const actionBodyTemplate = useCallback((data: SMChannelDto) => {
    const accept = () => {
      DeleteSMChannel({ smChannelId: data.id } as DeleteSMChannelRequest)
        .then((response) => {
          console.log('Removed Channel');
        })
        .catch((error) => {
          console.error('Remove Channel', error.message);
        });
    };

    const reject = () => {};

    const confirm = (event: any) => {
      confirmPopup({
        target: event.currentTarget,
        message: (
          <>
            Delete
            <br />"{data.name}" ?
            <br />
            Are you sure?
          </>
        ),
        icon: 'pi pi-exclamation-triangle',
        defaultFocus: 'accept',
        accept,
        reject
      });
    };

    return (
      <div className="flex p-0 justify-content-end align-items-center">
        <StreamCopyLinkDialog realUrl={data?.realUrl} />
        <MinusButton iconFilled={false} onClick={confirm} tooltip="Remove Channel" />
      </div>
    );
  }, []);

  const columns = useMemo(
    (): ColumnMeta[] => [
      channelNumberColumnConfig,
      channelLogoColumnConfig,
      channelNameColumnConfig,
      { field: 'group', filter: false, sortable: true, width: '5rem' },
      { align: 'right', filter: false, bodyTemplate: actionBodyTemplate, field: 'actions', fieldType: 'actions', header: 'Actions', width: '5rem' }
    ],
    [actionBodyTemplate, channelLogoColumnConfig, channelNameColumnConfig, channelNumberColumnConfig]
  );

  const rowClass = useCallback(
    (data: unknown): string => {
      const isHidden = getRecord(data, 'isHidden');

      if (isHidden === true) {
        return 'bg-red-900';
      }

      if (selectedSMChannel !== undefined) {
        const id = getRecord(data, 'id') as number;
        if (id === selectedSMChannel.id) {
          return 'bg-orange-900';
        }
      }

      return '';
    },
    [selectedSMChannel]
  );

  const rightHeaderTemplate = useMemo(
    () => (
      <div className="flex justify-content-end align-items-center w-full gap-1">
        <div className="">
          {/* <M3UFilesButton /> */}
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
  return (
    <>
      <ConfirmPopup />
      <SMDataTable
        columns={columns}
        enableClick
        selectRow
        showExpand
        defaultSortField="name"
        defaultSortOrder={1}
        emptyMessage="No Channels"
        headerRightTemplate={rightHeaderTemplate}
        headerName={GetMessage('channels').toUpperCase()}
        id={dataKey}
        isLoading={isLoading}
        onRowClick={(e: DataTableRowClickEvent) => {
          setSelectedSMEntity(e.data as SMChannelDto, true);
          console.log(e);
        }}
        onClick={(e: any) => {
          if (e.target.className && e.target.className === 'p-datatable-wrapper') {
            setSelectedSMChannel(undefined);
          }
        }}
        onRowExpand={(e: DataTableRowEvent) => {
          setSelectedSMEntity(e.data);
        }}
        onRowCollapse={(e: DataTableRowEvent) => {
          setSelectedSMEntity(e.data);
        }}
        rowClass={rowClass}
        queryFilter={useGetPagedSMChannels}
        rowExpansionTemplate={rowExpansionTemplate}
        selectedItemsKey="selectSelectedSMChannelDtoItems"
        style={{ height: 'calc(100vh - 10px)' }}
      />
    </>
  );
};

export default memo(SMChannelDataSelector);
