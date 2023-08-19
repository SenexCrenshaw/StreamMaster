
import { type CSSProperties } from "react";
import React from "react";
import { addOrUpdateValueForField, GetMessage, type DataTableFilterMetaData } from "../common/common";
import { getTopToolOptions } from "../common/common";

import { useLocalStorage } from "primereact/hooks";
import { type TriStateCheckboxChangeEvent } from "primereact/tristatecheckbox";
import { TriStateCheckbox } from "primereact/tristatecheckbox";
import VideoStreamAddPanel from "./VideoStreamAddDialog";

import ChannelGroupEditor from "./ChannelGroupEditor";
import { type ColumnMeta } from '../features/dataSelector2/DataSelectorTypes2';
import ChannelNumberEditor from "./ChannelNumberEditor";
import ChannelNameEditor from "./ChannelNameEditor";
import EPGSelector from "./EPGSelector";
import VideoStreamDeleteDialog from "./VideoStreamDeleteDialog";
import VideoStreamVisibleDialog from "./VideoStreamVisibleDialog";
import VideoStreamEditDialog from "./VideoStreamEditDialog";
import VideoStreamSetIconFromEPGDialog from "./VideoStreamSetLogoFromEPGDialog";
import AutoSetChannelNumbers from "./AutoSetChannelNumbers";
import VideoStreamResetLogoDialog from "./VideoStreamResetLogoDialog";
import VideoStreamSetLogosFromEPGDialog from "./VideoStreamSetLogosFromEPGDialog";
import VideoStreamResetLogosDialog from "./VideoStreamResetLogosDialog";
import VideoStreamSetEPGFromNameDialog from "./VideoStreamSetEPGFromNameDialog";
import VideoStreamSetEPGsFromNameDialog from "./VideoStreamSetEPGsFromNameDialog";
import DataSelector2 from "../features/dataSelector2/DataSelector2";
import { type DataTableFilterEvent } from "primereact/datatable";
import { type VideoStreamDto, type VideoStreamsGetVideoStreamsApiArg, type ChannelNumberPair, type ChannelGroupDto } from "../store/iptvApi";
import { useVideoStreamsGetVideoStreamsQuery } from "../store/iptvApi";
import ChannelLogoEditor from "./ChannelLogoEditor";

const VideoStreamDataSelector = (props: VideoStreamDataSelectorProps) => {

  const [enableEditMode, setEnableEditMode] = React.useState<boolean>(true);

  const [selectedVideoStreams, setSelectedVideoStreams] = React.useState<VideoStreamDto[]>([] as VideoStreamDto[]);
  const [showHidden, setShowHidden] = useLocalStorage<boolean | null | undefined>(undefined, props.id + '-showHidden');

  const [dataFilters, setDataFilters] = React.useState<DataTableFilterMetaData[]>([] as DataTableFilterMetaData[]);
  const [filters, setFilters] = React.useState<string>('');

  const [pageSize, setPageSize] = React.useState<number>(25);
  const [pageNumber, setPageNumber] = React.useState<number>(1);
  const [orderBy, setOrderBy] = React.useState<string>('user_tvg_name asc');

  const videoStreamsQuery = useVideoStreamsGetVideoStreamsQuery({ jsonFiltersString: filters, orderBy: orderBy ?? 'user_tvg_name', pageNumber: pageNumber, pageSize: pageSize } as VideoStreamsGetVideoStreamsApiArg);

  React.useEffect(() => {
    if (props.enableEditMode != enableEditMode) {
      setEnableEditMode(props.enableEditMode ?? true);
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [props.enableEditMode]);

  const targetActionBodyTemplate = React.useCallback((data: VideoStreamDto) => {
    return (
      <div className='flex p-0 justify-content-end align-items-center'>
        <VideoStreamResetLogoDialog value={data} />
        <VideoStreamSetEPGFromNameDialog value={data} />
        <VideoStreamSetIconFromEPGDialog value={data} />
        <VideoStreamVisibleDialog iconFilled={false} skipOverLayer values={[data]} />
        <VideoStreamDeleteDialog iconFilled={false} value={data} />
        <VideoStreamEditDialog iconFilled={false} value={data} />
      </div>
    );
  }, []);


  const logoEditorBodyTemplate = React.useCallback((data: VideoStreamDto) => {
    return (
      <ChannelLogoEditor
        data={data}
        enableEditMode={enableEditMode}
      />

    );
  }, [enableEditMode]);

  const channelNameEditorBodyTemplate = React.useCallback((data: VideoStreamDto) => {
    return (
      <ChannelNameEditor
        data={data}
        enableEditMode={enableEditMode}
      />
    )
  }, [enableEditMode]);

  const channelNumberEditorBodyTemplate = React.useCallback((data: VideoStreamDto) => {
    return (
      <ChannelNumberEditor
        data={data}
        enableEditMode={enableEditMode}
      />
    )
  }, [enableEditMode]);

  const epgEditorBodyTemplate = React.useCallback((data: VideoStreamDto) => {
    return (
      <EPGSelector
        data={data}
        enableEditMode={enableEditMode}
        value={data.user_Tvg_ID}
      />
    );
  }, [enableEditMode]);

  const channelGroupEditorBodyTemplate = React.useCallback((data: VideoStreamDto) => {

    if (data.user_Tvg_group === undefined) {
      return <span />
    }

    if (!enableEditMode) {
      return <span>{data.user_Tvg_group}</span>
    }

    return <ChannelGroupEditor data={data} />

  }, [enableEditMode]);

  const targetColumns = React.useMemo((): ColumnMeta[] => {
    return [
      {
        bodyTemplate: channelNumberEditorBodyTemplate,
        field: 'user_Tvg_chno',
        filter: false,
        header: 'Ch.',
        sortable: true,
        style: {
          maxWidth: '1rem',
          width: '1rem',
        } as CSSProperties,
      },
      {
        bodyTemplate: logoEditorBodyTemplate,
        field: 'user_Tvg_logo',
        fieldType: 'image',
        header: "Logo"
      },
      {
        bodyTemplate: channelNameEditorBodyTemplate,
        field: 'user_Tvg_name',
        filter: true,
        header: 'Name',
        sortable: true,
      },
      {
        align: 'left',
        bodyTemplate: channelGroupEditorBodyTemplate,
        field: 'user_Tvg_group',
        filter: true,
        header: 'Group',
        sortable: true,
        style: {
          maxWidth: '18rem',
        } as CSSProperties,
      },
      {
        bodyTemplate: epgEditorBodyTemplate,
        field: 'user_Tvg_ID_DisplayName',
        fieldType: 'epg',
        filter: true,
        sortable: true,
        style: {
          maxWidth: '16rem',
        } as CSSProperties,
      },
      {
        bodyTemplate: targetActionBodyTemplate, field: 'isHidden', header: 'Actions', isHidden: !enableEditMode, sortable: true,
        style: {
          maxWidth: '8rem',
          width: '8rem',
        } as CSSProperties,
      },
    ]
  }, [channelNameEditorBodyTemplate, channelNumberEditorBodyTemplate, enableEditMode, epgEditorBodyTemplate, logoEditorBodyTemplate, channelGroupEditorBodyTemplate, targetActionBodyTemplate]);

  const targetBriefColumns = React.useMemo((): ColumnMeta[] => {
    return [
      {
        bodyTemplate: channelNumberEditorBodyTemplate,
        field: 'user_Tvg_chno',
        filter: false,
        header: 'Ch.',
        sortable: true,
        style: {
          maxWidth: '1rem',
          width: '1rem',
        } as CSSProperties,
      },

      {
        bodyTemplate: channelNameEditorBodyTemplate,
        field: 'user_Tvg_name',
        filter: true,
        header: 'Name',
        sortable: true,
        style: {
          maxWidth: '18rem',
          width: '18rem',
        } as CSSProperties,
      },
      {
        align: 'left',
        bodyTemplate: channelGroupEditorBodyTemplate,
        field: 'user_Tvg_group',
        filter: true,
        header: 'Group',
        sortable: true,
        style: {
          maxWidth: '8rem',
          width: '8rem',
        } as CSSProperties,
      },
      {
        field: 'm3UFileName',
        filter: true,
        header: 'File',
        sortable: true,
        style: {
          maxWidth: '8rem',
          width: '8rem',
        } as CSSProperties,
      }
    ]
  }, [channelNameEditorBodyTemplate, channelNumberEditorBodyTemplate, channelGroupEditorBodyTemplate]);

  const getToolTip = (value: boolean | null | undefined) => {
    if (value === null) {
      return 'Show All';
    }

    if (value === true) {
      return 'Show Visible';
    }

    return 'Show Hidden';
  };

  const rightHeaderTemplate = React.useMemo(() => {
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
        <VideoStreamAddPanel group={props.channelGroups?.[0]?.name} />
      </div>
    );
  }, [props.channelGroups, selectedVideoStreams, setShowHidden, showHidden]);


  const rightHeaderBriefTemplate = React.useMemo(() => {

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

  React.useEffect(() => {
    if (!props.channelGroups) {
      return;
    }

    const toSend: DataTableFilterMetaData[] = [];

    dataFilters.forEach((item) => {
      const newValue = { ...item } as DataTableFilterMetaData;
      toSend.push(newValue);
    })

    const findIndex = toSend.findIndex((a) => a.matchMode === 'channelGroups');
    if (findIndex !== -1) {
      toSend.splice(findIndex, 1);
    }

    if (props.channelGroups && props.channelGroups.length > 0) {
      const channelNames = JSON.stringify(props.channelGroups.map(a => a.name));
      addOrUpdateValueForField(toSend, 'user_Tvg_group', 'channelGroups', channelNames);
    }

    setFilters(JSON.stringify(toSend));
    setDataFilters(toSend);

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [props.channelGroups]);

  const setFilter = React.useCallback((toFilter: DataTableFilterEvent): DataTableFilterMetaData[] => {
    if (toFilter === undefined || toFilter.filters === undefined) {
      return [];
    }

    const tosend: DataTableFilterMetaData[] = [];
    Object.keys(toFilter.filters).forEach((key) => {
      const value = toFilter.filters[key] as DataTableFilterMetaData;
      if (value.value === null || value.value === undefined || value.value === '') {
        return;
      }

      const newValue: DataTableFilterMetaData = { ...value, fieldName: key };
      tosend.push(newValue);
    });

    if (props.channelGroups && props.channelGroups.length > 0) {
      const channelNames = JSON.stringify(props.channelGroups.map(a => a.name));
      addOrUpdateValueForField(tosend, 'user_Tvg_group', 'channelGroups', channelNames);
    }

    setFilters(JSON.stringify(tosend));
    setDataFilters(tosend);
    return tosend;
  }, [props.channelGroups]);

  return (
    <DataSelector2
      columns={props.showBrief === true ? targetBriefColumns : targetColumns}
      dataSource={videoStreamsQuery.data}
      emptyMessage="No Streams"
      headerRightTemplate={props.showBrief === true ? rightHeaderBriefTemplate : rightHeaderTemplate}
      id={props.id + 'VideoStreamDataSelector'}
      isLoading={videoStreamsQuery.isLoading || videoStreamsQuery.isFetching}

      name={GetMessage('streams')}
      onFilter={(filterInfo) => {
        setFilter(filterInfo as DataTableFilterEvent);
      }}
      onPage={(pageInfo) => {
        if (pageInfo.page !== undefined) {
          setPageNumber(pageInfo.page + 1);
        }

        if (pageInfo.rows !== undefined) {
          setPageSize(pageInfo.rows);
        }
      }}
      onSelectionChange={(e) => {
        setSelectedVideoStreams(e as VideoStreamDto[]);
        props.onSelectionChange?.(e as VideoStreamDto[]);
      }}

      onSort={setOrderBy}
      selectionMode={props.showBrief === true ? 'single' : 'multiple'}
      showClearButton={false}
      showHidden={showHidden}
      style={{ height: 'calc(100vh - 40px)' }}
    />
  );
}

VideoStreamDataSelector.displayName = 'Stream Editor';
VideoStreamDataSelector.defaultProps = {
  channelGroups: [] as ChannelGroupDto[],
  showBrief: false
};

export type VideoStreamDataSelectorProps = {
  channelGroups?: ChannelGroupDto[];
  enableEditMode?: boolean;
  id: string;
  onSelectionChange?: (value: VideoStreamDto | VideoStreamDto[]) => void;
  showBrief?: boolean;
};

export default React.memo(VideoStreamDataSelector);
