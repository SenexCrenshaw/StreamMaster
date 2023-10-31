import { useChannelNameColumnConfig } from '@/components/columns/useChannelNameColumnConfig';
import { useChannelNumberColumnConfig } from '@/components/columns/useChannelNumberColumnConfig';
import { useEPGColumnConfig } from '@/components/columns/useEPGColumnConfig';
import DataSelector from '@/components/dataSelector/DataSelector';
import AutoSetChannelNumbers from '@/components/videoStream/AutoSetChannelNumbers';
import { ColumnMeta } from '@components/dataSelector/DataSelectorTypes';
import VideoStreamSetAutoSetEPGDialog from '@components/videoStream/VideoStreamSetAutoSetEPGDialog';
import { GroupIcon } from '@lib/common/Icons';
import { getColor } from '@lib/common/colors';
import { GetMessage, getChannelGroupMenuItem } from '@lib/common/common';
import { VideoStreamDto, useStreamGroupVideoStreamsGetPagedStreamGroupVideoStreamsQuery } from '@lib/iptvApi';
import { useSelectedStreamGroup } from '@lib/redux/slices/useSelectedStreamGroup';
import { Tooltip } from 'primereact/tooltip';
import { memo, useCallback, useMemo } from 'react';
import { v4 as uuidv4 } from 'uuid';
import StreamGroupChannelGroupsSelector from './StreamGroupChannelGroupsSelector';
import VideoStreamRemoveFromStreamGroupDialog from './VideoStreamRemoveFromStreamGroupDialog';

// const DataSelector = React.lazy(() => import('@components/dataSelector/DataSelector'));

interface StreamGroupSelectedVideoStreamDataSelectorProperties {
  readonly id: string;
}

const StreamGroupSelectedVideoStreamDataSelector = ({ id }: StreamGroupSelectedVideoStreamDataSelectorProperties) => {
  const dataKey = `${id}-StreamGroupSelectedVideoStreamDataSelector`;
  const { selectedStreamGroup } = useSelectedStreamGroup(id);

  const enableEdit = true;

  const { columnConfig: channelNumberColumnConfig } = useChannelNumberColumnConfig({ enableEdit, useFilter: false });
  const { columnConfig: channelNameColumnConfig } = useChannelNameColumnConfig({ enableEdit, useFilter: false });
  const { columnConfig: epgColumnConfig } = useEPGColumnConfig({ enableEdit, useFilter: false });

  const actionBodyTemplate = useCallback(
    (data: VideoStreamDto) => {
      if (data.isReadOnly === true) {
        const tooltipClassName = `grouptooltip-${uuidv4()}`;
        return (
          <div className="flex min-w-full min-h-full justify-content-end align-items-center">
            <Tooltip position="left" target={`.${tooltipClassName}`}>
              {getChannelGroupMenuItem(data.user_Tvg_group, data.user_Tvg_group)}
            </Tooltip>
            <GroupIcon className={tooltipClassName} style={{ color: getColor(data.user_Tvg_group) }} />
          </div>
        );
      }

      return (
        <div className="flex p-0 justify-content-end align-items-center">
          <VideoStreamSetAutoSetEPGDialog skipOverLayer id={id} values={[data]} />
          <VideoStreamRemoveFromStreamGroupDialog id={id} value={data} />
        </div>
      );
    },
    [id]
  );

  const columns = useMemo(
    (): ColumnMeta[] => [
      channelNumberColumnConfig,
      channelNameColumnConfig,
      epgColumnConfig,
      {
        bodyTemplate: actionBodyTemplate,
        field: 'Remove',
        header: '',
        resizeable: false,
        sortable: false,
        width: '3rem'
      }
    ],
    [channelNumberColumnConfig, channelNameColumnConfig, epgColumnConfig, actionBodyTemplate]
  );

  const rightHeaderTemplate = () => (
    <div className="flex justify-content-end align-items-center w-full gap-1">
      <StreamGroupChannelGroupsSelector streamGroupId={selectedStreamGroup?.id} />
      <AutoSetChannelNumbers streamGroupId={selectedStreamGroup?.id} id={dataKey} />
    </div>
  );

  return (
    <DataSelector
      columns={columns}
      defaultSortField="user_Tvg_name"
      emptyMessage="No Streams"
      headerName={GetMessage('streams')}
      headerRightTemplate={rightHeaderTemplate()}
      id={dataKey}
      key="rank"
      queryFilter={useStreamGroupVideoStreamsGetPagedStreamGroupVideoStreamsQuery}
      selectedItemsKey="selectSelectedStreamGroupDtoItems"
      selectedStreamGroupId={selectedStreamGroup?.id ?? 0}
      selectionMode="single"
      style={{ height: 'calc(100vh - 60px)' }}
    />
  );
};

StreamGroupSelectedVideoStreamDataSelector.displayName = 'Stream Editor';

export default memo(StreamGroupSelectedVideoStreamDataSelector);
