import { useEffect, type CSSProperties } from "react";
import { useMemo, memo, useCallback } from "react";
import { GetMessage } from "../../common/common";
import { useVideoStreamLinksGetVideoStreamVideoStreamsQuery, useVideoStreamsUpdateVideoStreamMutation, type ChildVideoStreamDto, type VideoStreamsUpdateVideoStreamApiArg } from "../../store/iptvApi";
import { useChannelNumberColumnConfig, useChannelNameColumnConfig } from "../../components/columns/columnConfigHooks";
import DataSelector from "../../components/dataSelector/DataSelector";
import { type ColumnMeta } from "../../components/dataSelector/DataSelectorTypes";
import VideoStreamRemoveFromVideoStreamDialog from "./VideoStreamRemoveFromVideoStreamDialog";
import { useQueryAdditionalFilters } from "../../app/slices/useQueryAdditionalFilters";

type VideoStreamSelectedVideoStreamDataSelectorProps = {
  id: string;
  videoStreamId: string;
};

const VideoStreamSelectedVideoStreamDataSelector = ({ id, videoStreamId }: VideoStreamSelectedVideoStreamDataSelectorProps) => {
  const dataKey = id + '-VideoStreamSelectedVideoStreamDataSelector';

  const { columnConfig: channelNumberColumnConfig } = useChannelNumberColumnConfig(false);
  const { columnConfig: channelNameColumnConfig } = useChannelNameColumnConfig(false);

  const { queryAdditionalFilter, setQueryAdditionalFilter } = useQueryAdditionalFilters(dataKey);

  const [videoStreamsUpdateVideoStreamMutation] = useVideoStreamsUpdateVideoStreamMutation();

  useEffect(() => {
    if (queryAdditionalFilter === undefined) {
      setQueryAdditionalFilter({ field: 'parentVideoStreamId', matchMode: 'equals', values: [videoStreamId] });
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [queryAdditionalFilter]);

  const targetActionBodyTemplate = useCallback((data: ChildVideoStreamDto) => {
    return (
      <div className='flex p-0 justify-content-end align-items-center'>
        <VideoStreamRemoveFromVideoStreamDialog id={id} value={data} videoStreamId={videoStreamId} />
      </div>
    );
  }, [id, videoStreamId]);


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

    await videoStreamsUpdateVideoStreamMutation(toSend)
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
      queryFilter={useVideoStreamLinksGetVideoStreamVideoStreamsQuery}
      reorderable
      selectionMode='single'
      style={{ height: 'calc(100vh - 134px)' }
      }

    />
  );
}

VideoStreamSelectedVideoStreamDataSelector.displayName = 'Stream Editor';


export default memo(VideoStreamSelectedVideoStreamDataSelector);
