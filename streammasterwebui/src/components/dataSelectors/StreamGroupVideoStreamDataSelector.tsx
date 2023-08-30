/* eslint-disable react/no-unused-prop-types */
import { useLocalStorage } from "primereact/hooks";
import { type TriStateCheckboxChangeEvent } from "primereact/tristatecheckbox";
import { TriStateCheckbox } from "primereact/tristatecheckbox";

import { useMemo, memo, useEffect, useState } from "react";
import { getTopToolOptions, GetMessage } from "../../common/common";
import { type StreamGroupVideoStreamsAddVideoStreamToStreamGroupApiArg } from "../../store/iptvApi";
import { useStreamGroupVideoStreamsAddVideoStreamToStreamGroupMutation, type VideoStreamIsReadOnly } from "../../store/iptvApi";
import { useStreamGroupVideoStreamsGetStreamGroupVideoStreamIdsQuery, type StreamGroupDto } from "../../store/iptvApi";
import { type VideoStreamDto } from "../../store/iptvApi";
import { useVideoStreamsGetVideoStreamsQuery } from "../../store/iptvApi";
import { useChannelGroupColumnConfig, useM3UFileNameColumnConfig, useChannelNumberColumnConfig, useChannelNameColumnConfig } from "../columns/columnConfigHooks";
import DataSelector from "../dataSelector/DataSelector";
import { type ColumnMeta } from "../dataSelector/DataSelectorTypes";

type StreamGroupVideoStreamDataSelectorProps = {
  id: string;
  onSelectionChange?: (value: VideoStreamDto | VideoStreamDto[]) => void;
  streamGroup: StreamGroupDto;
};

const StreamGroupVideoStreamDataSelector = ({ id, streamGroup }: StreamGroupVideoStreamDataSelectorProps) => {
  const dataKey = id + '-StreamGroupVideoStreamDataSelector';

  const [videoStreams, setVideoStreams] = useState<VideoStreamIsReadOnly[]>([] as VideoStreamIsReadOnly[]);

  const { columnConfig: m3uFileNameColumnConfig } = useM3UFileNameColumnConfig(false);
  const { columnConfig: channelNumberColumnConfig } = useChannelNumberColumnConfig(false);
  const { columnConfig: channelNameColumnConfig } = useChannelNameColumnConfig(false);
  const { columnConfig: channelGroupConfig } = useChannelGroupColumnConfig(false);

  const [showHidden, setShowHidden] = useLocalStorage<boolean | null | undefined>(undefined, id + '-showHidden');

  const streamGroupsGetStreamGroupVideoStreamIdsQuery = useStreamGroupVideoStreamsGetStreamGroupVideoStreamIdsQuery(streamGroup.id);

  const [streamGroupVideoStreamsAddVideoStreamToStreamGroupMutation] = useStreamGroupVideoStreamsAddVideoStreamToStreamGroupMutation();

  useEffect(() => {
    if (streamGroupsGetStreamGroupVideoStreamIdsQuery.data !== undefined) {
      setVideoStreams(streamGroupsGetStreamGroupVideoStreamIdsQuery.data);
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [streamGroupsGetStreamGroupVideoStreamIdsQuery.data]);

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
      defaultSortField="user_tvg_name"
      emptyMessage="No Streams"
      headerName={GetMessage('streams')}
      headerRightTemplate={rightHeaderTemplate}
      id={dataKey}
      isLoading={streamGroupsGetStreamGroupVideoStreamIdsQuery.isLoading || streamGroupsGetStreamGroupVideoStreamIdsQuery.isFetching}
      onSelectionChange={async (value) => {
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

        const toSend = {} as StreamGroupVideoStreamsAddVideoStreamToStreamGroupApiArg;

        toSend.streamGroupId = streamGroup.id;
        toSend.videoStreamId = stream.id;

        await streamGroupVideoStreamsAddVideoStreamToStreamGroupMutation(toSend);
      }}
      queryFilter={useVideoStreamsGetVideoStreamsQuery}
      selectionMode='single'
      showHidden={showHidden}
      style={{ height: 'calc(100vh - 40px)' }
      }

      videoStreamIsReadOnlys={videoStreams}
    />
  );
}

StreamGroupVideoStreamDataSelector.displayName = 'Stream Editor';


export default memo(StreamGroupVideoStreamDataSelector);
