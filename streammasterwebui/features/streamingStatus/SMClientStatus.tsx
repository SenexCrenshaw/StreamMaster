import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { formatJSONDateString, getElapsedTimeString } from '@lib/common/dateTime';
import { ClientStreamingStatistics } from '@lib/smAPI/smapiTypes';
import { useCallback, useMemo } from 'react';
import { VideoInfoDisplay } from './VideoInfoDisplay';

interface SMClientStatusProps {
  readonly clientStreamingStatistics: ClientStreamingStatistics[];
}

const SMClientStatus = ({ clientStreamingStatistics }: SMClientStatusProps) => {
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
    return <VideoInfoDisplay channelId={data.ChannelId} />;
  }, []);

  const columns = useMemo(
    (): ColumnMeta[] => [
      { align: 'left', field: 'ChannelName', filter: true, header: 'Channel', sortable: true, width: 120 },
      { align: 'right', field: 'ClientIPAddress', filter: true, header: 'IPAddress', sortable: true, width: 100 },
      { align: 'right', field: 'ClientAgent', filter: true, header: 'Agent', sortable: true, width: 180 },
      { align: 'right', bodyTemplate: clientBitsPerSecondTemplate, field: 'BitsPerSecond', header: 'Kbps', width: 50 },
      { align: 'center', bodyTemplate: clientStartTimeTemplate, field: 'StartTime', header: 'StartTime', width: 150 },
      { align: 'right', bodyTemplate: elapsedTSTemplate, field: 'ElapsedTime', header: '(d hh:mm:ss)', width: 85 },
      { align: 'center', bodyTemplate: actionTemplate, field: 'IsHidden', fieldType: 'actions', header: '', width: 24 }
    ],
    [actionTemplate, elapsedTSTemplate]
  );

  if (clientStreamingStatistics === undefined) return <div>Loading...</div>;

  return <SMDataTable headerName="CLIENTS" columns={columns} id="clientStatus" dataSource={clientStreamingStatistics} />;
};

export default SMClientStatus;
