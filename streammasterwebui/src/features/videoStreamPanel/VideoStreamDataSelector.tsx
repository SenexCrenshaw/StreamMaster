import { skipToken } from "@reduxjs/toolkit/dist/query";
import { memo, useEffect, useMemo, useState } from "react";
import { GetMessage } from "../../common/common";
import { useChannelNameColumnConfig, useChannelNumberColumnConfig } from "../../components/columns/columnConfigHooks";
import DataSelector from "../../components/dataSelector/DataSelector";
import { type ColumnMeta } from "../../components/dataSelector/DataSelectorTypes";
import { useVideoStreamLinksAddVideoStreamToVideoStreamMutation, useVideoStreamLinksGetVideoStreamVideoStreamIdsQuery, useVideoStreamsGetPagedVideoStreamsQuery, type VideoStreamDto, type VideoStreamLinksAddVideoStreamToVideoStreamApiArg } from "../../store/iptvApi";

type VideoStreamDataSelectorProps = {
  readonly id: string;
  readonly videoStreamId?: string;
};

const VideoStreamDataSelector = ({ id, videoStreamId }: VideoStreamDataSelectorProps) => {
  const dataKey = id + '-VideoStreamDataSelector';


  const [videoStreamIds, setVideoStreamIds] = useState<string[]>([] as string[]);

  const { columnConfig: channelNumberColumnConfig } = useChannelNumberColumnConfig({ enableEdit: false });
  const { columnConfig: channelNameColumnConfig } = useChannelNameColumnConfig({ enableEdit: false });

  const videoStreamLinksGetVideoStreamVideoStreamIdsQuery = useVideoStreamLinksGetVideoStreamVideoStreamIdsQuery(videoStreamId ?? skipToken);


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

  console.log("videostreamdataselector")

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

        let stream = {} as VideoStreamDto;

        if (Array.isArray(value)) {
          stream = value[0] as VideoStreamDto;
        } else {
          stream = value as VideoStreamDto;
        }

        const toSend = {} as VideoStreamLinksAddVideoStreamToVideoStreamApiArg;

        toSend.parentVideoStreamId = videoStreamId;
        toSend.childVideoStreamId = stream.id;

        await videoStreamLinksAddVideoStreamToVideoStreamMutation(toSend);
      }}
      queryFilter={useVideoStreamsGetPagedVideoStreamsQuery}
      selectedItemsKey='selectSelectedVideoStreamPanelVideoStreamDtoItems'
      selectionMode='single'
      style={{ height: 'calc(100vh - 480px)' }}
      videoStreamIdsIsReadOnly={(videoStreamIds || [])}
    />
  );
}

VideoStreamDataSelector.displayName = 'Stream Editor';

export default memo(VideoStreamDataSelector);
