import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { formatJSONDateString, getElapsedTimeString } from '@lib/common/dateTime';
import { ClientStreamingStatistics } from '@lib/smAPI/smapiTypes';
import { useCallback, useEffect, useMemo, useState } from 'react';
import { VideoInfoDisplay } from './VideoInfoDisplay';
import { GetClientStreamingStatistics } from '@lib/smAPI/Statistics/StatisticsCommands';
import CancelClientDialog from '@components/streaming/CancelClientDialog';

const SMClientStatus = () => {
  const [clientStreamingStatistics, setClientStreamingStatistics] = useState<ClientStreamingStatistics[]>([]);

  const getStats = useCallback(async () => {
    try {
      const [clientStats] = await Promise.all([GetClientStreamingStatistics()]);

      setClientStreamingStatistics(clientStats ?? []);
      // setStreamStreamingStatistics(streamStats ?? []);
    } catch (error) {}
  }, [setClientStreamingStatistics]);

  useEffect(() => {
    getStats();
    const intervalId = setInterval(getStats, 1000);
    return () => clearInterval(intervalId);
  }, [getStats]);

  const clientBitsPerSecondTemplate = (rowData: ClientStreamingStatistics) => {
    if (rowData.BitsPerSecond === undefined) return <div />;

    const kbps = rowData.BitsPerSecond / 1000;
    const roundedKbps = Math.ceil(kbps);

    return <div>{roundedKbps.toLocaleString('en-US')}</div>;
  };

  const elapsedTSTemplate = useCallback((rowData: ClientStreamingStatistics) => {
    return <div className="numeric-field">{getElapsedTimeString(rowData.StartTime, new Date().toString(), true)}</div>;
  }, []);

  const clientStartTimeTemplate = (rowData: ClientStreamingStatistics) => <div>{formatJSONDateString(rowData.StartTime ?? '')}</div>;

  const actionTemplate = useCallback((data: ClientStreamingStatistics) => {
    return (
      <div className="sm-center-stuff">
        <VideoInfoDisplay channelId={data.ChannelId} />
        <CancelClientDialog clientId={data.ClientId} />
      </div>
    );
  }, []);

  const columns = useMemo(
    (): ColumnMeta[] => [
      { align: 'left', field: 'ChannelName', filter: true, header: 'Channel', sortable: true, width: 120 },
      { align: 'right', field: 'ClientIPAddress', filter: true, header: 'IPAddress', sortable: true, width: 100 },
      { align: 'right', field: 'ClientAgent', filter: true, header: 'Agent', sortable: true, width: 180 },
      { align: 'right', bodyTemplate: clientBitsPerSecondTemplate, field: 'BitsPerSecond', header: 'Kbps', width: 50 },
      { align: 'center', bodyTemplate: clientStartTimeTemplate, field: 'StartTime', header: 'StartTime', width: 150 },
      { align: 'right', bodyTemplate: elapsedTSTemplate, field: 'ElapsedTime', header: '(d hh:mm:ss)', width: 85 },
      { align: 'center', bodyTemplate: actionTemplate, field: 'actions', fieldType: 'actions', header: '', width: 44 }
    ],
    [actionTemplate, elapsedTSTemplate]
  );

  if (clientStreamingStatistics === undefined) return <div>Loading...</div>;

  return <SMDataTable headerName="CLIENTS" columns={columns} id="clientStatus" dataSource={clientStreamingStatistics} />;
};

export default SMClientStatus;
