import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { formatJSONDateString, getElapsedTimeString } from '@lib/common/dateTime';
import { useSMContext } from '@lib/signalr/SMProvider';
import { ClientStreamingStatistics } from '@lib/smAPI/smapiTypes';
import { useCallback, useMemo } from 'react';

const SMClientsStatus = () => {
  const { clientStreamingStatistics } = useSMContext();

  const clientBitsPerSecondTemplate = (rowData: ClientStreamingStatistics) => {
    if (rowData.ReadBitsPerSecond === undefined) return <div />;

    const kbps = rowData.ReadBitsPerSecond / 1000;
    const roundedKbps = Math.ceil(kbps);

    return <div>{roundedKbps.toLocaleString('en-US')}</div>;
  };

  const elapsedTSTemplate = useCallback((rowData: ClientStreamingStatistics) => {
    return <div className="numeric-field">{getElapsedTimeString(rowData.StartTime, new Date().toString(), true)}</div>;
  }, []);

  const clientStartTimeTemplate = (rowData: ClientStreamingStatistics) => <div>{formatJSONDateString(rowData.StartTime ?? '')}</div>;

  const columns = useMemo(
    (): ColumnMeta[] => [
      { align: 'left', field: 'ChannelName', filter: true, header: 'Channel', sortable: true, width: 120 },
      { align: 'center', field: 'ClientIPAddress', filter: true, header: 'IPAddress', sortable: true, width: 100 },
      { align: 'center', field: 'ClientAgent', filter: true, header: 'Agent', sortable: true, width: 180 },
      { align: 'right', bodyTemplate: clientBitsPerSecondTemplate, field: 'BitsPerSecond', header: 'Kbps', width: 60 },
      { align: 'center', bodyTemplate: clientStartTimeTemplate, field: 'StartTime', header: 'StartTime', width: 180 },
      { align: 'right', bodyTemplate: elapsedTSTemplate, field: 'ElapsedTime', header: '(d hh:mm:ss:ms)', width: 150 }
    ],
    [elapsedTSTemplate]
  );

  if (clientStreamingStatistics === undefined) return <div>Loading...</div>;

  return <SMDataTable headerName="CLIENTS" columns={columns} id="clientStatus" dataSource={clientStreamingStatistics} />;
};

export default SMClientsStatus;
