

import React from 'react';

import StreamingClientsPanel from './StreamingClientsPanel';
import StreamingServerStatusPanel from './StreamingServerStatusPanel';
import { StreamingStatusIcon } from '../../common/icons';
import { type StreamStatisticsResult } from '../../store/iptvApi';
import { useVideoStreamsGetAllStatisticsForAllUrlsQuery } from '../../store/iptvApi';
import StreamingStatusGraph from '../../components/StreamingStatusGraph';
import { set } from 'video.js/dist/types/tech/middleware';

export const StreamingStatus = () => {
  const getStreamingStatus = useVideoStreamsGetAllStatisticsForAllUrlsQuery();


  const [dataSource, setDataSource] = React.useState<StreamStatisticsResult[]>([] as StreamStatisticsResult[]);

  React.useEffect(() => {
    if (getStreamingStatus.data === undefined || getStreamingStatus.data === null) {
      return;
    }

    setDataSource(getStreamingStatus.data);
  }, [getStreamingStatus.data]);


  return (
    <div className="streamingStatus">
      <div className="grid grid-nogutter flex justify-content-between align-items-center">

        <div className="flex w-full text-left font-bold text-white-500 surface-overlay justify-content-start align-items-center">
          <StreamingStatusIcon className='p-0 mr-1' />
          {StreamingStatus.displayName?.toUpperCase()}
        </div >

        <div className="flex col-12 w-full mt-1">
          <StreamingServerStatusPanel dataSource={dataSource} isLoading={getStreamingStatus.isLoading} style={{ height: 'calc(50vh - 140px)' }} />
        </div>
        <div className='flex col-12 w-full mt-1'>
          <StreamingClientsPanel dataSource={dataSource} isLoading={getStreamingStatus.isLoading} style={{ height: 'calc(50vh + 60px)' }} />
        </div>
        {/*
        <div className="flex col-12 w-full">
          <StreamingStatusGraph className='border-1 w-full' dataSource={dataSource} isLoading={getStreamingStatus.isLoading} style={{ height: 'calc(20vh)' }} />
        </div> */}

      </div>
    </div>
  );
};

StreamingStatus.displayName = 'Streaming Status';
StreamingStatus.defaultProps = {};

export default React.memo(StreamingStatus);
