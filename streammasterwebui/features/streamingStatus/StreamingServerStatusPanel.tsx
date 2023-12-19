import BookButton from '@components/buttons/BookButton';
import DataSelector from '@components/dataSelector/DataSelector';
import { ColumnMeta } from '@components/dataSelector/DataSelectorTypes';
import { VideoStreamSelector } from '@components/videoStream/VideoStreamSelector';
import { formatJSONDateString, getIconUrl, getTopToolOptions } from '@lib/common/common';
import {
  ChangeVideoStreamChannelRequest,
  SimulateStreamFailureRequest,
  StreamStatisticsResult,
  VideoInfo,
  useVideoStreamsGetAllStatisticsForAllUrlsQuery
} from '@lib/iptvApi';
import { GetVideoStreamInfoFromId } from '@lib/smAPI/VideoStreams/VideoStreamsGetAPI';
import { ChangeVideoStreamChannel, SimulateStreamFailure } from '@lib/smAPI/VideoStreams/VideoStreamsMutateAPI';
import useSettings from '@lib/useSettings';
import { Button } from 'primereact/button';
import { Dialog } from 'primereact/dialog';
import { Toast } from 'primereact/toast';
import { memo, useCallback, useMemo, useRef, type CSSProperties, useState } from 'react';
import { VideoInfoDisplay } from './VideoInfoDisplay';

interface StreamingServerStatusPanelProperties {
  readonly className?: string;
  readonly style?: CSSProperties;
}

export const StreamingServerStatusPanel = ({ className, style }: StreamingServerStatusPanelProperties) => {
  const setting = useSettings();
  const toast = useRef<Toast>(null);
  const [videoInfo, setVideoInfo] = useState<VideoInfo | undefined>(undefined);
  const [channelName, setChannelName] = useState<string>('');
  const getStreamingStatus = useVideoStreamsGetAllStatisticsForAllUrlsQuery();

  const onChangeVideoStreamChannel = useCallback(async (playingVideoStreamId: string, newVideoStreamId: string) => {
    if (playingVideoStreamId === undefined || playingVideoStreamId === '' || newVideoStreamId === undefined || newVideoStreamId === '') {
      return;
    }

    const toSend = {} as ChangeVideoStreamChannelRequest;

    toSend.playingVideoStreamId = playingVideoStreamId;
    toSend.newVideoStreamId = newVideoStreamId;

    await ChangeVideoStreamChannel(toSend)
      .then(() => {
        if (toast.current) {
          toast.current.show({
            detail: 'Changed Client Channel',
            life: 3000,
            severity: 'success',
            summary: 'Successful'
          });
        }
      })
      .catch(() => {
        if (toast.current) {
          toast.current.show({
            detail: 'Failed Client Channel',
            life: 3000,
            severity: 'error',
            summary: 'Error'
          });
        }
      });
  }, []);

  const videoStreamTemplate = useCallback(
    (rowData: StreamStatisticsResult) => (
      <VideoStreamSelector
        onChange={async (e) => {
          await onChangeVideoStreamChannel(rowData.videoStreamId ?? '', e.id);
        }}
        value={rowData.videoStreamName}
      />
    ),
    [onChangeVideoStreamChannel]
  );

  const onFailStream = useCallback(async (rowData: StreamStatisticsResult) => {
    if (!rowData.streamUrl || rowData.streamUrl === undefined || rowData.streamUrl === '') {
      return;
    }

    const toSend = {} as SimulateStreamFailureRequest;
    toSend.streamUrl = rowData.streamUrl;

    await SimulateStreamFailure(toSend)
      .then(() => {
        if (toast.current) {
          toast.current.show({
            detail: 'Next Stream',
            life: 3000,
            severity: 'success',
            summary: 'Successful'
          });
        }
      })
      .catch(() => {
        if (toast.current) {
          toast.current.show({
            detail: 'Next Stream Failed',
            life: 3000,
            severity: 'error',
            summary: 'Error'
          });
        }
      });
  }, []);

  const imageBodyTemplate = useCallback(
    (rowData: StreamStatisticsResult) => {
      const iconUrl = getIconUrl(rowData.logo, setting.defaultIcon, setting.cacheIcon);

      return (
        <div className="flex align-content-center flex-wrap">
          <img alt={rowData.logo ?? 'logo'} className="flex align-items-center justify-content-center max-w-full max-h-2rem h-2rem" src={iconUrl} />
        </div>
      );
    },
    [setting]
  );

  const inputBitsPerSecondTemplate = useCallback((rowData: StreamStatisticsResult) => {
    if (rowData.inputBitsPerSecond === undefined) return <div>0</div>;

    const kbps = rowData.inputBitsPerSecond / 1000;
    const roundedKbps = Math.ceil(kbps);

    return <div>{roundedKbps.toLocaleString('en-US')}</div>;
  }, []);

  const inputElapsedTimeTemplate = useCallback((rowData: StreamStatisticsResult) => <div>{rowData.inputElapsedTime?.split('.')[0]}</div>, []);

  const inputStartTimeTemplate = useCallback((rowData: StreamStatisticsResult) => <div>{formatJSONDateString(rowData.inputStartTime ?? '')}</div>, []);

  const dataSource = useMemo((): StreamStatisticsResult[] => {
    if (getStreamingStatus.data === undefined || getStreamingStatus.data.length === 0 || getStreamingStatus.data === null) {
      return [];
    }

    const data = [] as StreamStatisticsResult[];

    for (const item of getStreamingStatus.data) {
      if (data.findIndex((x) => x.videoStreamId === item.videoStreamId) === -1) {
        data.push(item);
      }
    }

    return data;
  }, [getStreamingStatus.data]);

  const streamCount = useCallback(
    (rowData: StreamStatisticsResult) => {
      if (getStreamingStatus.data === undefined || getStreamingStatus.data === null) {
        return <div>0</div>;
      }
      return <div>{getStreamingStatus.data.filter((x) => x.videoStreamId === rowData.videoStreamId).length}</div>;
    },
    [getStreamingStatus.data]
  );

  const onPreview = useCallback(async (rowData: StreamStatisticsResult) => {
    setChannelName(rowData.videoStreamName ?? '');
    await GetVideoStreamInfoFromId(rowData.channelId ?? '').then((data) => {
      if (data === null || data === undefined) return;

      // console.log(data);
      setVideoInfo(data);
    });
  }, []);

  const targetActionBodyTemplate = useCallback(
    (rowData: StreamStatisticsResult) => (
      <div className="dataselector p-inputgroup align-items-center justify-content-end">
        <BookButton iconFilled={false} onClick={(e) => onPreview(rowData)} />
        <Button
          // className="p-button-danger"
          icon="pi pi-angle-right"
          onClick={async () => await onFailStream(rowData)}
          rounded
          text
          tooltip="Next Stream"
          tooltipOptions={getTopToolOptions}
        />
      </div>
    ),
    [onFailStream, onPreview]
  );

  const sourceColumns = useMemo(
    (): ColumnMeta[] => [
      {
        bodyTemplate: imageBodyTemplate,
        field: 'icon',
        width: '4rem'
      },

      { field: 'channelName', header: 'Channel' },
      {
        align: 'center',
        bodyTemplate: videoStreamTemplate,
        field: 'videoStreamTemplate',
        header: 'Video Stream',
        width: '18rem'
      },
      {
        align: 'center',
        field: 'rank',
        header: 'Rank',
        width: '4rem'
      },

      {
        align: 'center',
        bodyTemplate: streamCount,
        field: 'Count',
        header: 'Streams',
        width: '4rem'
      },
      {
        align: 'center',
        bodyTemplate: inputBitsPerSecondTemplate,
        field: 'inputBitsPerSecond',
        header: 'Input kbps',
        width: '10rem'
      },
      {
        align: 'center',
        bodyTemplate: inputElapsedTimeTemplate,
        field: 'inputElapsedTime',
        header: 'Input Elapsed',
        width: '10rem'
      },
      {
        align: 'center',
        bodyTemplate: inputStartTimeTemplate,
        field: 'inputStartTime',
        header: 'Input Start',
        width: '10rem'
      },
      {
        align: 'center',
        bodyTemplate: targetActionBodyTemplate,
        field: 'Actions',
        width: '8rem'
      }
    ],
    [
      imageBodyTemplate,
      inputBitsPerSecondTemplate,
      inputElapsedTimeTemplate,
      inputStartTimeTemplate,
      streamCount,
      targetActionBodyTemplate,
      videoStreamTemplate
    ]
  );

  const onHide = useCallback(() => {
    setVideoInfo(undefined);
  }, []);

  return (
    <>
      <Toast position="bottom-right" ref={toast} />
      <Dialog className={`col-8 p-0`} closable header={'ffprobe: ' + channelName} maximizable modal onHide={onHide} visible={videoInfo !== undefined}>
        {videoInfo && <VideoInfoDisplay videoInfo={videoInfo} />}
      </Dialog>
      <div className="m3uFilesEditor flex flex-column col-12 flex-shrink-0 ">
        <DataSelector
          className={className}
          columns={sourceColumns}
          dataSource={dataSource}
          defaultSortField="videoStreamName"
          emptyMessage="No Streams"
          id="StreamingServerStatusPanel"
          isLoading={getStreamingStatus.isLoading}
          key="m3UStreamId"
          selectedItemsKey="selectSelectedItems"
          style={style}
        />
      </div>
    </>
  );
};

StreamingServerStatusPanel.displayName = 'Streaming Server Status';

export default memo(StreamingServerStatusPanel);
