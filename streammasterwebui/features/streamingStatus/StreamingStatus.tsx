import React from 'react';

import StandardHeader from '@components/StandardHeader';
import { StreamingStatusIcon } from '@lib/common/icons';
import StreamingClientsPanel from './StreamingClientsPanel';
import StreamingServerStatusPanel from './StreamingServerStatusPanel';

export const StreamingStatus = () => {
  return (
    <StandardHeader className="flex-column" displayName="Streaming Status" icon={<StreamingStatusIcon />}>
      <StreamingServerStatusPanel style={{ height: 'calc(50vh - 140px)' }} />
      <StreamingClientsPanel style={{ height: 'calc(50vh + 50px)' }} />
    </StandardHeader>
  );
};

StreamingStatus.displayName = 'Streaming Status';

export default React.memo(StreamingStatus);
