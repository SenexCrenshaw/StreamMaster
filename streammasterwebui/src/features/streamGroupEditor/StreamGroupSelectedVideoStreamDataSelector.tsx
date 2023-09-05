

import { useCallback, type CSSProperties, useEffect } from "react";
import { useMemo, memo } from "react";
import { GetMessage } from "../../common/common";
import { type UpdateStreamGroupRequest, type VideoStreamIsReadOnly } from "../../store/iptvApi";
import { type ChildVideoStreamDto, useStreamGroupsUpdateStreamGroupMutation } from "../../store/iptvApi";

import { useStreamGroupVideoStreamsGetStreamGroupVideoStreamsQuery, type StreamGroupDto } from "../../store/iptvApi";
import { type VideoStreamDto } from "../../store/iptvApi";
import { useChannelGroupColumnConfig, useM3UFileNameColumnConfig, useChannelNumberColumnConfig, useChannelNameColumnConfig, useEPGColumnConfig } from "../../components/columns/columnConfigHooks";
import DataSelector from "../../components/dataSelector/DataSelector";
import { type ColumnMeta } from "../../components/dataSelector/DataSelectorTypes";
import VideoStreamRemoveFromStreamGroupDialog from "./VideoStreamRemoveFromStreamGroupDialog";
import { useQueryAdditionalFilters } from "../../app/slices/useQueryAdditionalFilters";

type StreamGroupSelectedVideoStreamDataSelectorProps = {

  id: string;
  onSelectionChange?: (value: VideoStreamDto | VideoStreamDto[]) => void;
  streamGroup: StreamGroupDto;
};

const StreamGroupSelectedVideoStreamDataSelector = ({ id, streamGroup }: StreamGroupSelectedVideoStreamDataSelectorProps) => {
  const dataKey = id + '-StreamGroupSelectedVideoStreamDataSelector';
  const enableEdit = false;

  const { columnConfig: m3uFileNameColumnConfig } = useM3UFileNameColumnConfig(enableEdit);
  const { columnConfig: channelNumberColumnConfig } = useChannelNumberColumnConfig(enableEdit);
  const { columnConfig: channelNameColumnConfig } = useChannelNameColumnConfig(enableEdit);
  const { columnConfig: epgColumnConfig } = useEPGColumnConfig(enableEdit);
  const { columnConfig: channelGroupConfig } = useChannelGroupColumnConfig(enableEdit);
  const { queryAdditionalFilter, setQueryAdditionalFilter } = useQueryAdditionalFilters(dataKey);

  useEffect(() => {
    if (queryAdditionalFilter === undefined && streamGroup !== undefined && streamGroup.id !== undefined && streamGroup.id > 0) {
      setQueryAdditionalFilter({ field: 'id', matchMode: 'equals', values: [streamGroup.id.toString()] });
    }


  }, [queryAdditionalFilter, streamGroup]);


  const [streamGroupsUpdateStreamGroupMutation] = useStreamGroupsUpdateStreamGroupMutation();

  const targetActionBodyTemplate = useCallback((data: VideoStreamDto) => {
    return (
      <div className='flex p-0 justify-content-end align-items-center'>
        <VideoStreamRemoveFromStreamGroupDialog id={id} streamGroupId={streamGroup.id} value={data} />
      </div>
    );
  }, [id, streamGroup.id]);

  const targetColumns = useMemo((): ColumnMeta[] => {

    return [
      channelNumberColumnConfig,
      channelNameColumnConfig,
      channelGroupConfig,
      // epgColumnConfig,
      m3uFileNameColumnConfig,
      {
        bodyTemplate: targetActionBodyTemplate, field: 'Remove', header: 'X', resizeable: false, sortable: false,
        style: {
          maxWidth: '7rem',
          width: '7rem',
        } as CSSProperties,
      }
    ]
  }, [channelNumberColumnConfig, channelNameColumnConfig, channelGroupConfig, m3uFileNameColumnConfig, targetActionBodyTemplate]);

  const rightHeaderTemplate = () => {
    return (
      <div className="flex justify-content-end align-items-center w-full gap-1" />
    );
  }

  const onRowReorder = async (changed: VideoStreamDto[]) => {

    const newData = changed.map((x: VideoStreamDto, index: number) => {
      return {
        ...x,
        rank: index,
      }
    }) as ChildVideoStreamDto[];


    var toSend = {} as UpdateStreamGroupRequest;


    toSend.streamGroupId = streamGroup.id;

    toSend.videoStreams = newData.map((stream) => {
      return { rank: stream.rank, videoStreamId: stream.id } as VideoStreamIsReadOnly;
    });

    await streamGroupsUpdateStreamGroupMutation(toSend)
      .then(() => {

      }).catch(() => {
        console.log('error');
      });

  }

  return (
    <DataSelector
      columns={targetColumns}
      defaultSortField="user_tvg_name"
      emptyMessage="No Streams"
      headerName={GetMessage('streams')}
      headerRightTemplate={rightHeaderTemplate()}
      id={dataKey}
      key='rank'
      onRowReorder={async (e) => await onRowReorder(e as VideoStreamDto[])}
      queryFilter={useStreamGroupVideoStreamsGetStreamGroupVideoStreamsQuery}
      reorderable
      selectionMode='single'
      style={{ height: 'calc(100vh - 40px)' }
      }
    />
  );
}

StreamGroupSelectedVideoStreamDataSelector.displayName = 'Stream Editor';


export default memo(StreamGroupSelectedVideoStreamDataSelector);
