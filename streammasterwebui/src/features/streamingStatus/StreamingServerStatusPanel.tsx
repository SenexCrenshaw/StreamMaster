
import { Column } from 'primereact/column';
import { DataTable } from 'primereact/datatable';
import React from 'react';
import { type StreamStatisticsResult } from '../../store/iptvApi';
import { useStreamGroupsGetAllStatisticsForAllUrlsQuery } from '../../store/iptvApi';
import { formatJSONDateString } from '../../common/common';
import StreamMasterSetting from '../../store/signlar/StreamMasterSetting';


export const StreamingServerStatusPanel = () => {
  const setting = StreamMasterSetting();

  const getStreamingStatus = useStreamGroupsGetAllStatisticsForAllUrlsQuery();

  const imageBodyTemplate = (rowData: StreamStatisticsResult) => {
    return (
      <div className="flex align-content-center flex-wrap">
        <img
          alt={rowData.logo ?? 'logo'}
          className="flex align-items-center justify-content-center max-w-full max-h-2rem h-2rem"
          onError={(e: React.SyntheticEvent<HTMLImageElement, Event>) => (e.currentTarget.src = (e.currentTarget.src = setting.defaultIcon))}
          src={`${encodeURI(rowData.logo ?? '')}`}
        />
      </div>
    );
  };

  const inputBitsPerSecondTemplate = (rowData: StreamStatisticsResult) => {
    return rowData.inputBitsPerSecond?.toLocaleString('en-US');
  };

  const inputElapsedTimeTemplate = (rowData: StreamStatisticsResult) => {
    return rowData.inputElapsedTime?.split('.')[0];
  };

  const inputStartTimeTemplate = (rowData: StreamStatisticsResult) => {
    return formatJSONDateString(rowData.inputStartTime ?? '');
  };

  const dataSource = React.useMemo((): StreamStatisticsResult[] => {
    let data = [] as StreamStatisticsResult[];

    getStreamingStatus.data?.forEach((item) => {
      if (data.findIndex((x) => x.m3UStreamId === item.m3UStreamId) === -1) {
        data.push(item);
      }

    });

    return data;
  }, [getStreamingStatus.data]);

  return (

    <div className="flex w-full">
      <DataTable
        className="w-full text-sm"
        emptyMessage="No Status Found"
        key="id"
        loading={getStreamingStatus.isLoading}
        showGridlines
        stripedRows
        style={{ height: 'calc(50vh - 40px)' }}
        value={dataSource}
      >
        <Column
          body={imageBodyTemplate}
          header="Icon"
          key="icon"
          sortable
        />
        <Column
          field='m3UStreamName'
          header="Name"
          key="m3UStreamName"
          sortable
        />

        <Column
          body={inputBitsPerSecondTemplate}
          field="inputBitsPerSecond"
          header="Input Bps"
          key="inputBitsPerSecond"
          sortable
        />
        <Column
          body={inputElapsedTimeTemplate}
          field="inputElapsedTime"
          header="Input Elapsed"
          key="inputBytesWritten"
          sortable
        />
        <Column
          body={inputStartTimeTemplate}
          field="inputStartTime"
          header="input Start"
          key="inputStartTime"
          sortable
        />
      </DataTable>
    </div>

  );
};

StreamingServerStatusPanel.displayName = 'Streaming Server Status';
StreamingServerStatusPanel.defaultProps = {};

export default React.memo(StreamingServerStatusPanel);
