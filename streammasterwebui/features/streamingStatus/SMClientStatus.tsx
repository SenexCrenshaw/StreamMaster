import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { formatJSONDateString, getElapsedTimeString } from '@lib/common/dateTime';
import { ChannelMetric, ClientChannelDto } from '@lib/smAPI/smapiTypes';

import { GetChannelMetrics } from '@lib/smAPI/Statistics/StatisticsCommands';
import { useCallback, useEffect, useMemo, useRef, useState } from 'react';

interface IntClientChannelDto extends ClientChannelDto {
  readonly SourceName: string;
  readonly Logo?: string;
}

const SMClientStatus = () => {
  const [clientChannelMetrics, setClientChannelMetrics] = useState<IntClientChannelDto[]>([]);
  // const [loading, setLoading] = useState<boolean>(true);

  const channelMetricsRef = useRef<IntClientChannelDto[]>([]);

  const setChannelMetricsWithRef = (metrics: ChannelMetric[]) => {
    const sourceMetrics = metrics.filter((metric) => metric.SMStreamInfo === null);
    const intMetrics = [] as IntClientChannelDto[];

    for (const metric of sourceMetrics) {
      for (const clientChannel of metric.ClientChannels) {
        intMetrics.push({ ...clientChannel, Logo: metric.Logo, SourceName: metric.SourceName });
      }
    }

    channelMetricsRef.current = intMetrics;
    setClientChannelMetrics(intMetrics);

    // Log the client metrics for debugging
    // Logger.debug('SMClientStatus:', intMetrics);
  };

  const getChannelMetrics = useCallback(async () => {
    try {
      const [channelMetricsData] = await Promise.all([GetChannelMetrics()]);
      setChannelMetricsWithRef(channelMetricsData ?? []);
      // setLoading(false);
    } catch (error) {
      console.error('Error fetching channel metrics:', error);
      // setLoading(false);
    }
  }, []);

  useEffect(() => {
    const fetchData = () => {
      getChannelMetrics();
    };

    fetchData();
    const intervalId = setInterval(fetchData, 2000);

    return () => clearInterval(intervalId);
  }, [getChannelMetrics]);

  const clientBitsPerSecondTemplate = (rowData: ChannelMetric) => {
    if (rowData.Metrics.Kbps === undefined) return <div />;

    const found = channelMetricsRef.current.find((predicate) => predicate.Name === rowData.Name);

    if (found === undefined || found.Metrics === undefined) return <div />;

    const kbps = found.Metrics.Kbps;
    const roundedKbps = Math.ceil(kbps);

    return <div>{roundedKbps.toLocaleString('en-US')}</div>;
  };

  const averageLatencyTemplate = (rowData: ChannelMetric) => {
    if (rowData.Metrics.AverageLatency === undefined) return <div />;

    const found = channelMetricsRef.current.find((predicate) => predicate.Name === rowData.Name);

    if (found === undefined || found.Metrics === undefined) return <div />;
    return <div>{found.Metrics.AverageLatency.toFixed(2)}</div>;
  };

  const elapsedTSTemplate = useCallback((rowData: ChannelMetric) => {
    return <div className="numeric-field">{getElapsedTimeString(rowData.Metrics.StartTime, new Date().toString(), true)}</div>;
  }, []);

  const clientStartTimeTemplate = (rowData: ChannelMetric) => <div>{formatJSONDateString(rowData.Metrics.StartTime ?? '')}</div>;

  const actionTemplate = useCallback((data: ChannelMetric) => {
    return (
      <div className="sm-center-stuff">
        {/* <VideoInfoDisplay channelId={data.ChannelId} /> */}
        {/* <CancelClientDialog clientId={data.ClientId} /> */}
      </div>
    );
  }, []);

  const logoTemplate = useCallback((rowData: IntClientChannelDto) => {
    return (
      <div className="flex icon-button-template justify-content-center align-items-center w-full">
        <img alt="Icon logo" src={rowData.Logo} />
      </div>
    );
  }, []);

  const columns = useMemo(
    (): ColumnMeta[] => [
      { bodyTemplate: logoTemplate, field: 'Logo', fieldType: 'image', header: '' },
      { align: 'left', field: 'SourceName', filter: true, header: 'Source', sortable: true, width: 120 },
      { align: 'right', field: 'ClientIPAddress', filter: true, header: 'IPAddress', sortable: true, width: 100 },
      { align: 'right', field: 'ClientUserAgent', filter: true, header: 'Agent', sortable: true, width: 180 },

      { align: 'center', bodyTemplate: clientStartTimeTemplate, field: 'StartTime', header: 'StartTime', width: 140 },
      { align: 'right', bodyTemplate: clientBitsPerSecondTemplate, field: 'kbps', header: 'Kbps', width: 50 },
      { align: 'right', bodyTemplate: averageLatencyTemplate, field: 'AverageLatency', header: 'Read ms', width: 60 },
      { align: 'right', bodyTemplate: elapsedTSTemplate, field: 'ElapsedTime', header: '(d hh:mm:ss)', width: 95 },

      { align: 'center', bodyTemplate: actionTemplate, field: 'actions', fieldType: 'actions', header: '', width: 44 }
    ],
    [actionTemplate, elapsedTSTemplate, logoTemplate]
  );

  return <SMDataTable headerName="CLIENTS" columns={columns} id="SMClientStatus" dataKey="Name" dataSource={clientChannelMetrics} />;
};

export default SMClientStatus;
