import DataSelector from '@components/dataSelector/DataSelector';
import { ColumnMeta } from '@components/dataSelector/DataSelectorTypes';
import { formatJSONDateString, getTopToolOptions } from '@lib/common/common';
import { FailClientRequest, StreamStatisticsResult, useVideoStreamsGetAllStatisticsForAllUrlsQuery } from '@lib/iptvApi';
import { FailClient } from '@lib/smAPI/VideoStreams/VideoStreamsMutateAPI';
import { Button } from 'primereact/button';
import { Toast } from 'primereact/toast';
import { memo, useRef, type CSSProperties, useEffect, useState } from 'react';

interface StreamingClientsPanelProperties {
  readonly className?: string;
  readonly style?: CSSProperties;
}

const StreamingClientsPanel = ({ className, style }: StreamingClientsPanelProperties) => {
  const toast = useRef<Toast>(null);
  const [dataSource, setDataSource] = useState<StreamStatisticsResult[]>([]);

  const getStreamingStatus = useVideoStreamsGetAllStatisticsForAllUrlsQuery();

  useEffect(() => {
    if (getStreamingStatus.data === undefined || getStreamingStatus.data.length === 0 || getStreamingStatus.data === null) {
      setDataSource([]);
      return;
    }

    let data = [...dataSource];

    for (const item of getStreamingStatus.data) {
      const index = data.findIndex((x) => x.clientId === item.clientId);
      if (index === -1) {
        data.push(item);
      } else {
        data[index] = {
          ...data[index],
          clientIPAddress: item.clientIPAddress !== data[index].clientIPAddress ? item.clientIPAddress : data[index].clientIPAddress,
          clientAgent: item.clientAgent !== data[index].clientAgent ? item.clientAgent : data[index].clientAgent,
          videoStreamName: item.videoStreamName !== data[index].videoStreamName ? item.videoStreamName : data[index].videoStreamName,
          clientStartTime: item.clientStartTime !== data[index].clientStartTime ? item.clientStartTime : data[index].clientStartTime,
          clientElapsedTime: item.clientElapsedTime !== data[index].clientElapsedTime ? item.clientElapsedTime : data[index].clientElapsedTime,
          clientBitsPerSecond: item.clientBitsPerSecond !== data[index].clientBitsPerSecond ? item.clientBitsPerSecond : data[index].clientBitsPerSecond,
          inputStartTime: item.inputStartTime !== data[index].inputStartTime ? item.inputStartTime : data[index].inputStartTime
        };
      }
    }

    for (const item of dataSource) {
      const index = getStreamingStatus.data.findIndex((x) => x.clientId === item.clientId);
      if (index === -1) {
        data = data.filter((x) => x.clientId !== item.clientId);
      }
    }

    setDataSource(data);
  }, [getStreamingStatus.data]);

  const clientBitsPerSecondTemplate = (rowData: StreamStatisticsResult) => {
    if (rowData.clientBitsPerSecond === undefined) return <div />;

    const kbps = rowData.clientBitsPerSecond / 1000;
    const roundedKbps = Math.ceil(kbps);

    return <div>{roundedKbps.toLocaleString('en-US')}</div>;
  };

  const clientStartTimeTemplate = (rowData: StreamStatisticsResult) => <div>{formatJSONDateString(rowData.clientStartTime ?? '')}</div>;

  const clientElapsedTimeTemplate = (rowData: StreamStatisticsResult) => <div>{rowData.clientElapsedTime?.split('.')[0]}</div>;

  const onFailClient = async (rowData: StreamStatisticsResult) => {
    if (!rowData.clientId || rowData.clientId === undefined || rowData.clientId === '') {
      return;
    }

    const toSend = {} as FailClientRequest;

    toSend.clientId = rowData.clientId;

    await FailClient(toSend)
      .then(() => {
        if (toast.current) {
          toast.current.show({
            detail: 'Failed Client',
            life: 3000,
            severity: 'success',
            summary: 'Successful'
          });
        }
      })
      .catch(() => {
        if (toast.current) {
          toast.current.show({
            detail: 'Failed to Fail Client',
            life: 3000,
            severity: 'error',
            summary: 'Error'
          });
        }
      });
  };

  const targetActionBodyTemplate = (rowData: StreamStatisticsResult) => (
    <div className="dataselector p-inputgroup align-items-center justify-content-end">
      <Button
        className="p-button-danger"
        icon="pi pi-times"
        onClick={async () => await onFailClient(rowData)}
        rounded
        text
        tooltip="Fail Client"
        tooltipOptions={getTopToolOptions}
      />
    </div>
  );

  const columns = (): ColumnMeta[] => [
    {
      field: 'clientIPAddress',
      header: 'Client/IP Address',
      style: {
        maxWidth: '14rem',
        width: '14rem'
      } as CSSProperties
    },
    {
      field: 'clientAgent',
      header: 'Client/User Agent',
      style: {
        maxWidth: '14rem',
        width: '14rem'
      } as CSSProperties
    },
    { field: 'videoStreamName', header: 'Name' },

    {
      align: 'center',
      bodyTemplate: clientStartTimeTemplate,
      field: 'clientStartTime',
      header: 'Client Start',
      style: {
        maxWidth: '10rem',
        width: '10rem'
      } as CSSProperties
    },
    {
      align: 'center',
      bodyTemplate: clientElapsedTimeTemplate,
      field: 'clientElapsedTime',
      header: 'Client Elapsed',
      style: {
        maxWidth: '10rem',
        width: '10rem'
      } as CSSProperties
    },
    {
      align: 'center',
      bodyTemplate: clientBitsPerSecondTemplate,
      field: 'clientBitsPerSecond',
      header: 'Client kbps',
      style: {
        maxWidth: '10rem',
        width: '10rem'
      } as CSSProperties
    },
    {
      align: 'center',
      bodyTemplate: targetActionBodyTemplate,
      field: 'Actions',
      style: {
        maxWidth: '8rem',
        width: '8rem'
      } as CSSProperties
    }
  ];

  return (
    <>
      <Toast position="bottom-right" ref={toast} />
      <div className="m3uFilesEditor flex flex-column col-12 flex-shrink-0 ">
        <DataSelector
          className={className}
          columns={columns()}
          defaultSortField="clientStartTime"
          dataSource={dataSource}
          emptyMessage="No Clients Streaming"
          id="StreamingServerStatusPanel"
          isLoading={getStreamingStatus.isLoading}
          dataKey="clientId"
          selectedItemsKey="selectSelectedItems"
          style={style}
        />
      </div>
    </>
  );
};

export default memo(StreamingClientsPanel);
