import { TriStateCheckbox, type TriStateCheckboxChangeEvent } from "primereact/tristatecheckbox";
import { memo, useCallback, useEffect, useMemo, useState, type CSSProperties } from "react";
import { useQueryAdditionalFilters } from "../../app/slices/useQueryAdditionalFilters";
import { useShowHidden } from "../../app/slices/useShowHidden";
import { GetMessage, arraysContainSameStrings, getTopToolOptions } from "../../common/common";
import { useChannelGroupColumnConfig, useChannelLogoColumnConfig, useChannelNameColumnConfig, useChannelNumberColumnConfig, useEPGColumnConfig } from "../../components/columns/columnConfigHooks";
import DataSelector from "../../components/dataSelector/DataSelector";
import { type ColumnMeta } from "../../components/dataSelector/DataSelectorTypes";
import AutoSetChannelNumbers from "../../components/videoStream/AutoSetChannelNumbers";
import VideoStreamAddDialog from "../../components/videoStream/VideoStreamAddDialog";
import VideoStreamDeleteDialog from "../../components/videoStream/VideoStreamDeleteDialog";
import VideoStreamEditDialog from "../../components/videoStream/VideoStreamEditDialog";
import VideoStreamResetLogoDialog from "../../components/videoStream/VideoStreamResetLogoDialog";
import VideoStreamResetLogosDialog from "../../components/videoStream/VideoStreamResetLogosDialog";
import VideoStreamSetLogoFromEPGDialog from "../../components/videoStream/VideoStreamSetLogoFromEPGDialog";
import VideoStreamSetLogosFromEPGDialog from "../../components/videoStream/VideoStreamSetLogosFromEPGDialog";
import VideoStreamVisibleDialog from "../../components/videoStream/VideoStreamVisibleDialog";
import { useVideoStreamsGetVideoStreamsQuery, type VideoStreamDto } from "../../store/iptvApi";

type ChannelGroupVideoStreamDataSelectorProps = {
  readonly channelGroupNames?: string[];
  readonly enableEditMode?: boolean;
  readonly id: string;
  readonly onSelectionChange?: (value: VideoStreamDto | VideoStreamDto[]) => void;
  readonly reorderable?: boolean;
};

const ChannelGroupVideoStreamDataSelector = (props: ChannelGroupVideoStreamDataSelectorProps) => {
  const dataKey = props.id + '-ChannelGroupVideoStreamDataSelector';

  const [enableEditMode, setEnableEditMode] = useState<boolean>(true);

  const { columnConfig: epgColumnConfig } = useEPGColumnConfig(enableEditMode);
  const { columnConfig: channelNumberColumnConfig } = useChannelNumberColumnConfig(enableEditMode);
  const { columnConfig: channelNameColumnConfig } = useChannelNameColumnConfig(enableEditMode);
  const { columnConfig: channelLogoColumnConfig } = useChannelLogoColumnConfig(enableEditMode);


  // eslint-disable-next-line @typescript-eslint/require-array-sort-compare
  const { columnConfig: channelGroupConfig } = useChannelGroupColumnConfig(enableEditMode, [...(props.channelGroupNames ?? [])].sort());
  const { queryAdditionalFilter, setQueryAdditionalFilter } = useQueryAdditionalFilters(dataKey);
  const { showHidden, setShowHidden } = useShowHidden(dataKey);
  const [selectedVideoStreams, setSelectedVideoStreams] = useState<VideoStreamDto[]>([] as VideoStreamDto[]);

  useEffect(() => {
    if (!arraysContainSameStrings(queryAdditionalFilter?.values, props.channelGroupNames)) {
      setQueryAdditionalFilter({ field: 'user_Tvg_group', matchMode: 'equals', values: props.channelGroupNames });
    }

  }, [props.channelGroupNames, dataKey, queryAdditionalFilter, setQueryAdditionalFilter]);

  useEffect(() => {
    if (props.enableEditMode != enableEditMode) {
      setEnableEditMode(props.enableEditMode ?? true);
    }


    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [props.enableEditMode]);

  const targetActionBodyTemplate = useCallback((data: VideoStreamDto) => {
    return (
      <div className='flex p-0 justify-content-end align-items-center'>
        <VideoStreamResetLogoDialog value={data} />
        <VideoStreamSetLogoFromEPGDialog value={data} />
        <VideoStreamVisibleDialog iconFilled={false} id={dataKey} skipOverLayer values={[data]} />
        <VideoStreamDeleteDialog iconFilled={false} id={dataKey} values={[data]} />
        <VideoStreamEditDialog value={data} />
      </div>
    );
  }, [dataKey]);

  const targetColumns = useMemo((): ColumnMeta[] => {
    let columnConfigs = [
      channelNumberColumnConfig,
      channelLogoColumnConfig,
      channelNameColumnConfig,
    ];

    columnConfigs.push(channelGroupConfig);
    columnConfigs.push(epgColumnConfig);

    columnConfigs.push({
      bodyTemplate: targetActionBodyTemplate, field: 'isHidden', header: 'Actions', isHidden: !enableEditMode, resizeable: false, sortable: false,
      style: {
        maxWidth: '7rem',
        width: '7rem',
      } as CSSProperties,
    });

    return columnConfigs;

  }, [channelGroupConfig, channelLogoColumnConfig, channelNameColumnConfig, channelNumberColumnConfig, enableEditMode, epgColumnConfig, targetActionBodyTemplate]);


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
      <div className="flex justify-content-end align-items-center w-full gap-1">
        <TriStateCheckbox
          className='sm-tristatecheckbox'
          onChange={(e: TriStateCheckboxChangeEvent) => setShowHidden(e.value)}
          tooltip={getToolTip(showHidden)}
          tooltipOptions={getTopToolOptions}
          value={showHidden}
        />
        <VideoStreamResetLogosDialog id={dataKey} values={selectedVideoStreams} />
        <VideoStreamSetLogosFromEPGDialog id={dataKey} values={selectedVideoStreams} />
        <AutoSetChannelNumbers id={dataKey} values={selectedVideoStreams} />
        <VideoStreamVisibleDialog iconFilled id={dataKey} values={selectedVideoStreams} />
        <VideoStreamDeleteDialog iconFilled id={dataKey} values={selectedVideoStreams} />
        <VideoStreamAddDialog group={props.channelGroupNames?.[0]} />
      </div>
    );
  }, [dataKey, props.channelGroupNames, selectedVideoStreams, setShowHidden, showHidden]);

  return (
    <DataSelector
      columns={targetColumns}
      defaultSortField="user_tvg_name"
      defaultSortOrder={1}
      emptyMessage="No Streams"
      headerName={GetMessage('streams')}
      headerRightTemplate={rightHeaderTemplate}
      id={dataKey}
      onSelectionChange={(value, selectAll) => {
        if (selectAll !== true) {
          setSelectedVideoStreams(value as VideoStreamDto[]);
          props.onSelectionChange?.(value as VideoStreamDto[]);
        }
      }}
      queryFilter={useVideoStreamsGetVideoStreamsQuery}
      reorderable={props.reorderable}
      selectionMode='multiple'
      style={{ height: 'calc(100vh - 40px)' }}
    />
  );
}

ChannelGroupVideoStreamDataSelector.displayName = 'Stream Editor';
ChannelGroupVideoStreamDataSelector.defaultProps = {
  channelGroupNames: [] as string[],
  reorderable: false
};


export default memo(ChannelGroupVideoStreamDataSelector);
