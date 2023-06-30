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
import AutoSetChannelNumbers from "./AutoSetChannelNumbers";
import ChannelNumberEditor from "./ChannelNumberEditor";
import ChannelNameEditor from "./ChannelNameEditor";
import EPGSelector from "./EPGSelector";
import EPGFileAddDialog from "./EPGFileAddDialog";
import VideoStreamDeleteDialog from "./VideoStreamDeleteDialog";
import { type ColumnMeta } from "../features/dataSelector/DataSelectorTypes";
import { type DataTableRowDataArray } from "primereact/datatable";
import VideoStreamVisibleDialog from "./VideoStreamVisibleDialog";
import VideoStreamEditDialog from "./VideoStreamEditDialog";
import FileDialog from "./FileDialog";
// import AutoMatchIconToStreamsDialog from "./AutoMatchIconToStreamsDialog";

const VideoStreamDataSelector = (props: VideoStreamDataSelectorProps) => {
  const toast = React.useRef<Toast>(null);

  const [enableEditMode, setEnableEditMode] = useLocalStorage(true, props.id + '-enableEditMode');
  const [selectedM3UStreams, setSelectedM3UStreams] = React.useState<StreamMasterApi.VideoStreamDto[]>([] as StreamMasterApi.VideoStreamDto[]);
  const [showHidden, setShowHidden] = useLocalStorage<boolean | null | undefined>(undefined, props.id + '-showHidden');
  const [values, setValues] = React.useState<StreamMasterApi.VideoStreamDto[]>([] as StreamMasterApi.VideoStreamDto[]);

  const [addIcon, setAddIcon] = React.useState<boolean>(false);

  const videoStreamsQuery = StreamMasterApi.useVideoStreamsGetVideoStreamsQuery();

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


  const onValueChanged = React.useCallback((data: DataTableRowDataArray<StreamMasterApi.VideoStreamDto[]>) => {
    if (!data) {
      return;
    }

    setValues(data);
  }, []);

  const filteredStreams = React.useMemo((): StreamMasterApi.VideoStreamDto[] => {
    if (!videoStreamsQuery.data || videoStreamsQuery.data.length === 0) {
      return [] as StreamMasterApi.VideoStreamDto[];
    }

    let data = [] as StreamMasterApi.VideoStreamDto[];

    if (props.groups === undefined || props.groups.length === 0 || props.groups[0].name === undefined || props.groups.findIndex((a: StreamMasterApi.ChannelGroupDto) => a.name === 'All') !== -1) {
      data = videoStreamsQuery.data;
    } else {
      const names = props.groups.map((a: StreamMasterApi.ChannelGroupDto) => a.name.toLocaleLowerCase());
      data = videoStreamsQuery.data.filter((a: StreamMasterApi.VideoStreamDto) => a.user_Tvg_group !== undefined && names.includes(a.user_Tvg_group.toLocaleLowerCase()));
    }

    if (props.m3uFileId && props.m3uFileId > 0) {
      const d = data.filter((a: StreamMasterApi.VideoStreamDto) => a.m3UFileId === props.m3uFileId);
      return d;
    }

    return data;

  }, [videoStreamsQuery.data, props.groups, props.m3uFileId]);

  const ids = React.useMemo((): StreamMasterApi.ChannelNumberPair[] => {

    if (selectedM3UStreams.length === 0 && filteredStreams.length === 0) {
      return [] as StreamMasterApi.ChannelNumberPair[];
    }

    let ret = filteredStreams.map((a: StreamMasterApi.VideoStreamDto) => {
      return {
        channelNumber: a.user_Tvg_chno,
        id: a.id
      } as StreamMasterApi.ChannelNumberPair;
    });;

    let dataIds = ret.map((a: StreamMasterApi.ChannelNumberPair) => a.id);

    if (values.length !== 0 && values[0].id !== undefined) {
      dataIds = values.map((a: StreamMasterApi.VideoStreamDto) => a.id);
      ret = filteredStreams.filter((a: StreamMasterApi.VideoStreamDto) => dataIds.includes(a.id)).map((a: StreamMasterApi.VideoStreamDto) => {
        return {
          channelNumber: a.user_Tvg_chno,
          id: a.id
        } as StreamMasterApi.ChannelNumberPair;
      });
    }

    if (selectedM3UStreams.length !== 0 && selectedM3UStreams[0].id !== undefined) {
      const test = selectedM3UStreams.filter((a: StreamMasterApi.VideoStreamDto) => dataIds.includes(a.id)).map((a: StreamMasterApi.VideoStreamDto) => {
        return {
          channelNumber: a.user_Tvg_chno,
          id: a.id
        } as StreamMasterApi.ChannelNumberPair;
      });
      if (test.length !== 0) {
        ret = test;
      }
    }

    return ret;

  }, [filteredStreams, selectedM3UStreams, values]);


  const targetActionBodyTemplate = React.useCallback((data: StreamMasterApi.VideoStreamDto) => {
    return (
      <div className='flex p-0 justify-content-center align-items-center'>

        <VideoStreamVisibleDialog iconFilled={false} skipOverLayer values={[data]} />
        <VideoStreamEditDialog iconFilled={false} value={data} />
      </div>
    );
  }, []);

  /**
  * This function is a callback that is used to update an M3U stream.
  * It takes in several optional parameters that can be used to update
  * the stream's name, channel number, logo, and EPG ID.
  *
  * @param oldData - The old data for the stream that is being updated.
  * @param newName - The new name for the stream. Optional.
  * @param channelNumber - The new channel number for the stream. Optional.
  * @param Logo - The new logo for the stream. Optional.
  * @param EPGID - The new EPG ID for the stream. Optional.
  */
  const onUpdateVideoStream = React.useCallback(async (
    oldData: StreamMasterApi.VideoStreamDto,
    newName?: string | null,
    channelNumber?: number | null,
    Logo?: string | null,
    EPGID?: string | null,
  ) => {
    if (oldData.id < 0) {
      return;
    }

    if (!newName && !channelNumber && !Logo && !EPGID) {
      return;
    }

    const data = {} as StreamMasterApi.UpdateVideoStreamRequest;

    data.id = oldData.id;

    if (newName && newName !== '' && oldData.user_Tvg_name !== newName) {
      data.tvg_name = newName;
    }

    if (Logo && Logo !== '' && oldData.user_Tvg_logo !== Logo) {
      data.tvg_logo = Logo;
    }

    if (EPGID && EPGID !== '' && oldData.user_Tvg_ID !== EPGID) {
      data.tvg_ID = EPGID;
    }

    if (channelNumber && channelNumber > 0 && oldData.user_Tvg_chno !== channelNumber) {
      data.tvg_chno = channelNumber;
    }

    await UpdateVideoStream(data)
      .then((result) => {
        if (toast.current) {
          if (result) {
            toast.current.show({
              detail: `Updated Stream`,
              life: 3000,
              severity: 'success',
              summary: 'Successful',
            });
          } else {
            toast.current.show({
              detail: `Update Stream Failed`,
              life: 3000,
              severity: 'error',
              summary: 'Error',
            });
          }

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
        onAddIcon={() => setAddIcon(true)}
        onChange={
          async (e: StreamMasterApi.IconFileDto) => {

            // const newiconSource = e.originalSource.includes('://')
            //   ? e.originalSource
            //   : e.name;

            await onUpdateVideoStream(data, null, null, e.originalSource);
          }
        }
        onReset={
          async (e: StreamMasterApi.IconFileDto) => {

            // const newiconSource = e.originalSource.includes('://')
            //   ? e.originalSource
            //   : e.name;

            await onUpdateVideoStream(data, null, null, e.originalSource);
          }
        }
        resetValue={data.tvg_logo}
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
        field: 'id',
        filter: false,
        header: 'id',
        sortable: true,
        style: {
          maxWidth: '4rem',
          width: '4rem',
        } as CSSProperties,
      },
      {
        bodyTemplate: channelNumberEditorBodyTemplate,
        field: 'user_Tvg_chno',
        filter: false,
        header: 'Ch.',
        sortable: true,
        style: {
          maxWidth: '4rem',
          width: '4rem',
        } as CSSProperties,
      },

      {
        bodyTemplate: channelNameEditorBodyTemplate,
        field: 'user_Tvg_name',
        filter: true,
        header: 'Name',
        sortable: true,
        style: {
          width: '22rem',
        } as CSSProperties,
      },
      {
        bodyTemplate: logoEditorBodyTemplate,
        field: 'user_Tvg_logo',
        fieldType: 'image',
        header: "Logo"
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
        sortable: true,
        style: {
          maxWidth: '16rem',
        } as CSSProperties,
      },
      {
        bodyTemplate: targetActionBodyTemplate, field: 'isHidden', fieldType: 'isHidden', isHidden: !enableEditMode, sortable: true,
        style: {
          maxWidth: '8rem',
          width: '8rem',
        } as CSSProperties,
      },
    ]
  }, [channelNameEditorBodyTemplate, channelNumberEditorBodyTemplate, enableEditMode, epgEditorBodyTemplate, logoEditorBodyTemplate, channelGroupEditorBodyTemplate, targetActionBodyTemplate]);

  const onsetSelectedM3UStreams = React.useCallback((selectedData: StreamMasterApi.VideoStreamDto | StreamMasterApi.VideoStreamDto[]) => {

    if (Array.isArray(selectedData)) {
      const newDatas = selectedData.filter((cg) => cg.id !== undefined);
      setSelectedM3UStreams(newDatas);
    } else {
      setSelectedM3UStreams([selectedData]);
    }

  }, []);


  const videoStreamDelete = React.useCallback((videoStreamDeleteids: number[]) => {
    const test = selectedM3UStreams.filter((item) => !videoStreamDeleteids.includes(item.id));
    setSelectedM3UStreams(test);
  }, [selectedM3UStreams]);

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

        <AutoSetChannelNumbers ids={ids} />

        <VideoStreamVisibleDialog iconFilled values={selectedM3UStreams} />

        <VideoStreamDeleteDialog onChange={videoStreamDelete} values={selectedM3UStreams} />

        <VideoStreamAddPanel group={props.groups !== undefined && props.groups.length > 0 ? props.groups[0].name : undefined} />

      </div>
    );

  }, [ids, props.groups, selectedM3UStreams, setShowHidden, showHidden, videoStreamDelete]);

  const leftHeaderTemplate = React.useMemo(() => {
    return (

      <EPGFileAddDialog />

    );
  }, []);

  return (
    <>

      <Toast position="bottom-right" ref={toast} />
      <FileDialog
        fileType="icon"
        onHide={() => { setAddIcon(false); }}
        show={addIcon}
        showButton={false}
      />
      <DataSelector
        columns={targetColumns}
        dataSource={filteredStreams}
        emptyMessage="No Streams"
        headerLeftTemplate={leftHeaderTemplate}
        headerRightTemplate={rightHeaderTemplate}
        id={props.id + 'DataSelector'}
        isLoading={videoStreamsQuery.isLoading}
        leftColSize={1}
        name='Playlist Streams'
        onSelectionChange={
          (e) => onsetSelectedM3UStreams(e as StreamMasterApi.VideoStreamDto[])}
        onValueChanged={(e) => onValueChanged(e as StreamMasterApi.VideoStreamDto[])}
        rightColSize={4}
        selection={selectedM3UStreams}
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
