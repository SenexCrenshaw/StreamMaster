import { ColumnMeta } from '@components/dataSelector/DataSelectorTypes';
import { GetMessage } from '@lib/common/common';
import { VideoStreamDto, VideoStreamsUpdateVideoStreamApiArg, useVideoStreamLinksGetPagedVideoStreamVideoStreamsQuery } from '@lib/iptvApi';
import { useQueryAdditionalFilters } from '@lib/redux/slices/useQueryAdditionalFilters';
import { UpdateVideoStream } from '@lib/smAPI/VideoStreams/VideoStreamsMutateAPI';
import { memo, useCallback, useEffect, useMemo } from 'react';
import XButton from '../buttons/XButton';
import { useChannelNameColumnConfig } from '../columns/useChannelNameColumnConfig';
import { useChannelNumberColumnConfig } from '../columns/useChannelNumberColumnConfig';
import DataSelector from '../dataSelector/DataSelector';
import VideoStreamRemoveFromVideoStreamDialog from './VideoStreamRemoveFromVideoStreamDialog';

interface VideoStreamSelectedVideoStreamDataSelectorProperties {
  readonly id: string;
  readonly videoStreamId?: string;
  readonly dataSource?: VideoStreamDto[];
  onRemove?: (e: VideoStreamDto) => void;
  OnRowReorder?: (e: VideoStreamDto[]) => void;
}

const VideoStreamSelectedVideoStreamDataSelector = ({
  id,
  dataSource,
  onRemove,
  OnRowReorder,
  videoStreamId
}: VideoStreamSelectedVideoStreamDataSelectorProperties) => {
  const dataKey = `${id}-VideoStreamSelectedVideoStreamDataSelector`;

  const { columnConfig: channelNumberColumnConfig } = useChannelNumberColumnConfig({ enableEdit: false });
  const { columnConfig: channelNameColumnConfig } = useChannelNameColumnConfig({
    enableEdit: false
  });

  const { setQueryAdditionalFilter } = useQueryAdditionalFilters(dataKey);

  useEffect(() => {
    if (videoStreamId) {
      setQueryAdditionalFilter({
        field: 'parentVideoStreamId',
        matchMode: 'equals',
        values: [videoStreamId]
      });
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [videoStreamId]);

  const targetActionBodyTemplate = useCallback(
    (data: VideoStreamDto) => {
      if (videoStreamId === undefined) {
        return (
          <div className="flex p-0 justify-content-end align-items-center">
            <XButton
              onClick={() => {
                onRemove && onRemove(data as VideoStreamDto);
              }}
            />
          </div>
        );
      }

      return (
        <div className="flex p-0 justify-content-end align-items-center">
          <VideoStreamRemoveFromVideoStreamDialog value={data} videoStreamId={videoStreamId ?? 'ERROR'} />
        </div>
      );
    },
    [onRemove, videoStreamId]
  );

  const targetColumns = useMemo(
    (): ColumnMeta[] => [
      channelNumberColumnConfig,
      channelNameColumnConfig,
      {
        bodyTemplate: targetActionBodyTemplate,
        field: 'Remove',
        header: 'X',
        resizeable: false,
        sortable: false,
        width: '2rem'
      }
    ],
    [channelNumberColumnConfig, channelNameColumnConfig, targetActionBodyTemplate]
  );

  const rightHeaderTemplate = () => <div className="flex justify-content-end align-items-center w-full gap-1" />;

  const intOnRowReorder = async (changed: VideoStreamDto[]) => {
    const newData = changed.map((x: VideoStreamDto, index: number) => ({
      ...x,
      rank: index
    })) as VideoStreamDto[];

    if (OnRowReorder) {
      OnRowReorder(newData);
    }

    const toSend = {} as VideoStreamsUpdateVideoStreamApiArg;

    toSend.id = videoStreamId;
    toSend.childVideoStreams = newData;

    await UpdateVideoStream(toSend)
      .then(() => {})
      .catch(() => {
        console.log('error');
      });
  };

  return (
    <DataSelector
      columns={targetColumns}
      dataSource={dataSource}
      defaultSortField="user_Tvg_name"
      emptyMessage="No Streams"
      headerName={GetMessage('streams')}
      headerRightTemplate={rightHeaderTemplate()}
      id={dataKey}
      dataKey="rank"
      onRowReorder={async (e) => await intOnRowReorder(e as VideoStreamDto[])}
      queryFilter={dataSource === undefined ? useVideoStreamLinksGetPagedVideoStreamVideoStreamsQuery : undefined}
      reorderable
      selectedItemsKey={`selectSelected${videoStreamId}`}
      selectionMode="single"
      style={{ height: 'calc(100vh - 480px)' }}
    />
  );
};

VideoStreamSelectedVideoStreamDataSelector.displayName = 'Stream Editor';

export default memo(VideoStreamSelectedVideoStreamDataSelector);
