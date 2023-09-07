import { memo, useCallback, useEffect, useMemo, type CSSProperties } from "react";
import { useQueryAdditionalFilters } from "../../app/slices/useQueryAdditionalFilters";
import { useSelectedStreamGroup } from "../../app/slices/useSelectedStreamGroup";
import { GetMessage } from "../../common/common";
import { useChannelNameColumnConfig, useChannelNumberColumnConfig, useEPGColumnConfig } from "../../components/columns/columnConfigHooks";
import DataSelector from "../../components/dataSelector/DataSelector";
import { type ColumnMeta } from "../../components/dataSelector/DataSelectorTypes";
import StreamGroupChannelGroupsSelector from "../../components/selectors/StreamGroupChannelGroupsSelector";
import { useStreamGroupVideoStreamsGetStreamGroupVideoStreamsQuery, type VideoStreamDto } from "../../store/iptvApi";
import VideoStreamRemoveFromStreamGroupDialog from "./VideoStreamRemoveFromStreamGroupDialog";

type StreamGroupSelectedVideoStreamDataSelectorProps = {
  readonly id: string;
};

const StreamGroupSelectedVideoStreamDataSelector = ({ id }: StreamGroupSelectedVideoStreamDataSelectorProps) => {
  const dataKey = id + '-StreamGroupSelectedVideoStreamDataSelector';
  const { selectedStreamGroup } = useSelectedStreamGroup(id);
  const enableEdit = false;

  const { columnConfig: channelNumberColumnConfig } = useChannelNumberColumnConfig(true);
  const { columnConfig: channelNameColumnConfig } = useChannelNameColumnConfig(enableEdit);
  const { columnConfig: epgColumnConfig } = useEPGColumnConfig(true);
  const { setQueryAdditionalFilter } = useQueryAdditionalFilters(dataKey);

  useEffect(() => {

    if (selectedStreamGroup !== undefined && selectedStreamGroup !== undefined && selectedStreamGroup.id > 0) {
      setQueryAdditionalFilter({ field: 'streamGroupId', matchMode: 'equals', values: [selectedStreamGroup.id.toString()] });
    }


    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [selectedStreamGroup]);

  // const sourceaddtionalHeaderTemplate = () => {
  //   return (
  //     <div className="streamGroupEditor grid w-full flex flex-nowrap justify-content-end align-items-center p-0">
  //       <div className="flex w-full w-full p-0 align-items-center justify-content-end">
  //         <div className="flex justify-content-end gap-2 align-items-center mr-2">

  //           <StreamGroupChannelGroupsSelector streamGroupId={2} />

  //         </div >
  //       </div>
  //     </div >
  //   );
  // };

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
          maxWidth: '2rem',
        } as CSSProperties,
      }
    ]
  }, [channelNumberColumnConfig, channelNameColumnConfig, epgColumnConfig, targetActionBodyTemplate]);

  const rightHeaderTemplate = () => {
    return (
      <div className="flex justify-content-end align-items-center w-full gap-1" >
        <StreamGroupChannelGroupsSelector streamGroupId={selectedStreamGroup?.id} />
      </div>
    );
  }

  // const onRowReorder = async (changed: VideoStreamDto[]) => {

  //   const newData = changed.map((x: VideoStreamDto, index: number) => {
  //     return {
  //       rank: index,
  //       videoStreamId: x.id,
  //     }
  //   }) as VideoStreamIsReadOnly[];


  //   var toSend = {} as SetVideoStreamRanksRequest;

  //   toSend.streamGroupId = selectedStreamGroup.id;
  //   toSend.videoStreamIDRanks = newData;

  //   await streamGroupVideoStreamsSetVideoStreamRanksMutation(toSend)
  //     .then(() => {

  //     }).catch(() => {
  //       console.log('error');
  //     });

  // }

  return (
    <DataSelector
      columns={targetColumns}
      defaultSortField="user_tvg_name"
      emptyMessage="No Streams"
      headerName={GetMessage('streams')}
      headerRightTemplate={rightHeaderTemplate()}
      id={dataKey}
      key='rank'
      queryFilter={useStreamGroupVideoStreamsGetStreamGroupVideoStreamsQuery}
      selectionMode='single'
      style={{ height: 'calc(100vh - 40px)' }
      }
    />
  );
}

StreamGroupSelectedVideoStreamDataSelector.displayName = 'Stream Editor';


export default memo(StreamGroupSelectedVideoStreamDataSelector);
