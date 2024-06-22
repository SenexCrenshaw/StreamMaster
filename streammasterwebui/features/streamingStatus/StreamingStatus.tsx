import StandardHeader from '@components/StandardHeader';
import { StreamingStatusIcon } from '@lib/common/icons';
import React from 'react';
import SMChannelStatus from './SMChannelStatus';
import SMClientsStatus from './SMClientsStatus';

export const StreamingStatus = (): JSX.Element => (
  <StandardHeader className="flex-column" displayName="Streaming Status" icon={<StreamingStatusIcon />}>
    <div className="flex flex-row justify-content-between gap-2 w-full pr-2">
      <div className="sm-w-6">
        <SMChannelStatus />
      </div>
      {/* <StreamingClientsPanel style={{ height: 'calc(50vh + 50px)' }} /> */}
      <div className="sm-w-6">
        <SMClientsStatus />
      </div>
    </div>
  </StandardHeader>
);

StreamingStatus.displayName = 'Streaming Status';

export default React.memo(StreamingStatus);
