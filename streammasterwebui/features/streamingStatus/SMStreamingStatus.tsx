import { GetChannelStreamingStatistics, GetClientStreamingStatistics } from '@lib/smAPI/Statistics/StatisticsCommands';
import { ChannelStreamingStatistics, ClientStreamingStatistics } from '@lib/smAPI/smapiTypes';
import React, { useCallback, useEffect, useState } from 'react';
import SMChannelStatus from './SMChannelStatus';
import SMClientStatus from './SMClientStatus';

export const StreamingStatus = (): JSX.Element => {
  const [channelStreamingStatistics, setChannelStreamingStatistics] = useState<ChannelStreamingStatistics[]>([]);
  const [clientStreamingStatistics, setClientStreamingStatistics] = useState<ClientStreamingStatistics[]>([]);
  // const [streamStreamingStatistics, setStreamStreamingStatistics] = useState<StreamStreamingStatistic[]>([]);

  const getStats = useCallback(async () => {
    try {
      const [channelStats, clientStats] = await Promise.all([
        GetChannelStreamingStatistics(),
        GetClientStreamingStatistics()
        // GetStreamStreamingStatistics()
      ]);

      setChannelStreamingStatistics(channelStats ?? []);
      setClientStreamingStatistics(clientStats ?? []);
      // setStreamStreamingStatistics(streamStats ?? []);
    } catch (error) {}
  }, [setChannelStreamingStatistics, setClientStreamingStatistics]);

  useEffect(() => {
    // getStats();
    // const intervalId = setInterval(getStats, 1000);
    // return () => clearInterval(intervalId);
  }, [getStats]);

  return (
    <div className="flex flex-column w-full">
      <div className="flex flex-row justify-content-between gap-2 w-full pr-2">
        <div className="sm-w-6">
          <SMChannelStatus channelStreamingStatistics={channelStreamingStatistics} />
        </div>

        <div className="sm-w-6">
          <SMClientStatus clientStreamingStatistics={clientStreamingStatistics} />
        </div>
      </div>
    </div>
  );
};

StreamingStatus.displayName = 'Streaming Status';

export default React.memo(StreamingStatus);
