/* eslint-disable react/no-unused-prop-types */
import { useCallback, type CSSProperties, useEffect } from "react";
import { useMemo, memo } from "react";
import { GetMessage } from "../../common/common";
import { type UpdateStreamGroupRequest, type VideoStreamIsReadOnly } from "../../store/iptvApi";
import { type ChildVideoStreamDto, useStreamGroupsUpdateStreamGroupMutation } from "../../store/iptvApi";

import { useStreamGroupVideoStreamsGetStreamGroupVideoStreamsQuery, type StreamGroupDto } from "../../store/iptvApi";
import { type VideoStreamDto } from "../../store/iptvApi";
import { useChannelGroupColumnConfig, useM3UFileNameColumnConfig, useChannelNumberColumnConfig, useChannelNameColumnConfig } from "../../components/columns/columnConfigHooks";
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

  const { columnConfig: m3uFileNameColumnConfig } = useM3UFileNameColumnConfig(false);
  const { columnConfig: channelNumberColumnConfig } = useChannelNumberColumnConfig(false);
  const { columnConfig: channelNameColumnConfig } = useChannelNameColumnConfig(false);
  const { columnConfig: channelGroupConfig } = useChannelGroupColumnConfig(false);
  const { queryAdditionalFilter, setQueryAdditionalFilter } = useQueryAdditionalFilters(dataKey);

  useEffect(() => {
    if (queryAdditionalFilter === undefined && streamGroup !== undefined && streamGroup.id !== undefined && streamGroup.id > 0) {
      setQueryAdditionalFilter({ field: 'id', matchMode: 'equals', values: [streamGroup.id.toString()] });
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [queryAdditionalFilter, streamGroup]);

  // const streamGroupsGetStreamGroupVideoStreamsQuery = useStreamGroupVideoStreamsGetStreamGroupVideoStreamsQuery(streamGroup.id as StreamGroupVideoStreamsGetStreamGroupVideoStreamsApiArg);

  const [streamGroupsUpdateStreamGroupMutation] = useStreamGroupsUpdateStreamGroupMutation();


  // useEffect(() => {
  //   if (streamGroupsGetStreamGroupVideoStreamsQuery.data?.data !== undefined) {
  //     setVideoStreams(streamGroupsGetStreamGroupVideoStreamsQuery.data.data);
  //   }

  //   // eslint-disable-next-line react-hooks/exhaustive-deps
  // }, [streamGroupsGetStreamGroupVideoStreamsQuery.data]);

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
      // dataSource={videoStreams}
      defaultSortField="user_tvg_name"
      emptyMessage="No Streams"
      headerName={GetMessage('streams')}
      headerRightTemplate={rightHeaderTemplate()}
      id={dataKey}
      // isLoading={streamGroupsGetStreamGroupVideoStreamsQuery.isLoading || streamGroupsGetStreamGroupVideoStreamsQuery.isFetching}
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
