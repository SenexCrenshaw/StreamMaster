import { useChannelGroupColumnConfig } from '@components/columns/useChannelGroupColumnConfig';
import { useChannelNameColumnConfig } from '@components/columns/useChannelNameColumnConfig';
import { useChannelNumberColumnConfig } from '@components/columns/useChannelNumberColumnConfig';
import { useM3UFileNameColumnConfig } from '@components/columns/useM3UFileNameColumnConfig';
import DataSelector from '@components/dataSelector/DataSelector';
import { ColumnMeta } from '@components/dataSelector/DataSelectorTypes';
import { SMTriSelectShowHidden } from '@components/sm/SMTriSelectShowHidden';
import { GetMessage } from '@lib/common/common';

import { useSelectedStreamGroup } from '@lib/redux/slices/useSelectedStreamGroup';
import { skipToken } from '@reduxjs/toolkit/dist/query';
import { type DataTableRowClickEvent } from 'primereact/datatable';
import { memo, useMemo } from 'react';

interface StreamGroupVideoStreamDataSelectorProperties {
  readonly id: string;
}

const StreamGroupVideoStreamDataSelector = ({ id }: StreamGroupVideoStreamDataSelectorProperties) => {
  const dataKey = `${id}-StreamGroupVideoStreamDataSelector`;

  const { columnConfig: m3uFileNameColumnConfig } = useM3UFileNameColumnConfig({
    enableEdit: false
  });
  const { columnConfig: channelNumberColumnConfig } = useChannelNumberColumnConfig({ enableEdit: false });
  const { columnConfig: channelNameColumnConfig } = useChannelNameColumnConfig({
    enableEdit: false
  });
  const { columnConfig: channelGroupConfig } = useChannelGroupColumnConfig({
    enableEdit: false
  });
  const { selectedStreamGroup } = useSelectedStreamGroup(id);
  const streamGroupsGetStreamGroupVideoStreamIdsQuery = useStreamGroupVideoStreamsGetStreamGroupVideoStreamIdsQuery(selectedStreamGroup?.id ?? skipToken);
  const [streamGroupVideoStreamsSyncVideoStreamToStreamGroupMutation] = useStreamGroupVideoStreamsSyncVideoStreamToStreamGroupPostMutation();

  const SyncVideoStream = async (videoId: string) => {
    if (!videoId || !selectedStreamGroup) {
      return;
    }

    const toSend = {} as StreamGroupVideoStreamsSyncVideoStreamToStreamGroupPostApiArg;

    toSend.streamGroupId = selectedStreamGroup.id;
    toSend.videoStreamId = videoId;

    await streamGroupVideoStreamsSyncVideoStreamToStreamGroupMutation(toSend)
      .then(() => {})
      .catch((error) => {
        console.error(`Add Stream Error: ${error.message}`);
      });
  };

  const columns = useMemo(
    (): ColumnMeta[] => [channelNumberColumnConfig, channelNameColumnConfig, channelGroupConfig, m3uFileNameColumnConfig],
    [channelNumberColumnConfig, channelNameColumnConfig, channelGroupConfig, m3uFileNameColumnConfig]
  );

  const rightHeaderTemplate = useMemo(
    () => (
      <div className="flex justify-content-end align-items-center w-full gap-1">
        <SMTriSelectShowHidden dataKey={dataKey} />
      </div>
    ),
    [dataKey]
  );

  const onRowClick = async (event: DataTableRowClickEvent) => {
    await SyncVideoStream(event.data.id)
      .then(() => {})
      .catch((error) => {
        console.error(`Add Stream Error: ${error.message}`);
      });
  };

  return (
    <DataSelector
      columns={columns}
      defaultSortField="user_Tvg_name"
      defaultSortOrder={1}
      emptyMessage="No Streams"
      headerName={GetMessage('streams')}
      headerRightTemplate={rightHeaderTemplate}
      id={dataKey}
      isLoading={streamGroupsGetStreamGroupVideoStreamIdsQuery.isLoading || streamGroupsGetStreamGroupVideoStreamIdsQuery.isFetching}
      onRowClick={async (e) => await onRowClick(e)}
      queryFilter={useVideoStreamsGetPagedVideoStreamsQuery}
      selectedItemsKey="selectSelectedStreamGroupDtoItems"
      selectionMode="single"
      style={{ height: 'calc(100vh - 60px)' }}
      videoStreamIdsIsReadOnly={(streamGroupsGetStreamGroupVideoStreamIdsQuery.data ?? []).map((x) => x.videoStreamId ?? '')}
    />
  );
};

StreamGroupVideoStreamDataSelector.displayName = 'Stream Editor';

export default memo(StreamGroupVideoStreamDataSelector);
