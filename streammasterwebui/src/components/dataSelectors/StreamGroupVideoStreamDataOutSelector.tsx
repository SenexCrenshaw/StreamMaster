/* eslint-disable react/no-unused-prop-types */
import { useLocalStorage } from "primereact/hooks";
import { type TriStateCheckboxChangeEvent } from "primereact/tristatecheckbox";
import { TriStateCheckbox } from "primereact/tristatecheckbox";

import { useMemo, memo, useEffect, useState } from "react";
import { getTopToolOptions, GetMessage } from "../../common/common";
import { useStreamGroupsRemoveVideoStreamToStreamGroupMutation, type StreamGroupsRemoveVideoStreamToStreamGroupApiArg } from "../../store/iptvApi";
import { type StreamGroupsGetStreamGroupVideoStreamsApiArg } from "../../store/iptvApi";
import { useStreamGroupsGetStreamGroupVideoStreamsQuery, type StreamGroupDto } from "../../store/iptvApi";
import { type VideoStreamDto } from "../../store/iptvApi";
import { useChannelGroupColumnConfig, useM3UFileNameColumnConfig, useChannelNumberColumnConfig, useChannelNameColumnConfig } from "../columns/columnConfigHooks";
import DataSelector from "../dataSelector/DataSelector";
import { type ColumnMeta } from "../dataSelector/DataSelectorTypes";

type StreamGroupVideoStreamDataOutSelectorProps = {

  id: string;
  onSelectionChange?: (value: VideoStreamDto | VideoStreamDto[]) => void;
  streamGroup: StreamGroupDto;
};

const StreamGroupVideoStreamDataOutSelector = ({ id, streamGroup }: StreamGroupVideoStreamDataOutSelectorProps) => {
  const dataKey = id + '-StreamGroupVideoStreamDataOutSelector';

  const [videoStreams, setVideoStreams] = useState<VideoStreamDto[]>([] as VideoStreamDto[]);

  const { columnConfig: m3uFileNameColumnConfig } = useM3UFileNameColumnConfig(false);
  const { columnConfig: channelNumberColumnConfig } = useChannelNumberColumnConfig(false);
  const { columnConfig: channelNameColumnConfig } = useChannelNameColumnConfig(false);
  const { columnConfig: channelGroupConfig } = useChannelGroupColumnConfig(false);

  const [showHidden, setShowHidden] = useLocalStorage<boolean | null | undefined>(undefined, id + '-showHidden');

  const streamGroupsGetStreamGroupVideoStreamsQuery = useStreamGroupsGetStreamGroupVideoStreamsQuery(streamGroup.id as StreamGroupsGetStreamGroupVideoStreamsApiArg);

  const [streamGroupsRemoveVideoStreamToStreamGroupMutation] = useStreamGroupsRemoveVideoStreamToStreamGroupMutation();

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
          onChange={(e: TriStateCheckboxChangeEvent) => { setShowHidden(e.value); }}
          tooltip={getToolTip(showHidden)}
          tooltipOptions={getTopToolOptions}
          value={showHidden} />

      </div>
    );

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [showHidden]);

  return (
    <DataSelector
      columns={targetColumns}
      dataSource={videoStreams}
      defaultSortField="user_tvg_name"
      emptyMessage="No Streams"
      headerName={GetMessage('streams')}
      headerRightTemplate={rightHeaderTemplate}
      id={dataKey}
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
      showHidden={showHidden}
      style={{ height: 'calc(100vh - 40px)' }
      }
    />
  );
}

StreamGroupVideoStreamDataOutSelector.displayName = 'Stream Editor';


export default memo(StreamGroupVideoStreamDataOutSelector);
