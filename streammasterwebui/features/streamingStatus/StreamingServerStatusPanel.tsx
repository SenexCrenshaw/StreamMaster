import BookButton from '@components/buttons/BookButton';
import DataSelector from '@components/dataSelector/DataSelector';
import { ColumnMeta } from '@components/dataSelector/DataSelectorTypes';
import { VideoStreamSelector } from '@components/videoStream/VideoStreamSelector';
import { formatJSONDateString, getIconUrl, getTopToolOptions } from '@lib/common/common';
import {
  ChangeVideoStreamChannelRequest,
  InputStreamingStatistics,
  SimulateStreamFailureRequest,
  VideoInfo,
  useStatisticsGetInputStatisticsQuery
} from '@lib/iptvApi';
import { GetVideoStreamInfoFromId } from '@lib/smAPI/VideoStreams/VideoStreamsGetAPI';
import { ChangeVideoStreamChannel, SimulateStreamFailure } from '@lib/smAPI/VideoStreams/VideoStreamsMutateAPI';
import useSettings from '@lib/useSettings';
import { Button } from 'primereact/button';
import { Dialog } from 'primereact/dialog';
import { Toast } from 'primereact/toast';
import { memo, useCallback, useEffect, useMemo, useRef, useState, type CSSProperties } from 'react';
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

  const getStreamingStatus = useStatisticsGetInputStatisticsQuery(undefined, { pollingInterval: 1000 * 1 });
  const [dataSource, setDataSource] = useState<InputStreamingStatistics[]>([]);
  const [data, setData] = useState<InputStreamingStatistics[]>([]);

  useEffect(() => {
    if (getStreamingStatus.data === undefined || getStreamingStatus.data.length === 0 || getStreamingStatus.data === null) {
      if (dataSource !== undefined && dataSource.length > 0) {
        setDataSource([]);
      }
      if (data !== undefined && data.length > 0) {
        setData([]);
      }

      return;
    }

    let dataDS = [...dataSource];

    for (const item of getStreamingStatus.data) {
      const index = dataDS.findIndex((x) => x.id === item.id);
      if (index === -1) {
        dataDS.push(item);
      } else {
        dataDS[index] = {
          ...dataDS[index],
          logo: item.logo !== dataDS[index].logo ? item.logo : dataDS[index].logo,
          channelName: item.channelName !== dataDS[index].channelName ? item.channelName : dataDS[index].channelName,
          rank: item.rank !== dataDS[index].rank ? item.rank : dataDS[index].rank,
          elapsedTime: item.elapsedTime !== dataDS[index].elapsedTime ? item.elapsedTime : dataDS[index].elapsedTime,
          bitsPerSecond: item.bitsPerSecond !== dataDS[index].bitsPerSecond ? item.bitsPerSecond : dataDS[index].bitsPerSecond,
          clients: item.clients !== dataDS[index].clients ? item.clients : dataDS[index].clients
        };
      }
    }

    for (const item of dataSource) {
      const index = getStreamingStatus.data.findIndex((x) => x.id === item.id);
      if (index === -1) {
        dataDS = dataDS.filter((x) => x.id !== item.id);
      }
    }
    setData(getStreamingStatus.data);
    setDataSource(dataDS);
  }, [getStreamingStatus.data]);

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
    (rowData: InputStreamingStatistics) => (
      <VideoStreamSelector
        onChange={async (e) => {
          await onChangeVideoStreamChannel(rowData.id ?? '', e.id);
        }}
        value={rowData.channelName}
      />
    ),
    [onChangeVideoStreamChannel]
  );

  const onFailStream = useCallback(async (rowData: InputStreamingStatistics) => {
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
    (rowData: InputStreamingStatistics) => {
      const iconUrl = getIconUrl(rowData.logo, setting.defaultIcon, setting.cacheIcon);

      return (
        <div className="flex align-content-center flex-wrap">
          <img
            loading="lazy"
            alt={rowData.logo ?? 'logo'}
            className="flex align-items-center justify-content-center max-w-full max-h-2rem h-2rem"
            src={iconUrl}
          />
        </div>
      );
    },
    [setting]
  );

  const inputBitsPerSecondTemplate = useCallback((rowData: InputStreamingStatistics) => {
    if (rowData.bitsPerSecond === undefined) return <div>0</div>;

    const kbps = rowData.bitsPerSecond / 1000;
    const roundedKbps = Math.ceil(kbps);

    return <div>{roundedKbps.toLocaleString('en-US')}</div>;
  }, []);

  const inputStartTimeTemplate = useCallback((rowData: InputStreamingStatistics) => <div>{formatJSONDateString(rowData.startTime ?? '')}</div>, []);

  // const streamCount = useCallback(
  //   (rowData: InputStreamingStatistics) => {
  //     if (data === undefined || data === null) {
  //       return <div>0</div>;
  //     }

  //     return <div>{data.filter((x) => x.id === rowData.id).length}</div>;
  //   },
  //   [data]
  // );

  const onPreview = useCallback(async (rowData: InputStreamingStatistics) => {
    if (rowData.channelName !== undefined && rowData.channelName === '') {
      setChannelName(rowData.channelName);
    }

    if (rowData.channelId === undefined || rowData.channelId === '') {
      return;
    }
    await GetVideoStreamInfoFromId(rowData.channelId).then((data) => {
      if (data === null || data === undefined) return;

      // console.log(data);
      setVideoInfo(data);
    });
  }, []);

  const targetActionBodyTemplate = useCallback(
    (rowData: InputStreamingStatistics) => (
      <div className="dataselector p-inputgroup align-items-center justify-content-end">
        <BookButton iconFilled={false} onClick={(e) => onPreview(rowData)} tooltip="ffprobe" />
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

  const columns = useMemo(
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
        // bodyTemplate: streamCount,
        field: 'clients',
        header: 'Streams',
        width: '4rem'
      },
      {
        align: 'center',
        bodyTemplate: inputBitsPerSecondTemplate,
        field: 'bitsPerSecond',
        header: 'Input kbps',
        width: '10rem'
      },
      {
        align: 'center',
        field: 'elapsedTime',
        header: 'Input Elapsed',
        width: '10rem'
      },
      {
        align: 'center',
        bodyTemplate: inputStartTimeTemplate,
        field: 'startTime',
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
    [imageBodyTemplate, inputBitsPerSecondTemplate, inputStartTimeTemplate, targetActionBodyTemplate, videoStreamTemplate]
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
          columns={columns}
          dataSource={dataSource}
          defaultSortField="videoStreamName"
          emptyMessage="No Streams"
          id="StreamingServerStatusPanel"
          isLoading={getStreamingStatus.isLoading}
          dataKey="circularBufferId"
          selectedItemsKey="selectSelectedItems"
          style={style}
        />
      </div>
    </>
  );
};

StreamingServerStatusPanel.displayName = 'Streaming Server Status';

export default memo(StreamingServerStatusPanel);
