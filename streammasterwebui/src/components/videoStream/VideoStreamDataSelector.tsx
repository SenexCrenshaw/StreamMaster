/* eslint-disable @typescript-eslint/no-unused-vars */
import { useLocalStorage } from "primereact/hooks";
import { type TriStateCheckboxChangeEvent } from "primereact/tristatecheckbox";
import { TriStateCheckbox } from "primereact/tristatecheckbox";
import { type CSSProperties } from "react";
import { useState, useEffect, useCallback, useMemo, memo } from "react";
import { type SMDataTableFilterMetaData } from "../../common/common";
import { getTopToolOptions, addOrUpdateValueForField, GetMessage } from "../../common/common";
import { type VideoStreamDto, type VideoStreamsGetVideoStreamsApiArg, type ChannelNumberPair } from "../../store/iptvApi";
import { useVideoStreamsGetVideoStreamsQuery } from "../../store/iptvApi";
import AutoSetChannelNumbers from "../AutoSetChannelNumbers";
import { useChannelGroupColumnConfig, useM3UFileNameColumnConfig, useEPGColumnConfig, useChannelNumberColumnConfig, useChannelNameColumnConfig, useChannelLogoColumnConfig } from "../columns/columnConfigHooks";
import { type LazyTableState } from "../dataSelector/DataSelector";
import DataSelector from "../dataSelector/DataSelector";
import { type ColumnMeta } from "../dataSelector/DataSelectorTypes";
import VideoStreamDeleteDialog from "./VideoStreamDeleteDialog";
import VideoStreamEditDialog from "./VideoStreamEditDialog";
import VideoStreamResetLogoDialog from "./VideoStreamResetLogoDialog";
import VideoStreamResetLogosDialog from "./VideoStreamResetLogosDialog";
import VideoStreamSetEPGFromNameDialog from "./VideoStreamSetEPGFromNameDialog";
import VideoStreamSetEPGsFromNameDialog from "./VideoStreamSetEPGsFromNameDialog";
import VideoStreamSetLogoFromEPGDialog from "./VideoStreamSetLogoFromEPGDialog";
import VideoStreamVisibleDialog from "./VideoStreamVisibleDialog";
import VideoStreamAddDialog from "./VideoStreamAddDialog";
import VideoStreamSetLogosFromEPGDialog from "./VideoStreamSetLogosFromEPGDialog";

type VideoStreamDataSelectorProps = {
  channelGroupNames?: string[];
  enableEditMode?: boolean;
  id: string;
  onSelectionChange?: (value: VideoStreamDto | VideoStreamDto[]) => void;
  showBrief?: boolean;
};

const VideoStreamDataSelector = (props: VideoStreamDataSelectorProps) => {

  const [enableEditMode, setEnableEditMode] = useState<boolean>(true);

  const m3uFileNameColumnConfig = useM3UFileNameColumnConfig(enableEditMode);
  const epgColumnConfig = useEPGColumnConfig(enableEditMode);
  const channelNumberColumnConfig = useChannelNumberColumnConfig(enableEditMode);
  const channelNameColumnConfig = useChannelNameColumnConfig(enableEditMode);
  const channelLogoColumnConfig = useChannelLogoColumnConfig(enableEditMode);

  const [lazyState, setLazyState] = useState<LazyTableState>({
    filters: {},
    filterString: '',
    first: 0,
    page: 1,
    rows: 25,
    sortField: 'user_tvg_name',
    sortOrder: undefined,
    sortString: 'user_tvg_name asc',
  });

  const videoStreamsQuery = useVideoStreamsGetVideoStreamsQuery({ jsonFiltersString: lazyState.filterString, orderBy: lazyState.sortString ?? 'user_tvg_name', pageNumber: lazyState.page, pageSize: lazyState.rows } as VideoStreamsGetVideoStreamsApiArg);
  const channelGroupConfig = useChannelGroupColumnConfig(enableEditMode, props.channelGroupNames?.sort() ?? []);

  const [selectedVideoStreams, setSelectedVideoStreams] = useState<VideoStreamDto[]>([] as VideoStreamDto[]);
  const [showHidden, setShowHidden] = useLocalStorage<boolean | null | undefined>(undefined, props.id + '-showHidden');

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
        <VideoStreamSetEPGFromNameDialog value={data} />
        <VideoStreamSetLogoFromEPGDialog value={data} />
        <VideoStreamVisibleDialog iconFilled={false} skipOverLayer values={[data]} />
        <VideoStreamDeleteDialog iconFilled={false} value={data} />
        <VideoStreamEditDialog iconFilled={false} value={data} />
      </div>
    );
  }, []);

  const targetColumns = useMemo((): ColumnMeta[] => {
    return [
      channelNumberColumnConfig,
      channelLogoColumnConfig,
      channelNameColumnConfig,
      channelGroupConfig,
      epgColumnConfig,
      {
        bodyTemplate: targetActionBodyTemplate, field: 'isHidden', header: 'Actions', isHidden: !enableEditMode, sortable: true,
        style: {
          maxWidth: '8rem',
          width: '8rem',
        } as CSSProperties,
      },
    ]
  }, [channelNumberColumnConfig, channelLogoColumnConfig, channelNameColumnConfig, channelGroupConfig, epgColumnConfig, targetActionBodyTemplate, enableEditMode]);

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
        <VideoStreamResetLogosDialog values={selectedVideoStreams} />
        <VideoStreamSetEPGsFromNameDialog values={selectedVideoStreams} />
        <VideoStreamSetLogosFromEPGDialog values={selectedVideoStreams} />
        <AutoSetChannelNumbers ids={ids} />
        <VideoStreamVisibleDialog iconFilled values={selectedVideoStreams} />
        <VideoStreamDeleteDialog values={selectedVideoStreams} />
        <VideoStreamAddDialog group={props.channelGroupNames?.[0]} />
      </div>
    );
  }, [props.channelGroupNames, selectedVideoStreams, setShowHidden, showHidden]);


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

  const updateFilter = useCallback((toFilter: LazyTableState): SMDataTableFilterMetaData[] => {
    // Early return if the filter is undefined or empty.
    if (!toFilter?.filters) {
      return [];
    }

    // Filter and map the filter keys to the desired format.
    const tosend: SMDataTableFilterMetaData[] = Object.keys(toFilter.filters)
      .map(key => {
        const value = toFilter.filters[key] as SMDataTableFilterMetaData;
        if (!value.value) {
          return null;
        }

        const newValue: SMDataTableFilterMetaData = { ...value, fieldName: key };
        toFilter.filters[key] = newValue;
        return newValue;
      })
      .filter(Boolean) as SMDataTableFilterMetaData[];

    if (props.channelGroupNames?.length) {
      const channelNames = JSON.stringify(props.channelGroupNames);
      addOrUpdateValueForField(tosend, 'user_Tvg_group', 'equals', channelNames);
    }

    toFilter.filterString = JSON.stringify(tosend);
    setLazyState(toFilter);

    return tosend;
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [props.channelGroupNames]);


  return (
    <DataSelector
      columns={props.showBrief === true ? targetBriefColumns : targetColumns}
      dataSource={videoStreamsQuery.data}
      emptyMessage="No Streams"
      headerName={GetMessage('streams')}
      headerRightTemplate={props.showBrief === true ? rightHeaderBriefTemplate : rightHeaderTemplate}
      id={props.id + 'VideoStreamDataSelector'}
      isLoading={videoStreamsQuery.isLoading || videoStreamsQuery.isFetching}
      onFilter={(info) => {
        console.log('filterInfo', info);
        updateFilter(info);
      }}
      onSelectionChange={(e) => {
        console.log('onSelectionChange', e as VideoStreamDto[]);
        setSelectedVideoStreams(e as VideoStreamDto[]);
        props.onSelectionChange?.(e as VideoStreamDto[]);
      }}

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
