
import { type CSSProperties } from "react";
import React from "react";

import * as StreamMasterApi from '../store/iptvApi';

import { addOrUpdateValueForField, GetMessage, type DataTableFilterMetaData } from "../common/common";
import { getTopToolOptions } from "../common/common";
import { UpdateVideoStream } from "../store/signlar_functions";
import { useLocalStorage } from "primereact/hooks";
import { type TriStateCheckboxChangeEvent } from "primereact/tristatecheckbox";
import { TriStateCheckbox } from "primereact/tristatecheckbox";
import VideoStreamAddPanel from "./VideoStreamAddDialog";
import IconSelector from "./IconSelector";
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

const VideoStreamDataSelector = (props: VideoStreamDataSelectorProps) => {

  const [enableEditMode, setEnableEditMode] = useLocalStorage(true, props.id + '-enableEditMode');

  const [selectedVideoStreams, setSelectedVideoStreams] = React.useState<StreamMasterApi.VideoStreamDto[]>([] as StreamMasterApi.VideoStreamDto[]);
  const [showHidden, setShowHidden] = useLocalStorage<boolean | null | undefined>(undefined, props.id + '-showHidden');

  const [dataFilters, setDataFilters] = React.useState<DataTableFilterMetaData[]>([] as DataTableFilterMetaData[]);
  const [filters, setFilters] = React.useState<string>('');

  const [pageSize, setPageSize] = React.useState<number>(25);
  const [pageNumber, setPageNumber] = React.useState<number>(1);
  const [orderBy, setOrderBy] = React.useState<string>('user_tvg_name');

  const videoStreamsQuery = StreamMasterApi.useVideoStreamsGetVideoStreamsForChannelGroupsQuery({ jsonFiltersString: filters, orderBy: orderBy ?? 'name', pageNumber: pageNumber === 0 ? 1 : pageNumber, pageSize: pageSize } as StreamMasterApi.VideoStreamsGetVideoStreamsForChannelGroupsApiArg);

  React.useEffect(() => {
    const callback = (event: KeyboardEvent) => {
      if ((event.ctrlKey) && event.code === 'KeyE') {
        event.preventDefault();
        setEnableEditMode(!enableEditMode);
      }

    };

    document.addEventListener('keydown', callback);
    return () => {
      document.removeEventListener('keydown', callback);
    };
  }, [enableEditMode, setEnableEditMode]);

  const ids = React.useMemo((): StreamMasterApi.ChannelNumberPair[] => {

    if (selectedVideoStreams.length === 0) {
      return [] as StreamMasterApi.ChannelNumberPair[];
    }

    let ret = selectedVideoStreams.map((a: StreamMasterApi.VideoStreamDto) => {
      return {
        channelNumber: a.user_Tvg_chno,
        id: a.id
      } as StreamMasterApi.ChannelNumberPair;
    });;


    return ret;

  }, [selectedVideoStreams]);

  const targetActionBodyTemplate = React.useCallback((data: StreamMasterApi.VideoStreamDto) => {
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

  const onUpdateVideoStream = React.useCallback(async (
    oldData: StreamMasterApi.VideoStreamDto,
    newName?: string | null,
    channelNumber?: number | null,
    Logo?: string | null,
    EPGID?: string | null,
  ) => {
    if (oldData.id === '') {
      return;
    }

    if (!newName && !channelNumber && (Logo === null || Logo === undefined) && (EPGID === null || EPGID === undefined)) {
      return;
    }

    const data = {} as StreamMasterApi.UpdateVideoStreamRequest;

    data.id = oldData.id;

    if (newName && newName !== '' && oldData.user_Tvg_name !== newName) {
      data.tvg_name = newName;
    }

    if (Logo !== null && oldData.user_Tvg_logo !== Logo) {
      data.tvg_logo = Logo;
    }

    if (EPGID !== null && oldData.user_Tvg_ID !== EPGID) {
      data.tvg_ID = EPGID;
    }

    if (channelNumber && channelNumber > 0 && oldData.user_Tvg_chno !== channelNumber) {
      data.tvg_chno = channelNumber;
    }

    await UpdateVideoStream(data)
      .then(() => {
      }).catch(() => {
      });

  }, []);

  const logoEditorBodyTemplate = React.useCallback((data: StreamMasterApi.VideoStreamDto) => {
    return (
      <IconSelector
        className="p-inputtext-sm"
        enableEditMode
        onChange={
          async (e: string) => {
            await onUpdateVideoStream(data, null, null, e);
          }
        }
        value={data.user_Tvg_logo}
      />

    );
  }, [onUpdateVideoStream]);

  const channelNameEditorBodyTemplate = React.useCallback((data: StreamMasterApi.VideoStreamDto) => {
    return (
      <ChannelNameEditor
        data={data}
        enableEditMode={enableEditMode}
      />
    )
  }, [enableEditMode]);

  const channelNumberEditorBodyTemplate = React.useCallback((data: StreamMasterApi.VideoStreamDto) => {
    return (
      <ChannelNumberEditor
        data={data}
        enableEditMode={enableEditMode}
      />
    )
  }, [enableEditMode]);

  const epgEditorBodyTemplate = React.useCallback((data: StreamMasterApi.VideoStreamDto) => {
    return (
      <EPGSelector
        data={data}
        enableEditMode={enableEditMode}
        value={data.user_Tvg_ID}
      />
    );
  }, [enableEditMode]);

  const channelGroupEditorBodyTemplate = React.useCallback((data: StreamMasterApi.VideoStreamDto) => {

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

  const rightHeaderTemplate = React.useMemo(() => {
    const getToolTip = (value: boolean | null | undefined) => {
      if (value === null) {
        return 'Show All';
      }

      if (value === true) {
        return 'Show Visible';
      }

      return 'Show Hidden';
    }

    return (
      <div className="flex justify-content-end align-items-center w-full gap-1" >

        <TriStateCheckbox
          onChange={(e: TriStateCheckboxChangeEvent) => { setShowHidden(e.value); }}
          tooltip={getToolTip(showHidden)}
          tooltipOptions={getTopToolOptions}
          value={showHidden} />

        {/* <Checkbox
          checked={enableEditMode}

          onChange={(e: CheckboxChangeEvent) => {
            setEnableEditMode(e.checked ?? false);
          }}

          tooltip={`Edit Mode ${enableEditMode ? 'Enabled' : 'Disabled'}  `}
          tooltipOptions={getTopToolOptions}
        /> */}

        <VideoStreamResetLogosDialog values={selectedVideoStreams} />

        <VideoStreamSetEPGsFromNameDialog values={selectedVideoStreams} />

        <VideoStreamSetLogosFromEPGDialog values={selectedVideoStreams} />

        <AutoSetChannelNumbers ids={ids} />

        <VideoStreamVisibleDialog iconFilled values={selectedVideoStreams} />

        <VideoStreamDeleteDialog values={selectedVideoStreams} />
        {JSON.stringify(props.channelGroups?.length ?? -1)}
        <VideoStreamAddPanel group={props.channelGroups !== undefined && props.channelGroups.length > 0 ? props.channelGroups[0].name : undefined} />

      </div>
    );

  }, [ids, props.channelGroups, selectedVideoStreams, setShowHidden, showHidden]);

  React.useEffect(() => {
    if (!props.channelGroups) {
      return;
    }

    const tosend = [] as DataTableFilterMetaData[];

    dataFilters.forEach((item) => {
      const newValue = { ...item } as DataTableFilterMetaData;
      tosend.push(newValue);

    })

    const findIndex = tosend.findIndex((a) => a.matchMode === 'channelGroups');
    if (findIndex !== -1) {
      tosend.splice(findIndex, 1);
    }

    if (props.channelGroups && props.channelGroups.length > 0) {
      const channelNames = JSON.stringify(props.channelGroups.map(a => a.name));
      addOrUpdateValueForField(tosend, 'user_Tvg_group', 'channelGroups', channelNames);
    }

    setFilters(JSON.stringify(tosend));
    setDataFilters(tosend);

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [props.channelGroups]);

  const setFilter = React.useCallback((toFilter: DataTableFilterEvent): DataTableFilterMetaData[] => {

    if (toFilter === undefined || toFilter.filters === undefined) {
      return [] as DataTableFilterMetaData[];
    }

    const tosend = [] as DataTableFilterMetaData[];
    Object.keys(toFilter.filters).forEach((key) => {
      const value = toFilter.filters[key] as DataTableFilterMetaData;
      if (value.value === null || value.value === undefined || value.value === '') {
        return;
      }

      const newValue = { ...value } as DataTableFilterMetaData;
      newValue.fieldName = key;
      newValue.valueType = typeof value.value;
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
      columns={targetColumns}
      dataSource={videoStreamsQuery.data}
      emptyMessage="No Streams"
      headerRightTemplate={rightHeaderTemplate}
      id={props.id + 'VideoStreamDataSelector'}
      isLoading={videoStreamsQuery.isLoading || videoStreamsQuery.isFetching}
      leftColSize={1}
      name={GetMessage('playlist streams')}
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
        setSelectedVideoStreams(e as StreamMasterApi.VideoStreamDto[]);
        props.onSelectionChange?.(e as StreamMasterApi.VideoStreamDto[]);
      }}
      onSort={(sortInfo) => {
        if (sortInfo.sortField !== null && sortInfo.sortField !== undefined) {
          if (sortInfo.sortOrder === 1) {
            setOrderBy(sortInfo.sortField + " asc");
          }
          else {
            setOrderBy(sortInfo.sortField + " desc");
          }
        }

      }}
      rightColSize={4}
      selectionMode='multiple'
      showHidden={showHidden}
      style={{ height: 'calc(100vh - 40px)' }}
    />
  );
}

VideoStreamDataSelector.displayName = 'Stream Editor';
VideoStreamDataSelector.defaultProps = {
  channelGroups: [] as StreamMasterApi.ChannelGroupDto[],
};

export type VideoStreamDataSelectorProps = {
  channelGroups?: StreamMasterApi.ChannelGroupDto[];
  id: string;
  onSelectionChange?: (value: StreamMasterApi.VideoStreamDto | StreamMasterApi.VideoStreamDto[]) => void;
};

export default React.memo(VideoStreamDataSelector);
