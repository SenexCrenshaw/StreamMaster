import { type DataTableRowClickEvent } from "primereact/datatable";
import { TriStateCheckbox, type TriStateCheckboxChangeEvent } from "primereact/tristatecheckbox";
import { memo, useEffect, useMemo, useState } from "react";
import { useSelectedStreamGroup } from "../../app/slices/useSelectedStreamGroup";
import { useShowHidden } from "../../app/slices/useShowHidden";
import { GetMessage, getTopToolOptions } from "../../common/common";
import { useChannelGroupColumnConfig, useChannelNameColumnConfig, useChannelNumberColumnConfig, useM3UFileNameColumnConfig } from "../../components/columns/columnConfigHooks";
import DataSelector from "../../components/dataSelector/DataSelector";
import { type ColumnMeta } from "../../components/dataSelector/DataSelectorTypes";
import { useStreamGroupVideoStreamsAddVideoStreamToStreamGroupMutation, useStreamGroupVideoStreamsGetStreamGroupVideoStreamIdsQuery, useVideoStreamsGetVideoStreamsQuery, type StreamGroupVideoStreamsAddVideoStreamToStreamGroupApiArg, type VideoStreamIsReadOnly } from "../../store/iptvApi";

type StreamGroupVideoStreamDataSelectorProps = {
  readonly id: string;
};

const StreamGroupVideoStreamDataSelector = ({ id }: StreamGroupVideoStreamDataSelectorProps) => {
  const dataKey = id + '-StreamGroupVideoStreamDataSelector';
  const [videoStreams, setVideoStreams] = useState<VideoStreamIsReadOnly[]>([] as VideoStreamIsReadOnly[]);
  const { columnConfig: m3uFileNameColumnConfig } = useM3UFileNameColumnConfig(false);
  const { columnConfig: channelNumberColumnConfig } = useChannelNumberColumnConfig(false);
  const { columnConfig: channelNameColumnConfig } = useChannelNameColumnConfig(false);
  const { columnConfig: channelGroupConfig } = useChannelGroupColumnConfig(false);
  const { showHidden, setShowHidden } = useShowHidden(dataKey);
  const { selectedStreamGroup } = useSelectedStreamGroup(id);
  const streamGroupsGetStreamGroupVideoStreamIdsQuery = useStreamGroupVideoStreamsGetStreamGroupVideoStreamIdsQuery(selectedStreamGroup?.id ?? 1);
  const [streamGroupVideoStreamsAddVideoStreamToStreamGroupMutation] = useStreamGroupVideoStreamsAddVideoStreamToStreamGroupMutation();

  useEffect(() => {
    if (streamGroupsGetStreamGroupVideoStreamIdsQuery.data !== undefined) {
      setVideoStreams(streamGroupsGetStreamGroupVideoStreamIdsQuery.data);
    }


  }, [streamGroupsGetStreamGroupVideoStreamIdsQuery.data]);

  const addVideoStream = async (videoId: string) => {
    if (!videoId || !selectedStreamGroup) {
      return;
    }

    const toSend = {} as StreamGroupVideoStreamsAddVideoStreamToStreamGroupApiArg;

    toSend.streamGroupId = selectedStreamGroup.id;
    toSend.videoStreamId = videoId;

    await streamGroupVideoStreamsAddVideoStreamToStreamGroupMutation(toSend).then(() => {
    }).catch((error) => {
      console.error('Add Stream Error: ' + error.message);
    });

  }


  const targetColumns = useMemo((): ColumnMeta[] => {

    return [
      channelNumberColumnConfig,
      channelNameColumnConfig,
      channelGroupConfig,
      m3uFileNameColumnConfig
    ]
  }, [channelNumberColumnConfig, channelNameColumnConfig, channelGroupConfig, m3uFileNameColumnConfig]);

  const getToolTip = (value: boolean | null | undefined) => {
    if (value === null) {
      return 'Show All';
    }

    if (value === true) {
      return 'Show Visible';
    }

    return 'Show Hidden';
  };

  const rightHeaderTemplate = useMemo(() => {

    return (
      <div className="flex justify-content-end align-items-center w-full gap-1" >

        <TriStateCheckbox
          className='sm-tristatecheckbox'
          onChange={(e: TriStateCheckboxChangeEvent) => { setShowHidden(e.value); }}
          tooltip={getToolTip(showHidden)}
          tooltipOptions={getTopToolOptions}
          value={showHidden} />

      </div>
    );


    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [showHidden]);

  const onRowClick = async (event: DataTableRowClickEvent) => {
    console.log(event);
    await addVideoStream(event.data.id);
  };

  return (
    <DataSelector
      columns={targetColumns}
      defaultSortField="user_tvg_name"
      defaultSortOrder={1}
      emptyMessage="No Streams"
      headerName={GetMessage('streams')}
      headerRightTemplate={rightHeaderTemplate}
      id={dataKey}
      isLoading={streamGroupsGetStreamGroupVideoStreamIdsQuery.isLoading || streamGroupsGetStreamGroupVideoStreamIdsQuery.isFetching}
      onRowClick={async (e) => await onRowClick(e)}
      queryFilter={useVideoStreamsGetVideoStreamsQuery}
      selectionMode='single'
      style={{ height: 'calc(100vh - 40px)' }
      }

      videoStreamIdsIsReadOnly={(videoStreams || []).map((x) => x.videoStreamId ?? '')}
    />
  );
}

StreamGroupVideoStreamDataSelector.displayName = 'Stream Editor';


export default memo(StreamGroupVideoStreamDataSelector);
