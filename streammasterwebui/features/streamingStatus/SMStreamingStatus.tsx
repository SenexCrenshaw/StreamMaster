import { memo } from 'react';
import SMChannelStatus from './SMChannelStatus';
import SMClientStatus from './SMClientStatus';

export const StreamingStatus = (): JSX.Element => {
  return (
    <div className="flex flex-row gap-2 w-full px-1">
      <div className="sm-w-5">
        <SMChannelStatus />
      </div>
      <div className="sm-w-7">
        <SMClientStatus />
      </div>
    </div>
  );
};

StreamingStatus.displayName = 'Streaming Status';

export default memo(StreamingStatus);
