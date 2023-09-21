import { memo, useCallback, useEffect, useMemo, type CSSProperties } from "react";
import { useQueryAdditionalFilters } from "../../app/slices/useQueryAdditionalFilters";
import { GetMessage } from "../../common/common";
import { useChannelNameColumnConfig, useChannelNumberColumnConfig } from "../../components/columns/columnConfigHooks";
import DataSelector from "../../components/dataSelector/DataSelector";
import { type ColumnMeta } from "../../components/dataSelector/DataSelectorTypes";
import { UpdateVideoStream } from "../../smAPI/VideoStreams/VideoStreamsMutateAPI";
import { useVideoStreamLinksGetPagedVideoStreamVideoStreamsQuery, type ChildVideoStreamDto, type VideoStreamsUpdateVideoStreamApiArg } from "../../store/iptvApi";
import VideoStreamRemoveFromVideoStreamDialog from "./VideoStreamRemoveFromVideoStreamDialog";

type VideoStreamSelectedVideoStreamDataSelectorProps = {
  readonly id: string;
  readonly videoStreamId?: string;
};

const VideoStreamSelectedVideoStreamDataSelector = ({ id, videoStreamId }: VideoStreamSelectedVideoStreamDataSelectorProps) => {
  const dataKey = id + '-VideoStreamSelectedVideoStreamDataSelector';

  const { columnConfig: channelNumberColumnConfig } = useChannelNumberColumnConfig({ enableEdit: false });
  const { columnConfig: channelNameColumnConfig } = useChannelNameColumnConfig({ enableEdit: false });

  const { setQueryAdditionalFilter } = useQueryAdditionalFilters(dataKey);

  useEffect(() => {
    if (videoStreamId) {
      setQueryAdditionalFilter({ field: 'parentVideoStreamId', matchMode: 'equals', values: [videoStreamId] });
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [videoStreamId]);

  const targetActionBodyTemplate = useCallback((data: ChildVideoStreamDto) => {
    return (
      <div className='flex p-0 justify-content-end align-items-center'>
        <VideoStreamRemoveFromVideoStreamDialog value={data} videoStreamId={videoStreamId ?? 'ERROR'} />
      </div>
    );
  }, [videoStreamId]);


  const targetColumns = useMemo((): ColumnMeta[] => {

    return [
      channelNumberColumnConfig,
      channelNameColumnConfig,
      {
        bodyTemplate: targetActionBodyTemplate, field: 'Remove', header: 'X', resizeable: false, sortable: false,
        style: {
          maxWidth: '2rem',
          width: '2rem',
        } as CSSProperties,
      }
    ]
  }, [channelNumberColumnConfig, channelNameColumnConfig, targetActionBodyTemplate]);


  const rightHeaderTemplate = () => {
    return (
      <div className="flex justify-content-end align-items-center w-full gap-1" />
    );
  }

  const onRowReorder = async (changed: ChildVideoStreamDto[]) => {

    const newData = changed.map((x: ChildVideoStreamDto, index: number) => {
      return {
        ...x,
        rank: index,
      }
    }) as ChildVideoStreamDto[];

    var toSend = {} as VideoStreamsUpdateVideoStreamApiArg;


    toSend.id = videoStreamId;
    toSend.childVideoStreams = newData;

    await UpdateVideoStream(toSend)
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
      onRowReorder={async (e) => await onRowReorder(e as ChildVideoStreamDto[])}
      queryFilter={useVideoStreamLinksGetPagedVideoStreamVideoStreamsQuery}
      reorderable
      selectedItemsKey={`selectSelected` + videoStreamId}
      selectionMode='single'
      style={{ height: 'calc(100vh - 480px)' }
      }

    />
  );
}

VideoStreamSelectedVideoStreamDataSelector.displayName = 'Stream Editor';


export default memo(VideoStreamSelectedVideoStreamDataSelector);
