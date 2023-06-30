
import { Column } from 'primereact/column';
import { DataTable } from 'primereact/datatable';
import React from 'react';
import { formatJSONDateString } from '../../common/common';

import { type StreamStatisticsResult } from '../../store/iptvApi';
import { useStreamGroupsGetAllStatisticsForAllUrlsQuery } from '../../store/iptvApi';

const StreamingClientsPanel = () => {

  const getStreamingStatus = useStreamGroupsGetAllStatisticsForAllUrlsQuery();


  const clientBitsPerSecondTemplate = (rowData: StreamStatisticsResult) => {
    return rowData.clientBitsPerSecond?.toLocaleString('en-US');
  };


  const clientStartTimeTemplate = (rowData: StreamStatisticsResult) => {
    return formatJSONDateString(rowData.clientStartTime ?? '');
  };

  const clientElapsedTimeTemplate = (rowData: StreamStatisticsResult) => {
    return rowData.clientElapsedTime?.split('.')[0];
  };

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
        value={getStreamingStatus.data}
      >
        clientAgent
        <Column
          field='clientAgent'
          header="Client/User Agent"
          key="clientAgent"
          sortable
        />
        <Column
          field='m3UStreamName'
          header="Name"
          key="m3UStreamName"
          sortable
        />
        <Column
          body={clientStartTimeTemplate}
          field="clientStartTime"
          header="Client Start"
          key="clientStartTime"
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
          body={clientBitsPerSecondTemplate}
          field="clientBitsPerSecond"
          header="Client Bps"
          key="clientBitsPerSecond"
          sortable
        />
      </DataTable>
    </div>
  );
};

StreamingClientsPanel.displayName = 'Streaming Clients Panel';
StreamingClientsPanel.defaultProps = {};

export default React.memo(StreamingClientsPanel);
