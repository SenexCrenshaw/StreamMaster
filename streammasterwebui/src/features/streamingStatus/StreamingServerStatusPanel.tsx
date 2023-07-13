
import { Column } from 'primereact/column';
import { DataTable } from 'primereact/datatable';
import React from 'react';
import { type StreamStatisticsResult } from '../../store/iptvApi';
import { formatJSONDateString } from '../../common/common';
import StreamMasterSetting from '../../store/signlar/StreamMasterSetting';


export const StreamingServerStatusPanel = (props: StreamingServerStatusPanelProps) => {
  const setting = StreamMasterSetting();


  const imageBodyTemplate = React.useCallback((rowData: StreamStatisticsResult) => {
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
  }, [setting.defaultIcon]);

  const inputBitsPerSecondTemplate = React.useCallback((rowData: StreamStatisticsResult) => {

    if (rowData.inputBitsPerSecond === undefined) return undefined;

    const kbps = rowData.inputBitsPerSecond / 1000;
    const roundedKbps = Math.ceil(kbps);
    return roundedKbps.toLocaleString('en-US');
  }, []);

  const inputElapsedTimeTemplate = React.useCallback((rowData: StreamStatisticsResult) => {
    return rowData.inputElapsedTime?.split('.')[0];
  }, []);

  const inputStartTimeTemplate = React.useCallback((rowData: StreamStatisticsResult) => {
    return formatJSONDateString(rowData.inputStartTime ?? '');
  }, []);

  const dataSource = React.useMemo((): StreamStatisticsResult[] => {
    let data = [] as StreamStatisticsResult[];

    props.dataSource.forEach((item) => {
      if (data.findIndex((x) => x.m3UStreamId === item.m3UStreamId) === -1) {
        data.push(item);
      }

    });

    return data;
  }, [props.dataSource]);

  const streamCount = React.useCallback((rowData: StreamStatisticsResult) => {
    return props.dataSource.filter((x) => x.m3UStreamId === rowData.m3UStreamId).length;
  }, [props.dataSource])

  return (

    <div className="flex w-full">
      <DataTable
        className="w-full text-sm"
        emptyMessage="No Status Found"
        key="id"
        loading={props.isLoading}
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
          body={streamCount}
          header="Count"
          key="streamCount"
          sortable
        />
        <Column
          body={inputBitsPerSecondTemplate}
          field="inputBitsPerSecond"
          header="Input kbps"
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
type StreamingServerStatusPanelProps = {
  dataSource: StreamStatisticsResult[];
  isLoading: boolean;
}
export default React.memo(StreamingServerStatusPanel);
