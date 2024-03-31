import { ColumnMeta } from '@components/dataSelector/DataSelectorTypes';
import { GetMessage } from '@lib/common/common';

import VideoStreamAddDialog from '@components/videoStream/VideoStreamAddDialog';
import { skipToken } from '@reduxjs/toolkit/dist/query';
import { memo, useEffect, useMemo, useState } from 'react';
import { useChannelNameColumnConfig } from '../columns/useChannelNameColumnConfig';
import { useChannelNumberColumnConfig } from '../columns/useChannelNumberColumnConfig';
import DataSelector from '../dataSelector/DataSelector';

interface VideoStreamDataSelectorProperties {
  readonly id: string;
  readonly videoStreamId?: string;
  onRowClick?: (e: VideoStreamDto) => void;
}

const VideoStreamDataSelector = ({ id, onRowClick, videoStreamId }: VideoStreamDataSelectorProperties) => {
  const dataKey = `${id}-VideoStreamDataSelector`;

  const [videoStreamIds, setVideoStreamIds] = useState<string[]>([] as string[]);

  const { columnConfig: channelNumberColumnConfig } = useChannelNumberColumnConfig({ enableEdit: false });
  const { columnConfig: channelNameColumnConfig } = useChannelNameColumnConfig({ enableEdit: false });
  const videoStreamLinksGetVideoStreamVideoStreamIdsQuery = useVideoStreamLinksGetVideoStreamVideoStreamIdsQuery(videoStreamId ?? skipToken);

  useEffect(() => {
    if (videoStreamLinksGetVideoStreamVideoStreamIdsQuery.data !== undefined) {
      setVideoStreamIds(videoStreamLinksGetVideoStreamVideoStreamIdsQuery.data);
    }
  }, [videoStreamLinksGetVideoStreamVideoStreamIdsQuery.data]);

  const targetColumns = useMemo((): ColumnMeta[] => {
    const columnConfigs = [channelNumberColumnConfig, channelNameColumnConfig];

    return columnConfigs;
  }, [channelNameColumnConfig, channelNumberColumnConfig]);

  const rightHeaderTemplate = useMemo(() => {
    return (
      <div className="flex justify-content-end align-items-center w-full gap-1">
        <VideoStreamAddDialog group="" />
      </div>
    );
  }, []);

  return (
    <DataSelector
      columns={targetColumns}
      defaultSortField="user_Tvg_name"
      emptyMessage="No Streams"
      headerName={GetMessage('streams')}
      headerRightTemplate={rightHeaderTemplate}
      id={dataKey}
      isLoading={videoStreamLinksGetVideoStreamVideoStreamIdsQuery.isLoading || videoStreamLinksGetVideoStreamVideoStreamIdsQuery.isFetching}
      onRowClick={async (e) => {
        if (e.data === undefined) {
          return;
        }

        if (videoStreamId === undefined) {
          onRowClick && onRowClick(e.data as VideoStreamDto);
          return;
        }

        let stream = {} as VideoStreamDto;

        stream = Array.isArray(e.data) ? (e.data[0] as VideoStreamDto) : (e.data as VideoStreamDto);

        const toSend = {} as VideoStreamLinksAddVideoStreamToVideoStreamApiArg;

        toSend.parentVideoStreamId = videoStreamId;
        toSend.childVideoStreamId = stream.id;

        await AddVideoStreamToVideoStream(toSend)
          .then()
          .catch((error) => {
            console.error(`Add Stream Error: ${error.message}`);
          });
      }}
      queryFilter={useVideoStreamsGetPagedVideoStreamsQuery}
      selectedItemsKey={`selectSelected${videoStreamId}`}
      selectionMode="single"
      style={{ height: 'calc(100vh - 480px)' }}
      videoStreamIdsIsReadOnly={videoStreamIds || []}
    />
  );
};

VideoStreamDataSelector.displayName = 'Stream Editor';

export default memo(VideoStreamDataSelector);
