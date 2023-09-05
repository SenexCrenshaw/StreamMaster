import { useCallback, type CSSProperties, useEffect } from "react";
import { useMemo, memo } from "react";
import { GetMessage } from "../../common/common";
import { type UpdateStreamGroupRequest, type VideoStreamIsReadOnly } from "../../store/iptvApi";
import { type ChildVideoStreamDto, useStreamGroupsUpdateStreamGroupMutation } from "../../store/iptvApi";
import { useStreamGroupVideoStreamsGetStreamGroupVideoStreamsQuery } from "../../store/iptvApi";
import { type VideoStreamDto } from "../../store/iptvApi";
import { useChannelNumberColumnConfig, useChannelNameColumnConfig, useEPGColumnConfig } from "../../components/columns/columnConfigHooks";
import DataSelector from "../../components/dataSelector/DataSelector";
import { type ColumnMeta } from "../../components/dataSelector/DataSelectorTypes";
import VideoStreamRemoveFromStreamGroupDialog from "./VideoStreamRemoveFromStreamGroupDialog";
import { useQueryAdditionalFilters } from "../../app/slices/useQueryAdditionalFilters";
import { useSelectedStreamGroup } from "../../app/slices/useSelectedStreamGroup";

type StreamGroupSelectedVideoStreamDataSelectorProps = {
  readonly id: string;
};

const StreamGroupSelectedVideoStreamDataSelector = ({ id }: StreamGroupSelectedVideoStreamDataSelectorProps) => {
  const dataKey = id + '-StreamGroupSelectedVideoStreamDataSelector';
  const { selectedStreamGroup } = useSelectedStreamGroup(id);
  const enableEdit = false;

  // const { columnConfig: m3uFileNameColumnConfig } = useM3UFileNameColumnConfig(enableEdit);
  const { columnConfig: channelNumberColumnConfig } = useChannelNumberColumnConfig(true);
  const { columnConfig: channelNameColumnConfig } = useChannelNameColumnConfig(enableEdit);
  // const { columnConfig: channelGroupConfig } = useChannelGroupColumnConfig(enableEdit);
  const { columnConfig: epgColumnConfig } = useEPGColumnConfig(true);
  const { setQueryAdditionalFilter } = useQueryAdditionalFilters(dataKey);

  useEffect(() => {

    if (selectedStreamGroup !== undefined && selectedStreamGroup !== undefined && selectedStreamGroup.id > 0) {
      console.log('setting queryAdditionalFilter', { field: 'id', matchMode: 'equals', values: [selectedStreamGroup.id.toString()] });
      setQueryAdditionalFilter({ field: 'id', matchMode: 'equals', values: [selectedStreamGroup.id.toString()] });
    }


    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [selectedStreamGroup]);


  const [streamGroupsUpdateStreamGroupMutation] = useStreamGroupsUpdateStreamGroupMutation();

  const targetActionBodyTemplate = useCallback((data: VideoStreamDto) => {
    return (
      <div className='flex p-0 justify-content-end align-items-center'>
        <VideoStreamRemoveFromStreamGroupDialog id={id} value={data} />
      </div>
    );
  }, [id]);

  const targetColumns = useMemo((): ColumnMeta[] => {

    return [
      channelNumberColumnConfig,
      channelNameColumnConfig,
      // channelGroupConfig,
      epgColumnConfig,
      // m3uFileNameColumnConfig,
      {
        bodyTemplate: targetActionBodyTemplate, field: 'Remove', header: '', resizeable: false, sortable: false,
        style: {
          maxWidth: '4rem',
          width: '4rem',
        } as CSSProperties,
      }
    ]
  }, [channelNumberColumnConfig, channelNameColumnConfig, epgColumnConfig, targetActionBodyTemplate]);

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


    toSend.streamGroupId = selectedStreamGroup.id;

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
