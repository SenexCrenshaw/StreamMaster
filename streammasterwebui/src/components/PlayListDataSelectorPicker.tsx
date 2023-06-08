import { type CSSProperties } from "react";
import React from "react";
import * as StreamMasterApi from '../store/iptvApi';
import DataSelectorPicker from "../features/dataSelectorPicker/DataSelectorPicker";

import { Button } from 'primereact/button';
import { getTopToolOptions } from "../common/common";
import StreamMasterSetting from "../store/signlar/StreamMasterSetting";
import { Toast } from 'primereact/toast';
import * as Hub from "../store/signlar_functions";
import ChannelNumberEditor from "./ChannelNumberEditor";
import ChannelNameEditor from "./ChannelNameEditor";
import ChannelLogoEditor from "./ChannelLogoEditor";
import { type ColumnMeta } from "../features/dataSelector/DataSelectorTypes";
import { GroupIcon } from "../common/icons";
import { Tooltip } from "primereact/tooltip";

const PlayListDataSelectorPicker = (props: PlayListDataSelectorPickerProps) => {
  const toast = React.useRef<Toast>(null);

  const videoStreamsQuery = StreamMasterApi.useVideoStreamsGetVideoStreamsQuery();
  const [sourceVideoStreams, setSourceVideoStreams] = React.useState<StreamMasterApi.VideoStreamDto[]>([]);
  const [targetVideoStreams, setTargetVideoStreams] = React.useState<StreamMasterApi.VideoStreamDto[]>([]);
  const [isVideoStreamUpdating, setIsVideoStreamUpdating] = React.useState<boolean>(false);
  const [streamGroup, setStreamGroup] = React.useState<StreamMasterApi.StreamGroupDto>({} as StreamMasterApi.StreamGroupDto);
  const setting = StreamMasterSetting();

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
      const dsIds = props.videoStream?.childVideoStreams.map((sgvs) => sgvs.id);
      setTargetVideoStreams(videoStreamsQuery.data.filter((m3u) => dsIds?.includes(m3u.id)));

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


    setTargetVideoStreams(updatedStreams);


    if (props.showTriState === null) {
      setSourceVideoStreams(videoStreamsQuery.data.filter((m3u) => !ids?.includes(m3u.id)));
    } else {
      setSourceVideoStreams(videoStreamsQuery.data.filter((m3u) => m3u.isHidden !== props.showTriState && !ids?.includes(m3u.id)));
    }

  }, [videoStreamsQuery.data, props.videoStream?.childVideoStreams, props.showTriState, streamGroup]);

  const sourceColumns: ColumnMeta[] = [
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

  const channelNumberEditorBodyTemplate = React.useCallback((data: StreamMasterApi.VideoStreamDto) => {

    return (
      <ChannelNumberEditor
        data={data}
        enableEditMode
      />
    )
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
    return <ChannelLogoEditor
      data={data}
      enableEditMode
    />

  }, []);


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

  const onChange = React.useCallback(async (e: StreamMasterApi.VideoStreamDto[]) => {
    setTargetVideoStreams(e);
    props?.onSelectionChange?.(e);
    await onSave(e);
  }, [onSave, props]);

  const onRemoveRank = React.useCallback(async (data: StreamMasterApi.VideoStreamDto) => {
    const newtargetVideoStreams = targetVideoStreams.filter((m3u) => m3u.id !== data.id);
    setTargetVideoStreams(newtargetVideoStreams);
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
      bodyTemplate: channelNumberEditorBodyTemplate,
      field: 'user_Tvg_chno',
      filter: true,
      header: 'Ch.',
      isHidden: props.isAdditionalChannels === true,
      sortable: true,
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
      sortable: true,
    },
    // {
    //   bodyTemplate: epgEditorBodyTemplate,
    //   field: 'user_Tvg_ID',
    //   fieldType: 'epg',
    //   filter: true,
    //   sortable: true,
    //   style: {
    //     flexGrow: 0,
    //     flexShrink: 1,
    //     maxWidth: '13rem',
    //   } as CSSProperties,
    // },
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

  const headerPrefixTemplate = React.useMemo(() => {
    const record = 'hello';// props.selectedIptvChannel?.tvg_logo;
    return (
      <div className="flex flex-nowrap justify-content-center align-items-center p-0 h-full">
        <img
          alt={record ?? 'Logo'}
          className="max-h-full max-w-3rem p-0"
          onError={(e: React.SyntheticEvent<HTMLImageElement, Event>) => (e.currentTarget.src = (e.currentTarget.src = setting.defaultIcon))}
          src={`${encodeURI(record ?? '')}`}
          style={{
            objectFit: 'contain',
          }}
        />
      </div>
    );
  }, [setting.defaultIcon]);

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
            }) as StreamMasterApi.VideoStreamDto[];
            props.onValueChanged?.(newData as StreamMasterApi.VideoStreamDto[]);
          } else {
            props.onValueChanged?.(e as StreamMasterApi.VideoStreamDto[]);
          }
        }}
        // onTargetSelectionChange={(e) => {
        // }}
        selection={targetVideoStreams}
        sourceColumns={sourceColumns}
        sourceDataSource={sourceVideoStreams}
        sourceEnableState={false}
        sourceHeaderPrefixTemplate={headerPrefixTemplate}
        sourceHeaderTemplate={props.sourceHeaderTemplate}
        sourceName='Streams'
        sourceRightColSize={1}
        sourceSortField='user_Tvg_name'
        sourceStyle={{
          height: props.maxHeight !== null ? props.maxHeight : 'calc(100vh - 40px)',
        }}
        targetColumns={targetColumns}
        targetDataSource={targetVideoStreams}
        targetEnableState={false}
        targetName='Selected'
        targetReorderable={props.isAdditionalChannels}
        targetRightColSize={3}
        targetSortField={props.isAdditionalChannels === true ? 'rank' : 'user_Tvg_chno'}
      />
    </>
  );
};

PlayListDataSelectorPicker.displayName = 'PlayList Editor';
PlayListDataSelectorPicker.defaultProps = {
  isAdditionalChannels: false,
  maxHeight: null,
  showTriState: true
};

export type PlayListDataSelectorPickerProps = {
  id: string;
  isAdditionalChannels?: boolean;
  maxHeight?: number;
  onSelectionChange?: (value: StreamMasterApi.VideoStreamDto[]) => void;
  onValueChanged?: (value: StreamMasterApi.VideoStreamDto[]) => void;
  showTriState?: boolean | null | undefined;
  sourceHeaderTemplate?: React.ReactNode | undefined;
  streamGroup?: StreamMasterApi.StreamGroupDto | undefined;
  videoStream?: StreamMasterApi.VideoStreamDto | undefined;
};

export default React.memo(PlayListDataSelectorPicker);
