import { useLocalStorage } from "primereact/hooks";
import { type TriStateCheckboxChangeEvent } from "primereact/tristatecheckbox";
import { TriStateCheckbox } from "primereact/tristatecheckbox";
import { type CSSProperties } from "react";
import { useState, useEffect, useCallback, useMemo, memo } from "react";
import { arraysContainSameStrings } from "../../common/common";
import { getTopToolOptions, GetMessage } from "../../common/common";
import { type VideoStreamDto, type ChannelNumberPair } from "../../store/iptvApi";
import { useVideoStreamsGetVideoStreamsQuery } from "../../store/iptvApi";
import AutoSetChannelNumbers from "../AutoSetChannelNumbers";
import { useChannelGroupColumnConfig, useM3UFileNameColumnConfig, useEPGColumnConfig, useChannelNumberColumnConfig, useChannelNameColumnConfig, useChannelLogoColumnConfig } from "../columns/columnConfigHooks";
import DataSelector from "../dataSelector/DataSelector";
import { type ColumnMeta } from "../dataSelector/DataSelectorTypes";
import VideoStreamDeleteDialog from "../videoStream/VideoStreamDeleteDialog";
import VideoStreamEditDialog from "../videoStream/VideoStreamEditDialog";
import VideoStreamResetLogoDialog from "../videoStream/VideoStreamResetLogoDialog";
import VideoStreamResetLogosDialog from "../videoStream/VideoStreamResetLogosDialog";
import VideoStreamSetLogoFromEPGDialog from "../videoStream/VideoStreamSetLogoFromEPGDialog";
import VideoStreamVisibleDialog from "../videoStream/VideoStreamVisibleDialog";
import VideoStreamAddDialog from "../videoStream/VideoStreamAddDialog";
import VideoStreamSetLogosFromEPGDialog from "../videoStream/VideoStreamSetLogosFromEPGDialog";
import { useQueryAdditionalFilters } from "../../app/slices/useQueryAdditionalFilters";

type VideoStreamDataSelectorProps = {
  channelGroupNames?: string[];
  enableEditMode?: boolean;
  id: string;
  onSelectionChange?: (value: VideoStreamDto | VideoStreamDto[]) => void;
  showBrief?: boolean;
};

const VideoStreamDataSelector = (props: VideoStreamDataSelectorProps) => {
  const dataKey = props.id + '-VideoStreamDataSelector';

  const [enableEditMode, setEnableEditMode] = useState<boolean>(true);
  const [selectAll, setSelectAll] = useState<boolean>(false);
  const [totalRecords, setTotalRecords] = useState<number | undefined>(undefined);
  const { columnConfig: m3uFileNameColumnConfig } = useM3UFileNameColumnConfig(enableEditMode);
  const { columnConfig: epgColumnConfig } = useEPGColumnConfig(enableEditMode);
  const { columnConfig: channelNumberColumnConfig } = useChannelNumberColumnConfig(enableEditMode);
  const { columnConfig: channelNameColumnConfig } = useChannelNameColumnConfig(enableEditMode);
  const { columnConfig: channelLogoColumnConfig } = useChannelLogoColumnConfig(enableEditMode);

  // eslint-disable-next-line @typescript-eslint/require-array-sort-compare
  const { columnConfig: channelGroupConfig } = useChannelGroupColumnConfig(enableEditMode, [...(props.channelGroupNames ?? [])].sort());

  const { queryAdditionalFilter, setQueryAdditionalFilter } = useQueryAdditionalFilters(dataKey);
  const [selectedVideoStreams, setSelectedVideoStreams] = useState<VideoStreamDto[]>([] as VideoStreamDto[]);
  const [showHidden, setShowHidden] = useLocalStorage<boolean | null | undefined>(undefined, props.id + '-showHidden');

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
        <VideoStreamVisibleDialog iconFilled={false} id={props.id} skipOverLayer values={[data]} />
        <VideoStreamDeleteDialog iconFilled={false} value={data} />
        <VideoStreamEditDialog iconFilled={false} value={data} />
      </div>
    );
  }, [props.id]);

  const targetColumns = useMemo((): ColumnMeta[] => {
    let columnConfigs = [
      channelNumberColumnConfig,
      channelLogoColumnConfig,
      channelNameColumnConfig,
    ];

    // if (channelGroupIsLoading !== true) {
    columnConfigs.push(channelGroupConfig);
    // }

    // if (epgEditorIsLoading !== true) {
    columnConfigs.push(epgColumnConfig);
    // }

    columnConfigs.push({
      bodyTemplate: targetActionBodyTemplate, field: 'isHidden', header: 'Actions', isHidden: !enableEditMode, resizeable: false, sortable: false,
      style: {
        maxWidth: '7rem',
        width: '7rem',
      } as CSSProperties,
    });

    return columnConfigs;

  }, [channelGroupConfig, channelLogoColumnConfig, channelNameColumnConfig, channelNumberColumnConfig, enableEditMode, epgColumnConfig, targetActionBodyTemplate]);

  const targetBriefColumns = useMemo((): ColumnMeta[] => {

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
    let ids: ChannelNumberPair[] = [];

    if (selectedVideoStreams !== undefined && selectedVideoStreams.length > 0) {
      ids = selectedVideoStreams?.map((a: VideoStreamDto) => ({
        channelNumber: a.user_Tvg_chno,
        id: a.id
      })) ?? [];
    }

    return (
      <div className="flex justify-content-end align-items-center w-full gap-1">
        <TriStateCheckbox
          onChange={(e: TriStateCheckboxChangeEvent) => setShowHidden(e.value)}
          tooltip={getToolTip(showHidden)}
          tooltipOptions={getTopToolOptions}
          value={showHidden}
        />
        <VideoStreamResetLogosDialog overrideTotalRecords={selectAll ? totalRecords : undefined} values={selectedVideoStreams} />
        {/* <VideoStreamSetEPGsFromNameDialog overrideTotalRecords={selectAll ? totalRecords : undefined} values={selectedVideoStreams} /> */}
        <VideoStreamSetLogosFromEPGDialog overrideTotalRecords={selectAll ? totalRecords : undefined} values={selectedVideoStreams} />
        <AutoSetChannelNumbers ids={ids} overrideTotalRecords={selectAll ? totalRecords : undefined} />
        <VideoStreamVisibleDialog iconFilled id={props.id} overrideTotalRecords={selectAll ? totalRecords : undefined} selectAll={selectAll} values={selectedVideoStreams} />
        <VideoStreamDeleteDialog overrideTotalRecords={selectAll ? totalRecords : undefined} values={selectedVideoStreams} />
        <VideoStreamAddDialog group={props.channelGroupNames?.[0]} />
      </div>
    );
  }, [props.channelGroupNames, props.id, selectAll, selectedVideoStreams, setShowHidden, showHidden, totalRecords]);


  const rightHeaderBriefTemplate = useMemo(() => {

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
      columns={props.showBrief === true ? targetBriefColumns : targetColumns}
      defaultSortField="user_tvg_name"
      emptyMessage="No Streams"
      headerName={GetMessage('streams')}
      headerRightTemplate={props.showBrief === true ? rightHeaderBriefTemplate : rightHeaderTemplate}
      id={dataKey}
      onSelectionChange={(value, selectAllReturn, retTotalRecords) => {
        setTotalRecords(retTotalRecords);
        setSelectAll(selectAllReturn)
        if (selectAll !== true) {
          setSelectedVideoStreams(value as VideoStreamDto[]);
          props.onSelectionChange?.(value as VideoStreamDto[]);
        }
      }}
      queryFilter={useVideoStreamsGetVideoStreamsQuery}
      reorderable
      selectionMode={props.showBrief === true ? 'single' : 'multiple'}
      showHidden={showHidden}
      style={{ height: 'calc(100vh - 40px)' }}
    />
  );
}

VideoStreamDataSelector.displayName = 'Stream Editor';
VideoStreamDataSelector.defaultProps = {
  channelGroupNames: [] as string[],
  showBrief: false
};


export default memo(VideoStreamDataSelector);
