import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { formatJSONDateString, getElapsedTimeString } from '@lib/common/dateTime';
import { ChannelMetric } from '@lib/smAPI/smapiTypes';
import { DataTableRowData, DataTableRowExpansionTemplate } from 'primereact/datatable';
import { useCallback, useEffect, useMemo, useRef, useState } from 'react';

import CancelChannelDialog from '@components/streaming/CancelChannelDialog';
import MoveToNextStreamDialog from '@components/streaming/MoveToNextStreamDialog';
import { GetChannelMetrics } from '@lib/smAPI/Statistics/StatisticsCommands';

const SMChannelStatus = () => {
  const [channelMetrics, setChannelMetrics] = useState<ChannelMetric[]>([]);
  const [loading, setLoading] = useState<boolean>(true);

  const channelMetricsRef = useRef<ChannelMetric[]>([]);
  const setChannelMetricsWithRef = (metrics: ChannelMetric[]) => {
    channelMetricsRef.current = metrics;
    setChannelMetrics(metrics);
  };

  const getChannelMetrics = useCallback(async () => {
    try {
      const [channelMetricsData] = await Promise.all([GetChannelMetrics()]);
      setChannelMetricsWithRef(channelMetricsData ?? []);
      setLoading(false);
    } catch (error) {
      console.error('Error fetching channel metrics:', error);
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    const fetchData = () => {
      getChannelMetrics();
    };

    fetchData();
    const intervalId = setInterval(fetchData, 3000);

    return () => clearInterval(intervalId);
  }, [getChannelMetrics]);

  const clientBitsPerSecondTemplate = (rowData: ChannelMetric) => {
    const found = channelMetricsRef.current.find((predicate) => predicate.Id === rowData.Id);

    if (found === undefined || found.GetMetrics.Kbps === undefined) return <div />;
    const kbps = found.GetMetrics.Kbps;
    const roundedKbps = Math.ceil(kbps);
    return <div>{roundedKbps.toLocaleString('en-US')}</div>;
  };

  const elapsedTSTemplate = useCallback((rowData: ChannelMetric) => {
    return <div className="numeric-field">{getElapsedTimeString(rowData.StartTime, new Date().toString(), true)}</div>;
  }, []);

  const actionTemplate = useCallback((rowData: ChannelMetric) => {
    return (
      <div className="sm-center-stuff">
        <CancelChannelDialog channelId={rowData.Id} />
        <MoveToNextStreamDialog channelId={rowData.Id} />
      </div>
    );
  }, []);

  const clientStartTimeTemplate = (rowData: ChannelMetric) => <div>{formatJSONDateString(rowData.StartTime ?? '')}</div>;

  const logoTemplate = useCallback((rowData: ChannelMetric) => {
    return (
      <div className="flex icon-button-template justify-content-center align-items-center w-full">
        <img alt="Icon logo" src={rowData.Logo} />
      </div>
    );
  }, []);

  const columns = useMemo(
    (): ColumnMeta[] => [
      { bodyTemplate: logoTemplate, field: 'Logo', fieldType: 'image', header: '' },
      { field: 'Name', filter: true, sortable: true, width: 200 },
      { align: 'right', bodyTemplate: clientBitsPerSecondTemplate, field: 'GetMetrics.Kbps', header: 'Kbps', width: 80 }
      // { align: 'center', bodyTemplate: clientStartTimeTemplate, field: 'StartTime', header: 'Start', width: 180 },
      // { align: 'right', bodyTemplate: elapsedTSTemplate, field: 'ElapsedTime', header: '(d hh:mm:ss)', width: 85 },
      // { align: 'center', bodyTemplate: actionTemplate, field: 'actions', fieldType: 'actions', header: '', width: 42 }
    ],
    [logoTemplate]
  );

  const rowExpansionTemplate = useCallback((data: DataTableRowData<any>, options: DataTableRowExpansionTemplate) => {
    const ChannelMetric = data as unknown as ChannelMetric;
    return <div />;
    // return (
    //   <div className="ml-3 m-1">
    //     {
    //       <SMChannelStatusValue
    //         ChannelId={ChannelMetric.Id}
    //         CurrentRank={ChannelMetric.CurrentRank}
    //         StreamStreamingStatistics={ChannelMetric.StreamStreamingStatistics}
    //       />
    //     }
    //   </div>
    // );
  }, []);

  if (loading) return <div>Loading...</div>;

  return (
    <SMDataTable
      headerName="CHANNELS"
      columns={columns}
      id="channelStatus"
      dataSource={channelMetrics}
      showExpand
      rowExpansionTemplate={rowExpansionTemplate}
    />
  );
};

export default SMChannelStatus;
