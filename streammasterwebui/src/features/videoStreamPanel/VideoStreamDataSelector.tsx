import { useMemo, memo, useEffect, useState } from "react";
import { GetMessage } from "../../common/common";
import { type ChildVideoStreamDto } from "../../store/iptvApi";
import { useVideoStreamLinksAddVideoStreamToVideoStreamMutation, type VideoStreamLinksAddVideoStreamToVideoStreamApiArg } from "../../store/iptvApi";
import { useVideoStreamLinksGetVideoStreamVideoStreamIdsQuery, useVideoStreamsGetVideoStreamsQuery } from "../../store/iptvApi";
import { useChannelNumberColumnConfig, useChannelNameColumnConfig } from "../../components/columns/columnConfigHooks";
import DataSelector from "../../components/dataSelector/DataSelector";
import { type ColumnMeta } from "../../components/dataSelector/DataSelectorTypes";
import { useStreamToRemove } from "../../app/slices/useStreamToRemove";

type VideoStreamDataSelectorProps = {
  readonly id: string;
  readonly videoStreamId?: string;
};

const VideoStreamDataSelector = ({ id, videoStreamId }: VideoStreamDataSelectorProps) => {
  const dataKey = id + '-VideoStreamDataSelector';

  const { streamToRemove } = useStreamToRemove(id);


  const [videoStreamIds, setVideoStreamIds] = useState<string[]>([] as string[]);

  const { columnConfig: channelNumberColumnConfig } = useChannelNumberColumnConfig(false);
  const { columnConfig: channelNameColumnConfig } = useChannelNameColumnConfig(false);

  const videoStreamLinksGetVideoStreamVideoStreamIdsQuery = useVideoStreamLinksGetVideoStreamVideoStreamIdsQuery(videoStreamId ?? '');


  const [videoStreamLinksAddVideoStreamToVideoStreamMutation] = useVideoStreamLinksAddVideoStreamToVideoStreamMutation();

  useEffect(() => {
    if (videoStreamLinksGetVideoStreamVideoStreamIdsQuery.data !== undefined) {
      setVideoStreamIds(videoStreamLinksGetVideoStreamVideoStreamIdsQuery.data);
    }


  }, [videoStreamLinksGetVideoStreamVideoStreamIdsQuery.data]);

  const targetColumns = useMemo((): ColumnMeta[] => {
    let columnConfigs = [
      channelNumberColumnConfig,
      channelNameColumnConfig,
    ];

    return columnConfigs;

  }, [channelNameColumnConfig, channelNumberColumnConfig]);


  const rightHeaderTemplate = useMemo(() => {

    return (
      <div className="flex justify-content-end align-items-center w-full gap-1" />
    );
  }, []);



  return (
    <DataSelector
      columns={targetColumns}
      defaultSortField="user_tvg_name"
      emptyMessage="No Streams"
      headerName={GetMessage('streams')}
      headerRightTemplate={rightHeaderTemplate}
      id={dataKey}
      isLoading={videoStreamLinksGetVideoStreamVideoStreamIdsQuery.isLoading || videoStreamLinksGetVideoStreamVideoStreamIdsQuery.isFetching}
      onSelectionChange={async (value) => {
        if (value === undefined || videoStreamId === undefined) {
          return;
        }

        let stream = {} as ChildVideoStreamDto;

        if (Array.isArray(value)) {
          stream = value[0];
        } else {
          stream = value as ChildVideoStreamDto;
        }

        const toSend = {} as VideoStreamLinksAddVideoStreamToVideoStreamApiArg;

        toSend.parentVideoStreamId = videoStreamId;
        toSend.childVideoStreamId = stream.id;

        await videoStreamLinksAddVideoStreamToVideoStreamMutation(toSend);
      }}
      queryFilter={useVideoStreamsGetVideoStreamsQuery}
      selectionMode='single'
      streamToRemove={streamToRemove}
      style={{ height: 'calc(100vh - 134px)' }}
      videoStreamIdsIsReadOnly={(videoStreamIds || [])}
    />
  );
}

VideoStreamDataSelector.displayName = 'Stream Editor';

export default memo(VideoStreamDataSelector);
