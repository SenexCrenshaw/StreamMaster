/* eslint-disable react/no-unused-prop-types */
import { useMemo, memo, useEffect, useState } from "react";
import { GetMessage } from "../../common/common";
import { type UpdateStreamGroupRequest, type VideoStreamIsReadOnly } from "../../store/iptvApi";
import { type ChildVideoStreamDto, useStreamGroupsUpdateStreamGroupMutation } from "../../store/iptvApi";
import { useStreamGroupsRemoveVideoStreamToStreamGroupMutation, type StreamGroupsRemoveVideoStreamToStreamGroupApiArg } from "../../store/iptvApi";
import { type StreamGroupsGetStreamGroupVideoStreamsApiArg } from "../../store/iptvApi";
import { useStreamGroupsGetStreamGroupVideoStreamsQuery, type StreamGroupDto } from "../../store/iptvApi";
import { type VideoStreamDto } from "../../store/iptvApi";
import { useChannelGroupColumnConfig, useM3UFileNameColumnConfig, useChannelNumberColumnConfig, useChannelNameColumnConfig } from "../columns/columnConfigHooks";
import DataSelector from "../dataSelector/DataSelector";
import { type ColumnMeta } from "../dataSelector/DataSelectorTypes";

type StreamGroupSelectedVideoStreamDataSelectorProps = {

  id: string;
  onSelectionChange?: (value: VideoStreamDto | VideoStreamDto[]) => void;
  streamGroup: StreamGroupDto;
};

const StreamGroupSelectedVideoStreamDataSelector = ({ id, streamGroup }: StreamGroupSelectedVideoStreamDataSelectorProps) => {
  const dataKey = id + '-StreamGroupSelectedVideoStreamDataSelector';

  const [videoStreams, setVideoStreams] = useState<VideoStreamDto[]>([] as VideoStreamDto[]);

  const { columnConfig: m3uFileNameColumnConfig } = useM3UFileNameColumnConfig(false);
  const { columnConfig: channelNumberColumnConfig } = useChannelNumberColumnConfig(false);
  const { columnConfig: channelNameColumnConfig } = useChannelNameColumnConfig(false);
  const { columnConfig: channelGroupConfig } = useChannelGroupColumnConfig(false);

  const streamGroupsGetStreamGroupVideoStreamsQuery = useStreamGroupsGetStreamGroupVideoStreamsQuery(streamGroup.id as StreamGroupsGetStreamGroupVideoStreamsApiArg);

  const [streamGroupsRemoveVideoStreamToStreamGroupMutation] = useStreamGroupsRemoveVideoStreamToStreamGroupMutation();
  const [streamGroupsUpdateStreamGroupMutation] = useStreamGroupsUpdateStreamGroupMutation();


  useEffect(() => {
    if (streamGroupsGetStreamGroupVideoStreamsQuery.data !== undefined) {
      setVideoStreams(streamGroupsGetStreamGroupVideoStreamsQuery.data);
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [streamGroupsGetStreamGroupVideoStreamsQuery.data]);

  const targetColumns = useMemo((): ColumnMeta[] => {

    return [
      channelNumberColumnConfig,
      channelNameColumnConfig,
      channelGroupConfig,
      m3uFileNameColumnConfig,
    ]
  }, [channelNumberColumnConfig, channelNameColumnConfig, channelGroupConfig, m3uFileNameColumnConfig]);

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

    console.group('onRowReorder');
    newData.forEach((x) => {
      console.log(x.id, x.rank, x.user_Tvg_name);
    });
    console.groupEnd();

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
      dataSource={videoStreams}
      defaultSortField="user_tvg_name"
      emptyMessage="No Streams"
      headerName={GetMessage('streams')}
      headerRightTemplate={rightHeaderTemplate()}
      id={dataKey}
      key='rank'
      onRowReorder={async (e) => await onRowReorder(e as VideoStreamDto[])}
      // eslint-disable-next-line @typescript-eslint/no-unused-vars
      onSelectionChange={async (value, selectAllReturn, retTotalRecords) => {
        if (value === undefined) {
          return;
        }

        let stream = {} as VideoStreamDto;

        if (Array.isArray(value)) {
          stream = value[0];
        } else {
          stream = value as VideoStreamDto;
        }

        console.log(value);

        const toSend = {} as StreamGroupsRemoveVideoStreamToStreamGroupApiArg;

        toSend.streamGroupId = streamGroup.id;
        toSend.videoStreamId = stream.id;

        await streamGroupsRemoveVideoStreamToStreamGroupMutation(toSend);
      }}
      reorderable
      selectionMode='single'
      style={{ height: 'calc(100vh - 40px)' }
      }
    />
  );
}

StreamGroupSelectedVideoStreamDataSelector.displayName = 'Stream Editor';


export default memo(StreamGroupSelectedVideoStreamDataSelector);
