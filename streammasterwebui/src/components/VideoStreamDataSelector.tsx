/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable react/no-unused-prop-types */
// eslint-disable-next-line @typescript-eslint/no-unused-vars

import { type CSSProperties } from "react";
import React from "react";
import DataSelector from "../features/dataSelector/DataSelector";
import * as StreamMasterApi from '../store/iptvApi';
import { getTopToolOptions } from "../common/common";
import { Toast } from 'primereact/toast';
import { UpdateVideoStream } from "../store/signlar_functions";
import { useLocalStorage } from "primereact/hooks";
import { type TriStateCheckboxChangeEvent } from "primereact/tristatecheckbox";
import { TriStateCheckbox } from "primereact/tristatecheckbox";
import VideoStreamAddPanel from "./VideoStreamAddDialog";
import IconSelector from "./IconSelector";
import ChannelGroupEditor from "./ChannelGroupEditor";

import ChannelNumberEditor from "./ChannelNumberEditor";
import ChannelNameEditor from "./ChannelNameEditor";
import EPGSelector from "./EPGSelector";
import VideoStreamDeleteDialog from "./VideoStreamDeleteDialog";
import { type ColumnMeta } from "../features/dataSelector/DataSelectorTypes";
import VideoStreamVisibleDialog from "./VideoStreamVisibleDialog";
import VideoStreamEditDialog from "./VideoStreamEditDialog";

import VideoStreamSetIconFromEPGDialog from "./VideoStreamSetLogoFromEPGDialog";
import AutoSetChannelNumbers from "./AutoSetChannelNumbers";
import VideoStreamResetLogoDialog from "./VideoStreamResetLogoDialog";
import VideoStreamSetLogosFromEPGDialog from "./VideoStreamSetLogosFromEPGDialog";
import VideoStreamResetLogosDialog from "./VideoStreamResetLogosDialog";
import VideoStreamSetEPGFromNameDialog from "./VideoStreamSetEPGFromNameDialog";
import VideoStreamSetEPGsFromNameDialog from "./VideoStreamSetEPGsFromNameDialog";
import { useFilteredStreams } from "./useFilteredStreams";

const VideoStreamDataSelector = (props: VideoStreamDataSelectorProps) => {
  const toast = React.useRef<Toast>(null);

  const [enableEditMode, setEnableEditMode] = useLocalStorage(true, props.id + '-enableEditMode');

  // const [filteredStreams, setFilteredStreams] = React.useState<StreamMasterApi.VideoStreamDto[] | undefined>(undefined);
  const [selectedVideoStreams, setSelectedVideoStreams] = React.useState<StreamMasterApi.VideoStreamDto[]>([] as StreamMasterApi.VideoStreamDto[]);
  const [showHidden, setShowHidden] = useLocalStorage<boolean | null | undefined>(undefined, props.id + '-showHidden');

  const videoStreamsQuery = StreamMasterApi.useVideoStreamsGetVideoStreamsQuery({} as StreamMasterApi.VideoStreamsGetVideoStreamsApiArg);
  const channelGroupsQuery = StreamMasterApi.useChannelGroupsGetChannelGroupsQuery({} as StreamMasterApi.ChannelGroupsGetChannelGroupsApiArg);

  const filteredStreams = useFilteredStreams(channelGroupsQuery, props, videoStreamsQuery);


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



  React.useEffect(() => {
    if (filteredStreams !== undefined && filteredStreams.length > 0) {
      console.debug(filteredStreams.length);
      // setFilteredStreams(filteredStreamsOp);
    }

  }, [filteredStreams]);

  React.useEffect(() => {
    if (selectedVideoStreams === undefined || selectedVideoStreams.length === 0 || filteredStreams === undefined || filteredStreams.length === 0) {
      return;
    }

    let changed = false;

    const newSelectedVideoStreams = [] as StreamMasterApi.VideoStreamDto[];
    selectedVideoStreams.forEach((item) => {
      const test = filteredStreams.find((a) => a.id === item.id && a.user_Tvg_logo !== item.user_Tvg_logo);
      if (test !== undefined) {
        changed = true;
        newSelectedVideoStreams.push(test);
      }
    });

    if (changed) {
      setSelectedVideoStreams(newSelectedVideoStreams);
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [filteredStreams]);


  const targetActionBodyTemplate = React.useCallback((data: StreamMasterApi.VideoStreamDto) => {
    return (
      <div className='flex p-0 justify-content-end align-items-center'>
        <VideoStreamResetLogoDialog value={data} />
        <VideoStreamSetEPGFromNameDialog value={data} />
        <VideoStreamSetIconFromEPGDialog value={data} />
        <VideoStreamVisibleDialog iconFilled={false} skipOverLayer values={[data]} />
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
        if (toast.current) {
          toast.current.show({
            detail: `Updated Stream`,
            life: 3000,
            severity: 'success',
            summary: 'Successful',
          });

        }
      }).catch((e) => {
        if (toast.current) {
          toast.current.show({
            detail: `Update Stream Failed`,
            life: 3000,
            severity: 'error',
            summary: 'Error ' + e.message,
          });
        }
      });

  }, [toast]);

  const logoEditorBodyTemplate = React.useCallback((data: StreamMasterApi.VideoStreamDto) => {
    return (
      <IconSelector
        className="p-inputtext-sm"
        enableEditMode={enableEditMode}
        onChange={
          async (e: string) => {
            await onUpdateVideoStream(data, null, null, e);
          }
        }
        value={data.user_Tvg_logo}
      />

    );
  }, [enableEditMode, onUpdateVideoStream]);

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
        filter: false,
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

  const onsetSelectedVideoStreams = React.useCallback((selectedData: StreamMasterApi.VideoStreamDto | StreamMasterApi.VideoStreamDto[]) => {

    if (Array.isArray(selectedData)) {
      const newDatas = selectedData.filter((cg) => cg.id !== undefined);
      setSelectedVideoStreams(newDatas);
    } else {
      setSelectedVideoStreams([selectedData]);
    }

  }, []);


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

        {/* <AutoMatchIconToStreamsDialog ids={ids} /> */}

        <VideoStreamResetLogosDialog values={selectedVideoStreams} />
        <VideoStreamSetEPGsFromNameDialog values={selectedVideoStreams} />

        <VideoStreamSetLogosFromEPGDialog values={selectedVideoStreams} />

        <AutoSetChannelNumbers ids={ids} />

        <VideoStreamVisibleDialog iconFilled values={selectedVideoStreams} />

        <VideoStreamDeleteDialog values={selectedVideoStreams} />

        <VideoStreamAddPanel group={props.groups !== undefined && props.groups.length > 0 ? props.groups[0].name : undefined} />

      </div>
    );

  }, [ids, props.groups, selectedVideoStreams, setShowHidden, showHidden]);



  return (
    <>

      <Toast position="bottom-right" ref={toast} />
      <DataSelector
        columns={targetColumns}
        dataSource={filteredStreams}
        emptyMessage="No Streams"
        enableState={false}
        headerRightTemplate={rightHeaderTemplate}
        id={props.id + 'VideoStreamDataSelector'}
        isLoading={videoStreamsQuery.isLoading}
        leftColSize={1}
        name='Playlist Streams'
        onSelectionChange={(e) => onsetSelectedVideoStreams(e as StreamMasterApi.VideoStreamDto[])}
        rightColSize={4}
        selection={selectedVideoStreams}
        selectionMode='multiple'
        showHidden={showHidden}
        showSkeleton={videoStreamsQuery.isLoading || enableEditMode === undefined}
        sortField='user_Tvg_name'
        style={{ height: 'calc(100vh - 40px)' }}
      />

    </>
  );
}

VideoStreamDataSelector.displayName = 'Stream Editor';
VideoStreamDataSelector.defaultProps = {
  groups: [] as StreamMasterApi.ChannelGroupDto[],
};

export type VideoStreamDataSelectorProps = {
  groups?: StreamMasterApi.ChannelGroupDto[];
  id: string;
  m3uFileId?: number;
};

export default React.memo(VideoStreamDataSelector);
