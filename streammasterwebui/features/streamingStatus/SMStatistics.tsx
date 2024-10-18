import { ChannelMetric } from '@lib/smAPI/smapiTypes';
import { useCallback, useEffect, useMemo, useRef, useState } from 'react';

import { GetChannelMetrics } from '@lib/smAPI/Statistics/StatisticsCommands';
import { ReactFlow } from '@xyflow/react';
import '@xyflow/react/dist/style.css';

const SMStatistics = () => {
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

  const initialNodes = [
    { id: '1', position: { x: 0, y: 0 }, data: { label: '1' } },
    { id: '2', position: { x: 0, y: 100 }, data: { label: '2' } }
  ];
  const initialEdges = [{ id: 'e1-2', source: '1', target: '2' }];

  const nodes = useMemo(() => {
    if (loading) {
      return [];
    }

    const sourceMetrics = channelMetrics.filter((metric) => metric.SMStreamInfo !== null);

    const data = sourceMetrics.map((metric, index) => ({
      data: { label: 'Source: ' + metric.Name },
      id: metric.SMStreamInfo!.Id,
      position: {
        x: (index % 5) * 150,
        y: Math.floor(index / 5) * 100
      }
    }));

    let index = sourceMetrics.length;
    sourceMetrics.forEach((metric) => {
      if (metric.ClientChannels) {
        metric.ClientChannels.forEach((channel) => {
          const sourceNode = data.find((node) => node.id === metric.Id);

          const targetNode = {
            data: { label: channel.Name },
            id: channel.SMChannelId.toString(),
            position: {
              x: (index % 5) * 150,
              y: Math.floor(index / 5) * 100
            }
          };

          if (targetNode) {
            data.push(targetNode);
          }
          ++index;
        });
      }
    });

    return data;
  }, [channelMetrics, loading]);

  useEffect(() => {
    const fetchData = () => {
      getChannelMetrics();
    };

    fetchData();
    const intervalId = setInterval(fetchData, 3000);

    return () => clearInterval(intervalId);
  }, [getChannelMetrics]);
  return (
    <div style={{ height: '100vh', width: '100vw' }}>
      {/* <ReactFlow nodes={initialNodes} edges={initialEdges} /> */}
      <ReactFlow nodes={nodes} />
    </div>
  );
};

export default SMStatistics;
