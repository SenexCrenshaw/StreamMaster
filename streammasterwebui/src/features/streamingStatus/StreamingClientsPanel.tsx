
import { Column } from 'primereact/column';
import { DataTable } from 'primereact/datatable';
import React from 'react';
import { formatJSONDateString } from '../../common/common';
import StreamMasterSetting from '../../store/signlar/StreamMasterSetting';


import { type StreamStatisticsResult } from '../../store/iptvApi';
import { useStreamGroupsGetAllStatisticsForAllUrlsQuery } from '../../store/iptvApi';

const StreamingClientsPanel = () => {
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

  const clientBitsPerSecondTemplate = (rowData: StreamStatisticsResult) => {
    return rowData.clientBitsPerSecond?.toLocaleString('en-US');
  };

  const clientBytesReadTemplate = (rowData: StreamStatisticsResult) => {
    return rowData.clientBytesRead?.toLocaleString('en-US');
  };

  const inputStartTimeTemplate = (rowData: StreamStatisticsResult) => {
    return formatJSONDateString(rowData.inputStartTime ?? '');
  };

  const clientStartTimeTemplate = (rowData: StreamStatisticsResult) => {
    return formatJSONDateString(rowData.clientStartTime ?? '');
  };

  const inputBytesWrittenTemplate = (rowData: StreamStatisticsResult) => {
    return rowData.inputBytesWritten?.toLocaleString('en-US');
  };

  const inputElapsedTimeTemplate = (rowData: StreamStatisticsResult) => {
    return rowData.inputElapsedTime?.split('.')[0];
  };

  const clientElapsedTimeTemplate = (rowData: StreamStatisticsResult) => {
    return rowData.clientElapsedTime?.split('.')[0];
  };

  return (
    <div className="flex w-full">
      <DataTable
        className="w-full text-sm"
        emptyMessage="No Status Found"
        // header={renderHeader()}
        key="id"
        loading={getStreamingStatus.isLoading}
        showGridlines
        stripedRows
        value={getStreamingStatus.data}
      >
        <Column
          body={imageBodyTemplate}
          header="Icon"
          key="icon"
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
          body={inputBytesWrittenTemplate}
          field="inputBytesWritten"
          header="Bytes Read"
          key="inputBytesWritten"
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
        <Column
          body={clientBitsPerSecondTemplate}
          field="clientBitsPerSecond"
          header="Client Bps"
          key="clientBitsPerSecond"
          sortable
        />
        <Column
          body={clientBytesReadTemplate}
          field="clientBytesRead"
          header="Client Bytes Written"
          key="clientBytesRead"
          sortable
        />
        <Column
          body={clientElapsedTimeTemplate}
          field="clientElapsedTime"
          header="Client Elapsed"
          key="clientElapsedTime"
          sortable
        />
        <Column
          body={clientStartTimeTemplate}
          field="clientStartTime"
          header="Client Start"
          key="clientStartTime"
          sortable
        />
      </DataTable>
    </div>
  );
};

StreamingClientsPanel.displayName = 'Streaming Clients Panel';
StreamingClientsPanel.defaultProps = {};

export default React.memo(StreamingClientsPanel);
