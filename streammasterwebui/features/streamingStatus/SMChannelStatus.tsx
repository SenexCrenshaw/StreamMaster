import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { formatJSONDateString, getElapsedTimeString } from '@lib/common/dateTime';
import { useSMContext } from '@lib/signalr/SMProvider';
import { InputStreamingStatistics } from '@lib/smAPI/smapiTypes';
import { DataTableRowData, DataTableRowExpansionTemplate } from 'primereact/datatable';
import { useCallback, useMemo } from 'react';
import SMChannelStatusValue from './SMChannelStatusValue';

const SMChannelStatus = () => {
  const { inputStatistics } = useSMContext();

  const clientBitsPerSecondTemplate = (rowData: InputStreamingStatistics) => {
    if (rowData.BitsPerSecond === undefined) return <div />;

    const kbps = rowData.BitsPerSecond / 1000;
    const roundedKbps = Math.ceil(kbps);

    return <div>{roundedKbps.toLocaleString('en-US')}</div>;
  };

  const elapsedTSTemplate = useCallback((rowData: InputStreamingStatistics) => {
    return <div className="numeric-field">{getElapsedTimeString(rowData.StartTime, new Date().toString(), true)}</div>;
  }, []);

  const clientStartTimeTemplate = (rowData: InputStreamingStatistics) => <div>{formatJSONDateString(rowData.StartTime ?? '')}</div>;

  const columns = useMemo(
    (): ColumnMeta[] => [
      { field: 'ChannelName', filter: true, sortable: true, width: 300 },
      { align: 'right', bodyTemplate: clientBitsPerSecondTemplate, field: 'BitsPerSecond', header: 'Kbps', width: 50 },
      { align: 'center', bodyTemplate: clientStartTimeTemplate, field: 'StartTime', header: 'StartTime', width: 180 },
      { align: 'right', bodyTemplate: elapsedTSTemplate, field: 'ElapsedTime', header: '(d hh:mm:ss:ms)', width: 150 }
    ],
    [elapsedTSTemplate]
  );

  const rowExpansionTemplate = useCallback((data: DataTableRowData<any>, options: DataTableRowExpansionTemplate) => {
    const inputStreamingStatistics = data as unknown as InputStreamingStatistics;

    return <div className="ml-3 m-1">{<SMChannelStatusValue channelId={inputStreamingStatistics.ChannelId} />}</div>;
  }, []);

  if (inputStatistics === undefined) return <div>Loading...</div>;

  return (
    <SMDataTable
      headerName="CHANNELS"
      columns={columns}
      id="channelStatus"
      dataSource={inputStatistics}
      showExpand
      rowExpansionTemplate={rowExpansionTemplate}
    />
  );
};

export default SMChannelStatus;
