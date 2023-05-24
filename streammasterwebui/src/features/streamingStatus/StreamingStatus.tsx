
import React from 'react';

import StreamingClientsPanel from './StreamingClientsPanel';
import StreamingServerStatusPanel from './StreamingServerStatusPanel';
import { StreamingStatusIcon } from '../../common/icons';

export const StreamingStatus = () => {

  return (
    <div className="streamingStatus">
      <div className="grid grid-nogutter flex justify-content-between align-items-center">

        <div className="flex w-full text-left font-bold text-white-500 surface-overlay justify-content-start align-items-center">
          <StreamingStatusIcon className='p-0 mr-1' />
          {StreamingStatus.displayName?.toUpperCase()}
        </div >

        <div className="flex col-12 w-full mt-2">
          <StreamingServerStatusPanel />
        </div>
        <div className='flex col-12 w-full'>
          <StreamingClientsPanel />
        </div>
      </div>
    </div>
  );
};

StreamingStatus.displayName = 'Streaming Status';
StreamingStatus.defaultProps = {};

export default React.memo(StreamingStatus);
