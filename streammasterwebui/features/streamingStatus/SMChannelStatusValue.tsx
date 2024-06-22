import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { formatJSONDateString, getElapsedTimeString } from '@lib/common/dateTime';
import { Logger } from '@lib/common/logger';
import { useSMContext } from '@lib/signalr/SMProvider';
import { InputStreamingStatistics } from '@lib/smAPI/smapiTypes';
import { useCallback, useMemo } from 'react';

interface SMChannelStatusProperties {
  readonly channelId: number;
}

const SMChannelStatusValue = ({ channelId }: SMChannelStatusProperties) => {
  const { clientStreamingStatistics } = useSMContext();

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
      { bodyTemplate: clientBitsPerSecondTemplate, field: 'BitsPerSecond', header: 'Kbps', width: 50 },
      { bodyTemplate: clientStartTimeTemplate, field: 'StartTime', header: 'StartTime', width: 150 },
      { align: 'right', bodyTemplate: elapsedTSTemplate, field: 'ElapsedTime', header: '(d hh:mm:ss:ms)', width: 150 }
    ],
    [elapsedTSTemplate]
  );

  const dataSource = useMemo(() => {
    if (clientStreamingStatistics === undefined || channelId === undefined) return [];
    const data = clientStreamingStatistics.find((x) => x.ChannelId === channelId);
    if (data === undefined) return [];
    return [data];
  }, [channelId, clientStreamingStatistics]);

  if (dataSource === undefined) return <div>Loading...</div>;

  Logger.debug('SMChannelStatusValue', 'dataSource', dataSource);

  return <SMDataTable columns={columns} id="channelStatus" dataSource={dataSource} />;
};

export default SMChannelStatusValue;
