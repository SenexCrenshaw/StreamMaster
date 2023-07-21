/* eslint-disable react/no-unused-prop-types */
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

  const [sourceVideoStreams, setSourceVideoStreams] = React.useState<StreamMasterApi.VideoStreamDto[] | undefined>(undefined);
  const [targetVideoStreams, setTargetVideoStreams] = React.useState<StreamMasterApi.ChildVideoStreamDto[] | undefined>(undefined);
  const [isVideoStreamUpdating, setIsVideoStreamUpdating] = React.useState<boolean>(false);
  const [streamGroup, setStreamGroup] = React.useState<StreamMasterApi.StreamGroupDto | undefined>(undefined);

  React.useEffect(() => {
    if (!props.streamGroup || props.streamGroup.id === undefined) {
      setStreamGroup(undefined);
      return;
    }

    setStreamGroup(props.streamGroup);
    return () => setStreamGroup(undefined);

  }, [props.streamGroup])

  React.useEffect(() => {
    if (!videoStreamsQuery.data) {
      return;
    }

    if (props.videoStream?.childVideoStreams !== undefined && props.videoStream.childVideoStreams.length > 0) {
      const newStreams = [...props.videoStream.childVideoStreams];

      const dsIds = newStreams.map((sgvs) => sgvs.id);

      setTargetVideoStreams(newStreams.sort((a, b) => a.rank - b.rank));

      if (props.showTriState === null) {
        setSourceVideoStreams(videoStreamsQuery.data.filter((m3u) => !dsIds?.includes(m3u.id)));
      } else {
        setSourceVideoStreams(videoStreamsQuery.data.filter((m3u) => m3u.isHidden !== props.showTriState && !dsIds?.includes(m3u.id)));
      }

      return;
    }


    if (streamGroup === undefined || streamGroup.id === undefined || streamGroup.childVideoStreams === undefined) {
      const newData = [...videoStreamsQuery.data];

      if (props.showTriState === null) {
        setSourceVideoStreams(newData.sort((a, b) => a.user_Tvg_name.localeCompare(b.user_Tvg_name)));
      } else {
        setSourceVideoStreams(newData.filter((m3u) => m3u.isHidden !== props.showTriState).sort((a, b) => a.user_Tvg_name.localeCompare(b.user_Tvg_name)));
      }

      setTargetVideoStreams(undefined);
      return;
    }


    const ids = streamGroup.childVideoStreams.map((sgvs) => sgvs.id);
    const streams = videoStreamsQuery.data.filter((m3u) => ids?.includes(m3u.id));

    const roIds = streamGroup.childVideoStreams.filter((vs) => vs.isReadOnly === true).map((sgvs) => sgvs.id);

    const updatedStreams = streams.map((newStream) => {
      if (roIds.includes(newStream.id)) {
        return { ...newStream, isReadOnly: true };
      }

      return newStream;
    });

    if (props.isAdditionalChannels === true) {
      setTargetVideoStreams((updatedStreams as StreamMasterApi.ChildVideoStreamDto[]).filter((m3u) => props.showHidden === true || m3u.isHidden !== true).sort((a, b) => a.rank - b.rank));
    } else {

      setTargetVideoStreams(
        (updatedStreams as StreamMasterApi.ChildVideoStreamDto[]).filter((m3u) => props.showHidden === true || m3u.isHidden !== true)
      );
    }

    if (props.showTriState === null) {
      setSourceVideoStreams(videoStreamsQuery.data.filter((m3u) => !ids?.includes(m3u.id)));
    } else {
      setSourceVideoStreams(videoStreamsQuery.data.filter((m3u) => m3u.isHidden !== props.showTriState && !ids?.includes(m3u.id)));
    }

    return () => {
      setSourceVideoStreams(undefined);
      setTargetVideoStreams(undefined);
    }
  }, [props.isAdditionalChannels, props.showHidden, props.showTriState, props.videoStream, streamGroup, videoStreamsQuery.data]);

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
      bodyTemplate: channelNumberEditorBodyTemplate,
      field: 'user_Tvg_chno',
      filter: false,
      header: 'Ch.',
      sortable: true,
      style: {
        maxWidth: '3rem',
        width: '3rem',
      } as CSSProperties,
    },
    {
      field: 'user_Tvg_name', header: 'Name', sortable: true,
      style: {
        flexGrow: 1,
        flexShrink: 1
      }
    }
    ,
    {
      field: 'user_Tvg_group', header: 'Group', sortable: true,

    }
    ,
    {
      field: 'm3UFileId',
      fieldType: 'm3uFileName', header: 'File',
      sortable: true,

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
    if (data.id === '') {
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
    return (
      <IconSelector
        className="p-inputtext-sm"
        enableEditMode
        onChange={
          async (e: StreamMasterApi.IconFileDto) => {
            await onUpdateVideoStream(data, e.name);
          }
        }
        onReset={
          async (e: StreamMasterApi.IconFileDto) => {
            await onUpdateVideoStream(data, e.originalSource);
          }
        }
        resetValue={data.tvg_logo}
        value={data.user_Tvg_logo}
      />
    );

  }, [onUpdateVideoStream]);

  const onEdit = React.useCallback(async (data: StreamMasterApi.VideoStreamDto[]) => {
    if (data === null || data === undefined || !props.videoStream) {

      return;
    }

    const toSend = {} as StreamMasterApi.UpdateVideoStreamRequest;

    toSend.id = props.videoStream.id;

    const newData = data.map((x: StreamMasterApi.VideoStreamDto, index: number) => { return { ...x, rank: index, } }) as StreamMasterApi.ChildVideoStreamDto[];

    toSend.childVideoStreams = newData;

    Hub.UpdateVideoStream(toSend)
      .then((resultData) => {
        if (toast.current) {
          if (resultData) {
            toast.current.show({
              detail: `Video Stream Update Successful`,
              life: 3000,
              severity: 'success',
              summary: 'Successful',
            });

          } else {
            toast.current.show({
              detail: `Video Stream Update Failed`,
              life: 3000,
              severity: 'error',
              summary: 'Error',
            });
          }
        }
      }
      ).catch((error) => {
        if (toast.current) {
          toast.current.show({
            detail: `Channel Update Failed`,
            life: 3000,
            severity: 'error',
            summary: 'Error ' + error.message,
          });
        }
      });

  }, [props.videoStream]);


  const onSave = React.useCallback(async (data: StreamMasterApi.VideoStreamDto[]) => {

    if (props.isAdditionalChannels == true) {
      await onEdit(data);
      return;
    }

    if (isVideoStreamUpdating || data.length === 0 || data[0].id === undefined) {
      return;
    }

    setIsVideoStreamUpdating(true);

    var toSend = {} as StreamMasterApi.UpdateStreamGroupRequest;

    if (props.streamGroup)
      toSend.streamGroupId = props.streamGroup.id;

    toSend.videoStreams = data.map((stream) => {
      return { isReadOnly: stream.isReadOnly, videoStreamId: stream.id } as StreamMasterApi.VideoStreamIsReadOnly;
    });

    await Hub.UpdateStreamGroup(toSend)
      .then((returnData) => {
        if (toast.current) {
          if (returnData) {
            toast.current.show({
              detail: `Stream Group Update Successful`,
              life: 3000,
              severity: 'success',
              summary: 'Successful',
            });

            setStreamGroup(returnData);

          } else {
            toast.current.show({
              detail: `Stream Group Update Failed`,
              life: 3000,
              severity: 'error',
              summary: 'Error',
            });
          }
        }
      }).catch((e) => {
        if (toast.current) {
          toast.current.show({
            detail: `Stream Group Update Failed`,
            life: 3000,
            severity: 'error',
            summary: 'Error ' + e.message,
          });
        }
      });

    setIsVideoStreamUpdating(false);
  }, [isVideoStreamUpdating, onEdit, props.isAdditionalChannels, props.streamGroup]);

  const onChange = React.useCallback(async (e: StreamMasterApi.ChildVideoStreamDto[]) => {
    if (props.isAdditionalChannels === true) {

      const newData = e.map((x: StreamMasterApi.ChildVideoStreamDto, index: number) => { return { ...x, rank: index, } }) as StreamMasterApi.ChildVideoStreamDto[];

      setTargetVideoStreams(newData.sort((a, b) => a.rank - b.rank));
    } else {
      setTargetVideoStreams(e as StreamMasterApi.ChildVideoStreamDto[]);
    }

    props?.onSelectionChange?.(e);
    await onSave(e);
  }, [onSave, props]);

  const onRemoveRank = React.useCallback(async (data: StreamMasterApi.VideoStreamDto) => {
    if (targetVideoStreams === undefined) {
      return;
    }

    const newtargetVideoStreams = targetVideoStreams.filter((m3u) => m3u.id !== data.id);
    if (props.isAdditionalChannels === true) {
      setTargetVideoStreams(newtargetVideoStreams.sort((a, b) => a.rank - b.rank));
    } else {
      setTargetVideoStreams(newtargetVideoStreams);
    }

    props?.onSelectionChange?.(newtargetVideoStreams);
    await onSave(newtargetVideoStreams);

  }, [onSave, props, targetVideoStreams]);

  const sourceActionBodyTemplate = React.useCallback((data: StreamMasterApi.VideoStreamDto) => {

    if (data.isReadOnly === true) {
      return (
        <div className='flex min-w-full min-h-full justify-content-end align-items-center'>
          <Tooltip target=".GroupIcon-class" />
          <div
            className="GroupIcon-class border-white"
            data-pr-at="right+5 top"

            data-pr-hidedelay={100}
            data-pr-my="left center-2"

            data-pr-position="left"
            data-pr-showdelay={500}
            // data-pr-tooltip={`Group: ${data.user_Tvg_group}`}
            data-pr-tooltip='From Group'
          // style={{ minWidth: '10rem' }}
          >
            <GroupIcon />
          </div>
        </div>
      );
    }

    return (
      <Button
        className="p-button-danger"
        icon="pi pi-times"
        onClick={async () => await onRemoveRank(data)}
        rounded
        text
        tooltip="Remove"
        tooltipOptions={getTopToolOptions} />
    );
  }, [onRemoveRank]);


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
      field: 'user_Tvg_name',
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
        onTargetOnValueChanged={async (e) => {
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
        onTargetSelectionChange={async (e) => {
          if (props.isAdditionalChannels === true) {
            const d = e as StreamMasterApi.VideoStreamDto[];
            const newData = d.map((x: StreamMasterApi.VideoStreamDto, index: number) => {
              return {
                ...x,
                rank: index,
              }
            }) as StreamMasterApi.ChildVideoStreamDto[];
            await onEdit(newData);
          }
        }
        }
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
        targetSortField={props.isAdditionalChannels === true ? 'rank' : 'user_Tvg_chno'}
      />
    </>
  );
};

PlayListDataSelectorPicker.displayName = 'PlayList Editor';
PlayListDataSelectorPicker.defaultProps = {
  enableState: true,
  isAdditionalChannels: false,
  maxHeight: null,
  showHidden: true,
  showTriState: true
};

export type PlayListDataSelectorPickerProps = {
  enableState?: boolean;
  id: string;
  isAdditionalChannels?: boolean;
  maxHeight?: number;
  onSelectionChange?: (value: StreamMasterApi.ChildVideoStreamDto[]) => void;
  onValueChanged?: (value: StreamMasterApi.ChildVideoStreamDto[]) => void;
  showHidden?: boolean | undefined;
  showTriState?: boolean | null | undefined;
  sourceHeaderTemplate?: React.ReactNode | undefined;
  streamGroup?: StreamMasterApi.StreamGroupDto | undefined;
  videoStream?: StreamMasterApi.VideoStreamDto | undefined;
};

export default React.memo(PlayListDataSelectorPicker);
