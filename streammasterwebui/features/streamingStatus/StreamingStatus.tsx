import StandardHeader from '@components/StandardHeader';
import { StreamingStatusIcon } from '@lib/common/icons';
import { GetChannelStreamingStatistics, GetClientStreamingStatistics } from '@lib/smAPI/Statistics/StatisticsCommands';
import { ChannelStreamingStatistics, ClientStreamingStatistics } from '@lib/smAPI/smapiTypes';
import React, { useCallback, useEffect, useState } from 'react';
import SMChannelStatus from './SMChannelStatus';
import SMClientsStatus from './SMClientStatus';

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
    getStats();
    const intervalId = setInterval(getStats, 1000);
    return () => clearInterval(intervalId);
  }, [getStats]);
  return (
    <StandardHeader className="flex-column" displayName="Streaming Status" icon={<StreamingStatusIcon />}>
      <div className="flex flex-row justify-content-between gap-2 w-full pr-2">
        <div className="sm-w-6">
          <SMChannelStatus channelStreamingStatistics={channelStreamingStatistics} />
        </div>

        <div className="sm-w-6">
          <SMClientsStatus clientStreamingStatistics={clientStreamingStatistics} />
        </div>
      </div>
    </StandardHeader>
  );
};

StreamingStatus.displayName = 'Streaming Status';

export default React.memo(StreamingStatus);
