import { type CSSProperties } from "react";
import React from "react";
import * as StreamMasterApi from '../store/iptvApi';
import DataSelectorPicker from "../features/dataSelectorPicker/DataSelectorPicker";

import { Button } from 'primereact/button';
import { getTopToolOptions } from "../common/common";
import { Toast } from 'primereact/toast';
import * as Hub from "../store/signlar_functions";
import ChannelNumberEditor from "./ChannelNumberEditor";
import ChannelNameEditor from "./ChannelNameEditor";

import { type ColumnMeta } from "../features/dataSelector/DataSelectorTypes";
import { GroupIcon } from "../common/icons";
import { Tooltip } from "primereact/tooltip";
import IconSelector from "./IconSelector";
import EPGSelector from "./EPGSelector";

const PlayListDataSelectorPicker = (props: PlayListDataSelectorPickerProps) => {
  const toast = React.useRef<Toast>(null);

  const videoStreamsQuery = StreamMasterApi.useVideoStreamsGetVideoStreamsQuery();
  const [sourceVideoStreams, setSourceVideoStreams] = React.useState<StreamMasterApi.VideoStreamDto[]>([]);
  const [targetVideoStreams, setTargetVideoStreams] = React.useState<StreamMasterApi.ChildVideoStreamDto[]>([]);
  const [isVideoStreamUpdating, setIsVideoStreamUpdating] = React.useState<boolean>(false);
  const [streamGroup, setStreamGroup] = React.useState<StreamMasterApi.StreamGroupDto>({} as StreamMasterApi.StreamGroupDto);

  React.useMemo(() => {
    if (!props.streamGroup || props.streamGroup.id === undefined) {
      setStreamGroup({} as StreamMasterApi.StreamGroupDto);
      return;
    }

    setStreamGroup(props.streamGroup);

  }, [props.streamGroup])

  React.useMemo(() => {
    if (!videoStreamsQuery.data) {
      return;
    }

    if (props.videoStream?.childVideoStreams !== undefined) {
      const newStreams = [...props.videoStream.childVideoStreams];

      const dsIds = newStreams.map((sgvs) => sgvs.id);

      setTargetVideoStreams(newStreams.sort((a, b) => a.rank - b.rank)); // videoStreamsQuery.data.filter((m3u) => dsIds?.includes(m3u.id)));

      if (props.showTriState === null) {
        setSourceVideoStreams(videoStreamsQuery.data.filter((m3u) => !dsIds?.includes(m3u.id)));
      } else {
        setSourceVideoStreams(videoStreamsQuery.data.filter((m3u) => m3u.isHidden !== props.showTriState && !dsIds?.includes(m3u.id)));
      }

      return;
    }


    if (streamGroup === undefined || streamGroup.id === undefined || streamGroup.videoStreams === undefined) {
      const newData = [...videoStreamsQuery.data];

      if (props.showTriState === null) {
        setSourceVideoStreams(newData.sort((a, b) => a.user_Tvg_name.localeCompare(b.user_Tvg_name)));
      } else {
        setSourceVideoStreams(newData.filter((m3u) => m3u.isHidden !== props.showTriState).sort((a, b) => a.user_Tvg_name.localeCompare(b.user_Tvg_name)));
      }

      setTargetVideoStreams([]);
      return;
    }


    const ids = streamGroup.videoStreams.map((sgvs) => sgvs.id);
    const streams = videoStreamsQuery.data.filter((m3u) => ids?.includes(m3u.id));

    const roIds = streamGroup.videoStreams.filter((vs) => vs.isReadOnly === true).map((sgvs) => sgvs.id);

    const updatedStreams = streams.map((newStream) => {
      if (roIds.includes(newStream.id)) {
        return { ...newStream, isReadOnly: true };
      }

      return newStream;
    });

    if (props.isAdditionalChannels === true) {
      setTargetVideoStreams((updatedStreams as StreamMasterApi.ChildVideoStreamDto[]).sort((a, b) => a.rank - b.rank));
    } else {
      setTargetVideoStreams(updatedStreams as StreamMasterApi.ChildVideoStreamDto[]);
    }

    if (props.showTriState === null) {
      setSourceVideoStreams(videoStreamsQuery.data.filter((m3u) => !ids?.includes(m3u.id)));
    } else {
      setSourceVideoStreams(videoStreamsQuery.data.filter((m3u) => m3u.isHidden !== props.showTriState && !ids?.includes(m3u.id)));
    }

  }, [videoStreamsQuery.data, props.videoStream?.childVideoStreams, props.showTriState, props.isAdditionalChannels, streamGroup]);

  const channelNumberEditorBodyTemplate = React.useCallback((data: StreamMasterApi.VideoStreamDto) => {

    return (
      <ChannelNumberEditor
        data={data}
        enableEditMode
      />
    )
  }, []);


  const sourceColumns: ColumnMeta[] = [
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
      field: 'user_Tvg_name', header: 'Name', sortable: true,
      style: {
        flexGrow: 0,
        flexShrink: 1,
        maxWidth: '5rem',
      }
    }
    ,
    {
      field: 'user_Tvg_group', header: 'Group', sortable: true,
      style: {
        flexGrow: 0,
        flexShrink: 1,
        maxWidth: '5rem',
      }
    }
    ,
    {
      field: 'm3UFileId',
      fieldType: 'm3uFileName', header: 'File',
      sortable: true,
      style: {
        flexGrow: 0,
        flexShrink: 1,
        maxWidth: '2rem',
      }
    }
  ];

  const epgEditorBodyTemplate = React.useCallback((data: StreamMasterApi.VideoStreamDto) => {
    return (
      <EPGSelector
        data={data}
        enableEditMode
        value={data.user_Tvg_ID}
      />
    );
  }, []);

  const onUpdateVideoStream = React.useCallback(async (data: StreamMasterApi.VideoStreamDto, Logo: string) => {
    if (data.id < 0) {
      return;
    }


    const toSend = {} as StreamMasterApi.UpdateVideoStreamRequest;

    toSend.id = data.id;

    if (Logo && Logo !== '' && data.user_Tvg_logo !== Logo) {
      toSend.tvg_logo = Logo;
    }


    await Hub.UpdateVideoStream(toSend)
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

  }, []);

  const channelNameEditorBodyTemplate = React.useCallback((data: StreamMasterApi.VideoStreamDto) => {
    return (
      <ChannelNameEditor
        data={data}
        enableEditMode
      />
    )
  }, []);

  const logoEditorBodyTemplate = React.useCallback((data: StreamMasterApi.VideoStreamDto) => {
    // return <ChannelLogoEditor
    //   data={data}
    //   enableEditMode
    // />

    return (
      <IconSelector
        className="p-inputtext-sm"
        enableEditMode
        onChange={
          async (e: StreamMasterApi.IconFileDto) => {

            // const newiconSource = e.originalSource.includes('://')
            //   ? e.originalSource
            //   : e.name;

            await onUpdateVideoStream(data, e.name);
          }
        }
        onReset={
          async (e: StreamMasterApi.IconFileDto) => {

            // const newiconSource = e.originalSource.includes('://')
            //   ? e.originalSource
            //   : e.name;

            await onUpdateVideoStream(data, e.originalSource);
          }
        }
        resetValue={data.tvg_logo}
        value={data.user_Tvg_logo}
      />
    );

  }, [onUpdateVideoStream]);

  const onSave = React.useCallback(async (data: StreamMasterApi.VideoStreamDto[]) => {

    if (isVideoStreamUpdating || !props.streamGroup) {
      return;
    }

    setIsVideoStreamUpdating(true);

    var toSend = {} as StreamMasterApi.UpdateStreamGroupRequest;

    toSend.streamGroupId = props.streamGroup.id;

    toSend.videoStreamIds = data.map((stream) => {
      return stream.id;
    });


    await Hub.UpdateStreamGroup(toSend)
      .then((returnData) => {
        if (toast.current) {
          if (returnData) {
            toast.current.show({
              detail: `Channel Update Successful`,
              life: 3000,
              severity: 'success',
              summary: 'Successful',
            });
            // console.log('returnData', returnData.videoStreams.length);
            setStreamGroup(returnData);

          } else {
            toast.current.show({
              detail: `Channel Update Failed`,
              life: 3000,
              severity: 'error',
              summary: 'Error',
            });
          }
        }
      }).catch((e) => {
        if (toast.current) {
          toast.current.show({
            detail: `Channel Update Failed`,
            life: 3000,
            severity: 'error',
            summary: 'Error ' + e.message,
          });
        }
      });

    setIsVideoStreamUpdating(false);
  }, [isVideoStreamUpdating, props.streamGroup]);

  const onChange = React.useCallback(async (e: StreamMasterApi.ChildVideoStreamDto[]) => {
    if (props.isAdditionalChannels === true) {

      const newData = e.map((x: StreamMasterApi.ChildVideoStreamDto, index: number) => {
        return {
          ...x,
          rank: index,
        }
      }) as StreamMasterApi.ChildVideoStreamDto[];

      setTargetVideoStreams(newData.sort((a, b) => a.rank - b.rank));
    } else {
      setTargetVideoStreams(e as StreamMasterApi.ChildVideoStreamDto[]);
    }

    props?.onSelectionChange?.(e);
    await onSave(e);
  }, [onSave, props]);

  const onRemoveRank = React.useCallback(async (data: StreamMasterApi.VideoStreamDto) => {
    const newtargetVideoStreams = targetVideoStreams.filter((m3u) => m3u.id !== data.id);
    if (props.isAdditionalChannels === true) {
      setTargetVideoStreams(newtargetVideoStreams.sort((a, b) => a.rank - b.rank));
    } else {
      setTargetVideoStreams(newtargetVideoStreams);
    }

    props?.onSelectionChange?.(newtargetVideoStreams);
    await onSave(newtargetVideoStreams);

  }, [onSave, props, targetVideoStreams]);

  const sourceActionBodyTemplate = React.useCallback((data: StreamMasterApi.VideoStreamDto) => (
    <div className='flex min-w-full min-h-full justify-content-center align-items-center'>
      {data.isReadOnly === true &&
        <>
          <Tooltip target=".GroupIcon-class" />
          <div
            className="GroupIcon-class border-white"
            data-pr-at="right+5 top"

            data-pr-hidedelay={100}
            data-pr-my="left center-2"

            data-pr-position="left"
            data-pr-showdelay={500}
            data-pr-tooltip={`Group: ${data.user_Tvg_group}`}
          // style={{ minWidth: '10rem' }}
          >
            <GroupIcon />
          </div>
        </>
      }
      {data.isReadOnly !== true &&
        <Button
          className="p-button-danger"
          icon="pi pi-times"
          onClick={async () => await onRemoveRank(data)}
          rounded
          text
          tooltip="Remove"
          tooltipOptions={getTopToolOptions} />
      }
    </div>
  ), [onRemoveRank]);


  const targetColumns: ColumnMeta[] = [
    {
      field: 'id',
      filter: false,
      header: 'id',

      style: {
        maxWidth: '4rem',
        width: '4rem',
      } as CSSProperties,
    },
    {
      bodyTemplate: channelNumberEditorBodyTemplate,
      field: 'user_Tvg_chno',
      filter: true,
      header: 'Ch.',
      isHidden: props.isAdditionalChannels === true,

      style: {
        maxWidth: '4rem',
        width: '4rem',
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
      field:
        'user_Tvg_name',
      header: 'Name',

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
      bodyTemplate: sourceActionBodyTemplate,
      field: 'x',
      header: '',
      style: {
        maxWidth: '2rem',
        width: '2rem',
      } as CSSProperties,
    },
  ];


  return (
    <>
      <Toast position="bottom-right" ref={toast} />
      <DataSelectorPicker
        id={props.id + '-dataselectorpicker'}
        isLoading={videoStreamsQuery.isLoading}
        onSelectionChange={onChange}
        onTargetOnValueChanged={(e) => {
          if (props.isAdditionalChannels === true) {
            const d = e as StreamMasterApi.VideoStreamDto[];
            const newData = d.map((x: StreamMasterApi.VideoStreamDto, index: number) => {
              return {
                ...x,
                rank: index,
              }
            }) as StreamMasterApi.ChildVideoStreamDto[];
            props.onValueChanged?.(newData);
          } else {
            props.onValueChanged?.(e as StreamMasterApi.ChildVideoStreamDto[]);
          }
        }}
        onTargetSelectionChange={onChange}
        selection={targetVideoStreams}
        showUndo
        sourceColumns={sourceColumns}
        sourceDataSource={sourceVideoStreams}
        sourceEnableState={props.enableState}
        sourceHeaderTemplate={props.sourceHeaderTemplate}
        sourceName='Streams'
        sourceRightColSize={1}
        sourceSortField='user_Tvg_name'
        sourceStyle={{
          height: props.maxHeight !== null ? props.maxHeight : 'calc(100vh - 40px)',
        }}
        targetColumns={targetColumns}
        targetDataSource={targetVideoStreams}
        targetEnableState={props.enableState}
        targetName='Selected'
        targetReorderable={props.isAdditionalChannels}
        targetRightColSize={3}
        targetSortField={props.isAdditionalChannels === true ? '' : 'user_Tvg_chno'}
      />
    </>
  );
};

PlayListDataSelectorPicker.displayName = 'PlayList Editor';
PlayListDataSelectorPicker.defaultProps = {
  enableState: true,
  isAdditionalChannels: false,
  maxHeight: null,
  showTriState: true
};

export type PlayListDataSelectorPickerProps = {
  enableState?: boolean;
  id: string;
  isAdditionalChannels?: boolean;
  maxHeight?: number;
  onSelectionChange?: (value: StreamMasterApi.ChildVideoStreamDto[]) => void;
  onValueChanged?: (value: StreamMasterApi.ChildVideoStreamDto[]) => void;
  showTriState?: boolean | null | undefined;
  sourceHeaderTemplate?: React.ReactNode | undefined;
  streamGroup?: StreamMasterApi.StreamGroupDto | undefined;
  videoStream?: StreamMasterApi.VideoStreamDto | undefined;
};

export default React.memo(PlayListDataSelectorPicker);
