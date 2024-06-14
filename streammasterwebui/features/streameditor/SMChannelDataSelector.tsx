import EPGFilesButton from '@components/epgFiles/EPGFilesButton';
import { SMTriSelectShowHidden } from '@components/sm/SMTriSelectShowHidden';
import SMDataTable from '@components/smDataTable/SMDataTable';
import getRecord from '@components/smDataTable/helpers/getRecord';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import AutoSetEPGSMChannelDialog from '@components/smchannels/AutoSetEPGSMChannelDialog';
import CloneSMChannelDialog from '@components/smchannels/CloneSMChannelDialog';
import CreateSMChannelFromSMStreamDialog from '@components/smchannels/CreateSMChannelDialog';
import DeleteSMChannelDialog from '@components/smchannels/DeleteSMChannelDialog';
import DeleteSMChannelsDialog from '@components/smchannels/DeleteSMChannelsDialog';
import EditSMChannelDialog from '@components/smchannels/EditSMChannelDialog';
import SMChannelMultiVisibleDialog from '@components/smchannels/SMChannelMultiVisibleDialog';
import SetSMChannelsLogoFromEPGDialog from '@components/smchannels/SetSMChannelsLogoFromEPGDialog';
import { useSMChannelEPGColumnConfig } from '@components/smchannels/columns/useSMChannelEPGColumnConfig';
import { useSMChannelGroupColumnConfig } from '@components/smchannels/columns/useSMChannelGroupColumnConfig';
import { useSMChannelLogoColumnConfig } from '@components/smchannels/columns/useSMChannelLogoColumnConfig';
import { useSMChannelNameColumnConfig } from '@components/smchannels/columns/useSMChannelNameColumnConfig';
import { useSMChannelNumberColumnConfig } from '@components/smchannels/columns/useSMChannelNumberColumnConfig';
import { useSMChannelProxyColumnConfig } from '@components/smchannels/columns/useSMChannelProxyColumnConfig';
import { useSMChannelSGColumnConfig } from '@components/smchannels/columns/useSMChannelSGColumnConfig';
import StreamCopyLinkDialog from '@components/smstreams/StreamCopyLinkDialog';
import StreamGroupButton from '@components/streamGroup/StreamGroupButton';
import { GetMessage } from '@lib/common/intl';
import { useIsTrue } from '@lib/redux/hooks/isTrue';
import { useQueryFilter } from '@lib/redux/hooks/queryFilter';
import useGetPagedSMChannels from '@lib/smAPI/SMChannels/useGetPagedSMChannels';
import { SMChannelDto } from '@lib/smAPI/smapiTypes';
import { DataTableRowData, DataTableRowEvent, DataTableRowExpansionTemplate } from 'primereact/datatable';
import { memo, useCallback, useEffect, useMemo, useState } from 'react';
import SMChannelMenu from './SMChannelMenu';
import SMStreamDataSelectorValue from './SMStreamDataSelectorValue';
import useSelectedSMItems from './useSelectedSMItems';

interface SMChannelDataSelectorProperties {
  readonly enableEdit?: boolean;
  readonly id: string;
}

const SMChannelDataSelector = ({ enableEdit: propsEnableEdit, id }: SMChannelDataSelectorProperties) => {
  const dataKey = `${id}-SMChannelDataSelector`;

  const { isTrue: smTableIsSimple } = useIsTrue('streameditor-SMStreamDataSelector');
  const { selectedSMChannel, setSelectedSMChannel } = useSelectedSMItems();
  const [enableEdit, setEnableEdit] = useState<boolean>(true);
  const { columnConfig: channelNumberColumnConfig } = useSMChannelNumberColumnConfig({ enableEdit, useFilter: false });
  const { columnConfig: channelLogoColumnConfig } = useSMChannelLogoColumnConfig({ enableEdit });
  const channelNameColumnConfig = useSMChannelNameColumnConfig({ width: smTableIsSimple ? 200 : 125 });
  const epgColumnConfig = useSMChannelEPGColumnConfig({ width: smTableIsSimple ? 150 : 125 });
  const groupColumnConfig = useSMChannelGroupColumnConfig({ dataKey, width: smTableIsSimple ? 200 : 125 });
  const sgColumnConfig = useSMChannelSGColumnConfig({ dataKey: dataKey + '-sg', id: dataKey });
  const { columnConfig: proxyColumnConfig } = useSMChannelProxyColumnConfig({ enableEdit, useFilter: false });
  const { queryFilter } = useQueryFilter(dataKey);
  const { isLoading } = useGetPagedSMChannels(queryFilter);

  useEffect(() => {
    if (propsEnableEdit !== enableEdit) {
      setEnableEdit(propsEnableEdit ?? true);
    }
  }, [enableEdit, propsEnableEdit]);

  const rowExpansionTemplate = useCallback(
    (data: DataTableRowData<any>, options: DataTableRowExpansionTemplate) => {
      const channel = data as unknown as SMChannelDto;
      setSelectedSMChannel(channel);

      return (
        <div className="ml-3 m-1">
          <SMStreamDataSelectorValue smChannel={channel} id={channel.Id + '-streams'} />
        </div>
      );
    },
    [setSelectedSMChannel]
  );

  const actionTemplate = useCallback((data: SMChannelDto) => {
    return (
      <div className="flex justify-content-end align-items-center" style={{ paddingRight: '0.1rem' }}>
        <StreamCopyLinkDialog realUrl={data?.RealUrl} />
        <DeleteSMChannelDialog smChannel={data} />
        <EditSMChannelDialog smChannelDto={data} />
      </div>
    );
  }, []);

  const simpleActionTemplate = useCallback((data: SMChannelDto) => {
    return (
      <div className="flex justify-content-end align-items-center" style={{ paddingRight: '0.1rem' }}>
        <StreamCopyLinkDialog realUrl={data?.RealUrl} />
        <SetSMChannelsLogoFromEPGDialog smChannel={data} />
        <AutoSetEPGSMChannelDialog smChannel={data} />
        <CloneSMChannelDialog label="Copy Channel" smChannel={data} />
        <DeleteSMChannelDialog smChannel={data} />
        <EditSMChannelDialog smChannelDto={data} />
      </div>
    );
  }, []);

  const simpleColumns = useMemo(
    (): ColumnMeta[] => [
      channelNumberColumnConfig,
      channelLogoColumnConfig,
      channelNameColumnConfig,
      epgColumnConfig,
      groupColumnConfig,
      proxyColumnConfig,
      sgColumnConfig,
      {
        align: 'right',
        bodyTemplate: simpleActionTemplate,
        field: 'IsHidden',
        fieldType: 'actions',
        header: 'Actions',
        width: 100
      }
    ],
    [
      channelNumberColumnConfig,
      channelLogoColumnConfig,
      channelNameColumnConfig,
      epgColumnConfig,
      groupColumnConfig,
      proxyColumnConfig,
      sgColumnConfig,
      simpleActionTemplate
    ]
  );

  const columns = useMemo(
    (): ColumnMeta[] => [
      channelNumberColumnConfig,
      channelLogoColumnConfig,
      channelNameColumnConfig,
      epgColumnConfig,
      groupColumnConfig,
      sgColumnConfig,
      { align: 'right', bodyTemplate: actionTemplate, field: 'IsHidden', fieldType: 'actions', header: 'Actions', width: 52 }
    ],
    [actionTemplate, channelLogoColumnConfig, channelNameColumnConfig, channelNumberColumnConfig, epgColumnConfig, groupColumnConfig, sgColumnConfig]
  );

  const rowClass = useCallback(
    (data: unknown): string => {
      const isHidden = getRecord(data, 'IsHidden');

      if (selectedSMChannel !== undefined) {
        const id = getRecord(data, 'Id') as number;
        if (id === selectedSMChannel.Id) {
          if (isHidden === true) {
            return 'channel-row-selected-hidden';
          }
          return 'channel-row-selected';
        }
      }

      if (isHidden === true) {
        return 'bg-red-900';
      }

      return '';
    },
    [selectedSMChannel]
  );

  const headerCenterTemplate = useMemo(() => {
    if (smTableIsSimple) {
      return <StreamGroupButton className="sm-w-20rem" />;
    }
    return <StreamGroupButton />;

    // return <StreamGroupButton />;
  }, [smTableIsSimple]);

  const headerRightTemplate = useMemo(
    () => (
      <div className="flex flex-row justify-content-end align-items-center gap-1">
        <EPGFilesButton />
        <SMChannelMultiVisibleDialog iconFilled selectedItemsKey="selectSelectedSMChannelDtoItems" id={dataKey} skipOverLayer />
        <DeleteSMChannelsDialog selectedItemsKey="selectSelectedSMChannelDtoItems" id={dataKey} />
        <CreateSMChannelFromSMStreamDialog />
        <SMChannelMenu />
      </div>
    ),
    [dataKey]
  );

  const headerTitle = useCallback(() => {
    const name = GetMessage('channels').toUpperCase();

    return name;
  }, []);

  return (
    <SMDataTable
      columns={smTableIsSimple ? simpleColumns : columns}
      enableClick
      selectRow
      showExpand
      defaultSortField="Name"
      defaultSortOrder={1}
      emptyMessage="No Channels"
      enablePaginator
      expanderHeader={() => (
        <div className="flex align-content-center justify-content-center">
          <SMTriSelectShowHidden dataKey={dataKey} />
        </div>
      )}
      headerCenterTemplate={headerCenterTemplate}
      headerRightTemplate={headerRightTemplate}
      headerName={headerTitle()}
      id={dataKey}
      isLoading={isLoading}
      onRowExpand={(e: DataTableRowEvent) => {
        if (e.data.Id !== selectedSMChannel?.Id) {
          setSelectedSMChannel(e.data as SMChannelDto);
        }
      }}
      onRowCollapse={(e: DataTableRowEvent) => {
        if (e.data.Id === selectedSMChannel?.Id) {
          setSelectedSMChannel(undefined);
        }
      }}
      rowClass={rowClass}
      queryFilter={useGetPagedSMChannels}
      rowExpansionTemplate={rowExpansionTemplate}
      selectionMode="multiple"
      selectedItemsKey="selectSelectedSMChannelDtoItems"
      style={{ height: 'calc(100vh - 100px)' }}
    />
  );
};

export default memo(SMChannelDataSelector);
